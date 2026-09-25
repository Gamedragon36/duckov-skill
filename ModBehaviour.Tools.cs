using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 특수 스킬(투척술·파밍)의 실제 동작.
    /// 게임에 별도 이벤트가 없어서 주변 상황을 짧은 간격으로 확인해 처리한다.
    /// </summary>
    public partial class ModBehaviour
    {
        // ---- 투척술 ----
        private float _grenadeScanTimer;
        private float _grenadeWatchUntil;
        private readonly HashSet<int> _handledGrenades = new HashSet<int>();

        // ---- 파밍 ----
        private float _lootScanTimer;
        // 엘리트 파밍: 상자(시체)마다 **열 때 한 번만** 굴린 결과를 담아 둔다
        //   (2026-09-25: 아이템마다 굴리던 것을 상자 단위로 바꿈 — 한 번 성공하면 그 안의 모든 아이템을 감지)
        private readonly Dictionary<int, bool> _eliteRevealRolls = new Dictionary<int, bool>();
        private readonly Dictionary<Item, float> _pendingInspect = new Dictionary<Item, float>();
        private readonly HashSet<Item> _inspectingSeen = new HashSet<Item>();
        private readonly HashSet<Item> _knownItems = new HashSet<Item>();
        private readonly List<Item> _finishBuffer = new List<Item>();

        /// <summary>내가 던진 폭발물을 찾아서 거리/대미지/즉시폭발을 적용한다.</summary>
        private void ScanGrenades(float elapsed)
        {
            _grenadeScanTimer -= elapsed;
            if (_grenadeScanTimer > 0f)
            {
                return;
            }

            // 최적화: 폭발물을 쓴 직후 6초 동안만 촘촘히(0.1초) 확인하고,
            // 평소에는 1초에 한 번 확인한다(수류탄 신관이 3초 이상이라 충분히 잡힌다).
            _grenadeScanTimer = Time.time < _grenadeWatchUntil ? 0.1f : 1f;

            if (_skills == null || _main == null)
            {
                return;
            }

            // 0.0.6: 레벨 0 이어도 검사를 수행한다.
            // 예전에는 Lv.0 이면 검사를 건너뛰어서 "첫 투척 경험치를 영영 받을 수 없는" 자기잠금 버그가 있었다.
            int level = _skills.GetLevel("throwing");

            // 수류탄은 레이드에서만 나오므로 기지에서는 검사를 건너뛴다(성능·호환)
            LevelManager raidCheck = LevelManager.Instance;
            if (raidCheck != null && !raidCheck.IsRaidMap)
            {
                return;
            }

            float rangeBonus = _skills.ThrowingRangeBonus(level);
            float damageBonus = _skills.ThrowingDamageBonus(level);
            bool elite = _skills.IsElite("throwing");

            Grenade[] grenades = FindObjectsOfType<Grenade>();
            foreach (Grenade grenade in grenades)
            {
                if (grenade == null)
                {
                    continue;
                }
                int id = grenade.GetInstanceID();
                if (_handledGrenades.Contains(id))
                {
                    continue;
                }

                DamageInfo info = grenade.damageInfo;
                if (info.fromCharacter != _main)
                {
                    continue;   // 내가 던진 폭발물만 대상
                }

                _handledGrenades.Add(id);
                if (_handledGrenades.Count > 256)
                {
                    _handledGrenades.Clear();
                }

                // 경험치: 던질 때마다
                _skills.AddXp("throwing", Rates.ThrowingPerThrow);

                // 폭발 대미지 +%
                if (damageBonus > 0f)
                {
                    DamageInfo modified = info;
                    modified.damageValue *= (1f + damageBonus);
                    grenade.damageInfo = modified;
                }

                // 투척 거리 +% (포물선 사거리는 속도의 제곱에 비례)
                if (rangeBonus > 0f && !grenade.isLandmine)
                {
                    Rigidbody body = grenade.GetComponent<Rigidbody>();
                    if (body != null)
                    {
                        body.velocity *= Mathf.Sqrt(1f + rangeBonus);
                    }
                }

                // 엘리트: 확률로 즉시 폭발
                if (elite && !grenade.isLandmine && UnityEngine.Random.value < Specials.ThrowingInstantChance)
                {
                    grenade.delayFromCollide = false;
                    grenade.delayTime = 0f;
                }
            }
        }

        /// <summary>상자/시체의 아이템 감지 시간을 줄이거나 즉시 감지시킨다.</summary>
        private void ScanLoot(float elapsed)
        {
            if (_skills == null || _main == null)
            {
                return;
            }

            // 예약해 둔 "감지 완료" 처리
            if (_pendingInspect.Count > 0)
            {
                _finishBuffer.Clear();
                foreach (KeyValuePair<Item, float> pair in _pendingInspect)
                {
                    if (pair.Key == null || Time.time >= pair.Value)
                    {
                        _finishBuffer.Add(pair.Key);
                    }
                }
                foreach (Item item in _finishBuffer)
                {
                    _pendingInspect.Remove(item);
                    RevealItem(item);
                }
                _finishBuffer.Clear();
            }

            _lootScanTimer -= elapsed;
            if (_lootScanTimer > 0f)
            {
                return;
            }
            _lootScanTimer = 0.4f;

            // 최적화: 상자(루팅) UI가 열려 있을 때만 확인한다. 평소에는 비용 0.
            if (!(Duckov.UI.View.ActiveView is Duckov.UI.LootView))
            {
                // 창을 닫으면 '이번에 열었을 때의 굴림'을 지운다 → 다음에 열면 새로 굴린다
                if (_eliteRevealRolls.Count > 0)
                {
                    _eliteRevealRolls.Clear();
                }
                return;
            }

            // 0.0.6: 레벨 0 이어도 검사한다(예전에는 Lv.0 이면 건너뛰어 첫 감지 경험치를 못 받았다).
            int level = _skills.GetLevel("looting");

            bool elite = _skills.IsElite("looting");
            float factor = _skills.LootingTimeFactor(level);
            Vector3 myPosition = _main.transform.position;

            InteractableLootbox[] boxes = FindObjectsOfType<InteractableLootbox>();
            foreach (InteractableLootbox box in boxes)
            {
                if (box == null)
                {
                    continue;
                }
                // 가까이에 있는 상자만 확인(내가 열고 있는 상자)
                if ((box.transform.position - myPosition).sqrMagnitude > 36f)
                {
                    continue;
                }

                Inventory inventory = box.Inventory;
                if (inventory == null || inventory.Content == null)
                {
                    continue;
                }

                // 엘리트: **상자 단위로 한 번** 굴려서 성공하면 그 안의 아이템을 모두 즉시 감지
                //   (2026-09-25 사용자 요청: "50% 확률로 열어본 상자·시체의 모든 아이템이 감지")
                bool revealAll = elite && EliteRevealAll(box);

                foreach (Item item in inventory.Content)
                {
                    if (item == null || item.Inspected || !item.Inspecting)
                    {
                        continue;
                    }
                    if (!_inspectingSeen.Add(item))
                    {
                        continue;   // 이미 처리한 아이템
                    }

                    if (revealAll)
                    {
                        RevealItem(item);
                        continue;
                    }

                    float normal = GameplayDataSettings.LootingData.GetInspectingTime(item);
                    // 만렙(-100%)이어도 0초가 되지 않게 최소 시간을 둔다 (2026-09-25 사용자 요청)
                    float wait = Mathf.Max(normal * factor, Specials.LootingMinInspectTime);
                    _pendingInspect[item] = Time.time + wait;
                }
            }

            if (_inspectingSeen.Count > 512)
            {
                _inspectingSeen.Clear();
            }

            // 더 이상 필요 없는 예약(사라졌거나 이미 감지된 아이템)을 정리해 메모리 누수 방지
            if (_pendingInspect.Count > 64)
            {
                _finishBuffer.Clear();
                foreach (KeyValuePair<Item, float> pair in _pendingInspect)
                {
                    if (pair.Key == null || pair.Key.Inspected)
                    {
                        _finishBuffer.Add(pair.Key);
                    }
                }
                foreach (Item stale in _finishBuffer)
                {
                    _pendingInspect.Remove(stale);
                }
                _finishBuffer.Clear();
            }
        }

        /// <summary>엘리트 파밍: 이 상자(시체)를 열 때 성공했는지 — **열 때 한 번만** 굴린다.
        ///  한 번 성공하면 그 상자의 아이템을 모두 감지하므로, 0.4초마다 다시 굴리면 안 된다.</summary>
        private bool EliteRevealAll(InteractableLootbox box)
        {
            int id = box.GetInstanceID();
            bool success;
            if (_eliteRevealRolls.TryGetValue(id, out success))
            {
                return success;
            }

            success = UnityEngine.Random.value < Specials.LootingInstantChance;
            if (_eliteRevealRolls.Count > 64)
            {
                _eliteRevealRolls.Clear();   // raid 한 판에서 캐시가 커지지 않게
            }
            _eliteRevealRolls[id] = success;
            Debug.Log("[Dskill] 파밍 엘리트: 상자 굴림 " + (success ? "성공 — 안의 아이템을 모두 즉시 감지" : "실패"));
            return success;
        }

        /// <summary>아이템을 감지 완료 상태로 만든다(게임이 하는 것과 같은 처리).</summary>
        private void RevealItem(Item item)
        {
            if (item == null || item.Inspected)
            {
                return;
            }
            item.Inspecting = false;
            item.Inspected = true;
            if (_skills != null)
            {
                _skills.AddXp("looting", Rates.LootingPerItem);
            }
        }

        /// <summary>새로 주운 아이템을 확인해서 파밍 경험치를 준다.</summary>
        private void CheckPickups()
        {
            if (_mainItem == null || _skills == null)
            {
                return;
            }

            bool firstTime = _firstTick;
            if (firstTime)
            {
                _knownItems.Clear();
            }

            _itemBuffer.Clear();
            CollectItems(_mainItem, _itemBuffer);

            foreach (Item item in _itemBuffer)
            {
                if (!_knownItems.Add(item) || firstTime)
                {
                    continue;
                }
                if (IsFishItem(item))
                {
                    // 낚시로 얻은 물고기는 '낚시' 스킬 경험치를 준다
                    _skills.AddXp("fishing", Rates.FishingPerCatch);
                }
                else
                {
                    _skills.AddXp("looting", Rates.LootingPerPickup);
                }
            }

            if (_knownItems.Count > _itemBuffer.Count + 64)
            {
                _knownItems.IntersectWith(_itemBuffer);
            }
        }

        // ---- 구르기 ----
        private CA_Dash _dashAction;
        private float _originalDashCoolTime = -1f;
        private float _originalDashStamina = -1f;
        private float _originalDashTime = -1f;    // 구르기 '동작 시간'(0.0.6: 쿨타임과 함께 감소)
        private readonly object _dashSpeedToken = new object();   // 거리 보정용 스탯 토큰
        private Item _dashSpeedItem;
        private int _appliedDashSpeedLevel = -1;

        /// <summary>구르기 거리 보정: 동작 시간을 줄인 만큼 이동 속도를 올려 '이동 거리'를 유지한다.
        ///  (거리 = 속도 × 시간이므로, 시간이 0.64배가 되면 속도를 1/0.64 = 1.56배로 올린다)</summary>
        private void ApplyDashSpeedCompensation(Item characterItem, int level, float factor)
        {
            if (characterItem == null || factor <= 0.05f)
            {
                return;
            }
            if (_dashSpeedItem == characterItem && _appliedDashSpeedLevel == level)
            {
                return;   // 이미 같은 아이템·같은 레벨로 적용됨
            }

            try
            {
                characterItem.RemoveAllModifiersFrom(_dashSpeedToken);
                float bonus = (1f / factor) - 1f;
                if (bonus > 0f)
                {
                    if (characterItem.AddModifier("DashSpeed", new Modifier(ModifierType.PercentageMultiply, bonus, _dashSpeedToken)))
                    {
                        Debug.Log("[Dskill] 구르기 거리 보정 Lv." + level + " : 동작 시간 -" + ((1f - factor) * 100f).ToString("0.#") +
                                  "% → 속도 +" + (bonus * 100f).ToString("0.#") + "% (이동 거리 유지)");
                    }
                    else
                    {
                        Debug.LogWarning("[Dskill] DashSpeed 스탯이 없어 구르기 거리 보정을 건너뜁니다.");
                    }
                }
                _dashSpeedItem = characterItem;
                _appliedDashSpeedLevel = level;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 구르기 거리 보정 실패(무시): " + e.Message);
            }
        }
        // ---- 근접 공격속도 (0.0.8) ----
        // CA_Attack 의 시간 필드(cd·attackActionTime)는 private 이라 리플렉션으로 접근한다.
        // (구르기의 dashTime·coolTime 은 public 이라 직접 접근)
        private CA_Attack _meleeAttackAction;
        private float _originalMeleeCd = -1f;
        private float _originalMeleeActionTime = -1f;
        private float _originalMeleeDealDamage = -1f;
        private int _appliedMeleeSpeedLevel = -1;
        private static readonly System.Reflection.FieldInfo _meleeCdField =
            typeof(CA_Attack).GetField("cd", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        private static readonly System.Reflection.FieldInfo _meleeActionTimeField =
            typeof(CA_Attack).GetField("attackActionTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        private static readonly System.Reflection.FieldInfo _meleeDealDamageField =
            typeof(CA_Attack).GetField("dealDamageTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        /// <summary>근접 전투: 근접 공격 쿨타임(cd)과 동작 시간(attackActionTime)을 같은 비율로 줄인다.</summary>
        private void ApplyMeleeSpeedSettings()
        {
            if (_skills == null || _main == null)
            {
                return;
            }

            if (_meleeAttackAction == null)
            {
                _meleeAttackAction = _main.attackAction;
                if (_meleeAttackAction == null)
                {
                    return;
                }
                float cd = _meleeCdField != null ? (float)_meleeCdField.GetValue(_meleeAttackAction) : -1f;
                float actionTime = _meleeActionTimeField != null ? (float)_meleeActionTimeField.GetValue(_meleeAttackAction) : -1f;
                float dealDamage = _meleeDealDamageField != null ? (float)_meleeDealDamageField.GetValue(_meleeAttackAction) : -1f;
                // 다른 모드가 비정상 값을 넣었을 가능성을 대비해 정상 범위만 원본으로 인정한다
                if (actionTime >= 0.05f && actionTime <= 5f)
                {
                    _originalMeleeActionTime = actionTime;
                }
                if (dealDamage > 0f && dealDamage <= 5f)
                {
                    _originalMeleeDealDamage = dealDamage;
                }
                if (cd > 0f && cd <= 10f)
                {
                    _originalMeleeCd = cd;
                }
                Debug.Log("[Dskill] 근접 액션 확인: 쿨타임 " + cd.ToString("0.###") + " / 동작 " + actionTime.ToString("0.###") +
                          "초 / 피해판정 " + dealDamage.ToString("0.###") + "초");
            }
            // 근접은 게임에서 cd 가 -1(쿨타임 없음)로 오는 경우가 많다 → 동작 시간(attackActionTime)이 핵심.
            // 두 값 모두 쓸 수 없을 때만 건너뛴다.
            if (_originalMeleeActionTime <= 0f && _originalMeleeCd <= 0f)
            {
                return;
            }

            int level = _skills.GetLevel("melee");
            float bonus = _skills.MeleeSpeedBonus(level);           // 0 … 0.5
            if (level == _appliedMeleeSpeedLevel)
            {
                return;
            }

            try
            {
                // ⚠ 동작 시간(attackActionTime)·피해 판정(dealDamageTime)은 건드리지 않는다.
                //    실측 결과 공격 주기를 바꾸지 못하고 애니메이션만 잘랐다(0.0.8 실패).
                //    실제 공격 속도는 '무기'가 읽는 AttackSpeed 스탯으로 만든다.
                ApplyMeleeWeaponSpeed(level, bonus);
                _appliedMeleeSpeedLevel = level;
                Debug.Log("[Dskill] 근접 공격속도 적용 Lv." + level + " : 무기 AttackSpeed +" + (bonus * 100f).ToString("0.#") + "% (동작·피해판정은 원본 유지)");
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 근접 공격속도 적용 실패(무시): " + e.Message);
            }
        }

        /// <summary>장착한 근접 무기에 AttackSpeed 수정자를 건다.
        ///  이 스탯은 캐릭터가 아니라 무기(ItemAgent_MeleeWeapon)가 읽는 값이라 캐릭터에 걸면 거부된다(0.0.8 발견).</summary>
        private void ApplyMeleeWeaponSpeed(int level, float bonus)
        {
            Item weapon = _melee != null ? _melee.Item : null;
            if (weapon == null)
            {
                return;
            }
            try
            {
                weapon.RemoveAllModifiersFrom(_meleeSpeedToken);
                if (bonus <= 0f)
                {
                    return;
                }
                if (weapon.AddModifier("AttackSpeed", new Modifier(ModifierType.PercentageMultiply, bonus, _meleeSpeedToken)))
                {
                    if (!_meleeWeaponStatLogged)
                    {
                        _meleeWeaponStatLogged = true;
                        Debug.Log("[Dskill] 근접 무기 AttackSpeed 수정자 적용 성공: +" + (bonus * 100f).ToString("0.#") + "% (무기 스탯, Lv." + level + ")");
                    }
                }
                else if (!_meleeWeaponStatLogged)
                {
                    _meleeWeaponStatLogged = true;
                    Debug.LogWarning("[Dskill] 근접 무기에서도 AttackSpeed 스탯이 거부되었습니다 — 이 키는 수정할 수 없는 값입니다.");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 근접 무기 공속 적용 실패(무시): " + e.Message);
            }
        }

        private bool _meleeWeaponStatLogged;
        private readonly object _meleeSpeedToken = new object();   // 무기 AttackSpeed 수정자 토큰
        private bool _meleeUseTimeLogged;   // (미사용 메서드용 — 정리 대상)
        private bool _wasDashing;

        /// <summary>[미사용 — 0.0.8 실패] 근접 무기에는 UsageUtilities(사용 시간)가 없어 이 방식은 쓰지 않는다.
        ///  (치료 아이템에는 존재해서 그쪽은 그대로 사용)</summary>
        private void ApplyMeleeWeaponUseTimeUnused(float bonus)
        {
            if (_useTimeField == null || _melee == null)
            {
                return;
            }
            Item weapon = _melee.Item;
            if (weapon == null)
            {
                return;
            }
            UsageUtilities usage = weapon.UsageUtilities;
            if (usage == null)
            {
                if (!_meleeUseTimeLogged)
                {
                    _meleeUseTimeLogged = true;
                    Debug.LogWarning("[Dskill] 근접 무기에 사용 시간(UsageUtilities)이 없어 useTime 방식은 건너뜁니다.");
                }
                return;
            }

            float original;
            if (!_originalUseTime.TryGetValue(weapon.TypeID, out original))
            {
                original = usage.UseTime;
                _originalUseTime[weapon.TypeID] = original;
            }
            float target = Mathf.Max(0.05f, original * (1f - Mathf.Clamp01(bonus)));
            if (!_meleeUseTimeLogged)
            {
                _meleeUseTimeLogged = true;
                Debug.Log("[Dskill] 근접 무기 사용 시간 확인: " + original.ToString("0.###") + "초 → 목표 " + target.ToString("0.###") + "초");
            }
            if (Mathf.Abs(usage.UseTime - target) > 0.001f)
            {
                _useTimeField.SetValue(usage, target);
            }
        }

        private float _loggedDashCool = -1f;      // 쿨타임 로그 중복 방지
        private float _lastDashLogTime = -10f;    // 구르기 간격 실측용

        /// <summary>구르기 스킬: 쿨타임과 스태미나 소모를 줄인다.</summary>
        private void ApplyDashSettings()
        {
            if (_skills == null || _main == null)
            {
                return;
            }

            if (_dashAction == null)
            {
                _dashAction = _main.dashAction;
                if (_dashAction != null)
                {
                    // 새 대시 컴포넌트를 처음 만났을 때 원래 값을 기억해 둔다.
                    // 다른 모드가 비정상 값을 넣었을 가능성을 대비해 정상 범위만 받아들인다.
                    if (_dashAction.coolTime > 0f && _dashAction.coolTime <= 10f)
                    {
                        _originalDashCoolTime = _dashAction.coolTime;
                    }
                    if (_dashAction.staminaCost > 0f && _dashAction.staminaCost <= 200f)
                    {
                        _originalDashStamina = _dashAction.staminaCost;
                    }
                    if (_dashAction.dashTime > 0f && _dashAction.dashTime <= 5f)
                    {
                        _originalDashTime = _dashAction.dashTime;
                    }
                }
            }
            if (_dashAction == null)
            {
                return;
            }

            if (_originalDashCoolTime <= 0f)
            {
                _originalDashCoolTime = _dashAction.coolTime;
            }
            if (_originalDashStamina <= 0f)
            {
                _originalDashStamina = _dashAction.staminaCost;
            }
            if (_originalDashTime <= 0f)
            {
                _originalDashTime = _dashAction.dashTime;
            }

            int level = _skills.GetLevel("dash");
            if (level <= 0)
            {
                // 아직 스킬 레벨이 없으면 원래 값으로 돌려놓는다
                _dashAction.coolTime = _originalDashCoolTime;
                _dashAction.staminaCost = _originalDashStamina;
                _dashAction.dashTime = _originalDashTime;
                return;
            }

            float factor = 1f - _skills.DashReduction(level);
            float timeFactor = 1f - _skills.DashTimeReduction(level);   // 동작 시간은 최대 20%만 줄임
            _dashAction.coolTime = _originalDashCoolTime * factor;
            _dashAction.staminaCost = _originalDashStamina * factor;
            // 동작 시간도 함께 줄인다(애니메이션이 잘리지 않는 범위에서)
            if (_originalDashTime > 0f)
            {
                _dashAction.dashTime = _originalDashTime * timeFactor;
            }
            // 동작 시간이 짧아진 만큼 속도를 올려 이동 거리를 유지한다
            ApplyDashSpeedCompensation(_mainItem, level, timeFactor);

            // 값이 바뀔 때만 로그로 남긴다(밸런스 확인용)
            if (Mathf.Abs(_loggedDashCool - _dashAction.coolTime) > 0.001f)
            {
                _loggedDashCool = _dashAction.coolTime;
                Debug.Log("[Dskill] 구르기 적용 Lv." + level +
                          " : 쿨타임 " + _originalDashCoolTime.ToString("0.##") + " → " + _dashAction.coolTime.ToString("0.##") + "초" +
                          " / 동작 " + _originalDashTime.ToString("0.##") + " → " + _dashAction.dashTime.ToString("0.##") + "초" +
                          " (쿨타임·스태미나 -" + (_skills.DashReduction(level) * 100f).ToString("0.#") +
                          "% / 동작 -" + (_skills.DashTimeReduction(level) * 100f).ToString("0.#") + "%)");
            }
        }

        /// <summary>구르기 사용 감지(매 프레임). 구르면 경험치를 준다.</summary>
        private void DetectDash()
        {
            if (_skills == null || _main == null)
            {
                _wasDashing = false;
                return;
            }

            bool dashing = _main.Dashing;
            if (dashing && !_wasDashing)
            {
                _skills.AddXp("dash", Rates.DashPerUse);

                // 실측: 이전 구르기와의 간격을 로그로 남긴다(쿨타임 확인용)
                float gap = Time.time - _lastDashLogTime;
                Debug.Log("[Dskill] 구르기 사용 감지: 이전 사용과 " + gap.ToString("0.##") + "초 간격");
                _lastDashLogTime = Time.time;
            }
            _wasDashing = dashing;
        }

        /// <summary>모드를 끌 때 구르기 값을 원래대로 돌려놓는다.</summary>
        private void RestoreDash()
        {
            // 거리 보정 스탯 먼저 제거
            try
            {
                if (_dashSpeedItem != null)
                {
                    _dashSpeedItem.RemoveAllModifiersFrom(_dashSpeedToken);
                }
            }
            catch (Exception)
            {
            }
            _dashSpeedItem = null;
            _appliedDashSpeedLevel = -1;

            if (_dashAction == null)
            {
                return;
            }
            if (_originalDashCoolTime > 0f)
            {
                _dashAction.coolTime = _originalDashCoolTime;
            }
            if (_originalDashStamina > 0f)
            {
                _dashAction.staminaCost = _originalDashStamina;
            }
            if (_originalDashTime > 0f)
            {
                _dashAction.dashTime = _originalDashTime;
            }
            _dashAction = null;
        }
    }
}
