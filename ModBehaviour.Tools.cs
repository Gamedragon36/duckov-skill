using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

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

                // 엘리트: **지면에 닿으면 100% 즉시 폭발** (2026-09-26 사용자 요청 — 기존 '30% 확률 즉시 폭발' 폐지)
                //   delayFromCollide = true → 충돌(지면) 시점부터 신관이 돌고,
                //   delayTime = 0 → 닿는 즉시 터진다.
                if (elite && !grenade.isLandmine)
                {
                    grenade.delayFromCollide = true;
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

                // 지금 '내가 열고 있는' 상자인지 확인한다.
                //   게임은 내가 연 상자의 아이템만 감지(Inspecting)를 시작하므로 이 조건이 곧 판별이 된다.
                //   0.0.8 문제: 6m 안의 모든 상자에 굴림을 써버려 확률이 낭비되고, 캐시가 차서 굴림이 초기화되며
                //   (성공 → 실패로 다시 굴려짐) 게임이 감지를 시작하지 않은 아이템은 건너뛰었다
                //   → "50% 확률로 열었는데 절반만 즉시 감지"처럼 보였다.
                if (!HasInspectingItem(inventory))
                {
                    continue;
                }

                // 엘리트: **상자 단위로 한 번** 굴려서 성공하면 그 안의 아이템을 모두 즉시 감지
                //   (2026-09-25 사용자 요청: "50% 확률로 열어본 상자·시체의 모든 아이템이 감지")
                bool revealAll = elite && EliteRevealAll(box);
                int revealedNow = 0;

                foreach (Item item in inventory.Content)
                {
                    if (item == null || item.Inspected)
                    {
                        continue;
                    }

                    if (revealAll)
                    {
                        // 성공한 상자는 **아직 감지를 시작하지 않은 아이템까지 전부** 감지한다.
                        //   (게임이 아이템을 하나씩 감지하기 시작해도 나머지가 밀리지 않게)
                        _inspectingSeen.Add(item);
                        RevealItem(item);
                        revealedNow++;
                        continue;
                    }

                    if (!item.Inspecting)
                    {
                        continue;   // 게임이 아직 감지를 시작하지 않은 아이템은 다음 스캔에서 처리
                    }
                    if (!_inspectingSeen.Add(item))
                    {
                        continue;   // 이미 처리한 아이템
                    }

                    float normal = GameplayDataSettings.LootingData.GetInspectingTime(item);
                    // 만렙(-100%)이어도 0초가 되지 않게 최소 시간을 둔다 (2026-09-25 사용자 요청)
                    float wait = Mathf.Max(normal * factor, Specials.LootingMinInspectTime);
                    _pendingInspect[item] = Time.time + wait;
                }

                if (revealedNow > 0)
                {
                    Debug.Log("[Dskill] 파밍 엘리트: 상자 1개에서 " + revealedNow + "개를 즉시 감지했습니다.");
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

        /// <summary>이 상자에 '감지가 시작된' 아이템이 있는지 확인한다.
        ///  게임은 **내가 열고 있는 상자**의 아이템만 감지를 시작하므로,
        ///  이 조건이 곧 '지금 열고 있는 상자' 판별이 된다(0.0.8: 6m 안 모든 상자에 굴림을 낭비하던 문제 수정).</summary>
        private static bool HasInspectingItem(Inventory inventory)
        {
            foreach (Item item in inventory.Content)
            {
                if (item != null && !item.Inspected && item.Inspecting)
                {
                    return true;
                }
            }
            return false;
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
            // 캐시는 '상자 UI가 열려 있는 동안'만 유지된다(닫으면 위에서 비운다).
            // 굴림 대상도 실제로 연 상자로 한정했으므로 이 상한은 안전장치일 뿐이다.
            //   (0.0.8: 64 였고 정상 플레이에서도 자주 차서 **전체 초기화** → 열던 상자가 다시 굴려져
            //    성공이 실패로 바뀌었다 → '절반만 즉시 감지' 원인 중 하나)
            if (_eliteRevealRolls.Count > 256)
            {
                _eliteRevealRolls.Clear();
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

            // 홀드 자동 반복의 '휘두름 간격' = 동작 시간 기준(없으면 0.6초), 하한 0.2초.
            //  (조금 일찍 넣어도 게임이 무시하고 준비되면 휘두르므로 실질 속도는 게임이 정한다)
            float swing = _originalMeleeActionTime > 0f ? _originalMeleeActionTime : 0.6f;
            _meleeHoldInterval = Mathf.Max(0.2f, swing * 0.8f);

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

        // ---- 사격술: 조준(ADS) 시간 감소 (2026-09-26 사용자 요청) ----
        //  조준 시간은 **총기(무기)가 읽는 스탯 `AdsTime`** 이라 캐릭터가 아니라 장착한 총에 건다
        //  (근접 AttackSpeed 와 같은 방식 — 캐릭터에 걸면 거부된다).
        private readonly object _adsTimeToken = new object();
        private Item _adsTimeItem;
        private int _appliedAdsTimeLevel = -1;
        private bool _adsTimeStatLogged;

        /// <summary>사격술: 장착한 총의 조준 시간(AdsTime)을 레벨에 따라 줄인다 (만렙 -50%).</summary>
        private void ApplyAssaultAdsTime()
        {
            if (_skills == null)
            {
                return;
            }

            Item gun = _gun != null ? _gun.Item : null;
            int level = _skills.GetLevel("assault");
            float reduction = level > 0 ? Specials.AssaultAdsTimePerLevel * level : 0f;

            if (gun == _adsTimeItem && level == _appliedAdsTimeLevel)
            {
                return;   // 이미 같은 총·같은 레벨로 처리됨
            }

            try
            {
                // 총을 바꿨으면 이전 총에서 제거
                if (_adsTimeItem != null && _adsTimeItem != gun)
                {
                    _adsTimeItem.RemoveAllModifiersFrom(_adsTimeToken);
                }
                _adsTimeItem = gun;
                _appliedAdsTimeLevel = level;

                if (gun == null)
                {
                    return;
                }
                gun.RemoveAllModifiersFrom(_adsTimeToken);
                if (reduction <= 0f)
                {
                    return;
                }

                if (gun.AddModifier("AdsTime", new Modifier(ModifierType.PercentageMultiply, -reduction, _adsTimeToken)))
                {
                    if (!_adsTimeStatLogged)
                    {
                        _adsTimeStatLogged = true;
                        Debug.Log("[Dskill] 사격술: 총기 AdsTime(조준 시간) -" + (reduction * 100f).ToString("0.#") +
                                  "% 적용 성공 (무기 스탯, Lv." + level + ")");
                    }
                }
                else if (!_adsTimeStatLogged)
                {
                    _adsTimeStatLogged = true;
                    Debug.LogWarning("[Dskill] 총기에서 AdsTime 스탯이 거부되었습니다 — 조준 시간 감소가 적용되지 않습니다(다른 방식 필요).");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 사격술 조준 시간 적용 실패(무시): " + e.Message);
            }
        }
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
                return;
            }

            float original;
            if (!_originalUseTime.TryGetValue(weapon.TypeID, out original))
            {
                original = usage.UseTime;
                _originalUseTime[weapon.TypeID] = original;
            }
            float target = Mathf.Max(0.05f, original * (1f - Mathf.Clamp01(bonus)));
            if (Mathf.Abs(usage.UseTime - target) > 0.001f)
            {
                _useTimeField.SetValue(usage, target);
            }
        }

        private float _loggedDashCool = -1f;      // 쿨타임 로그 중복 방지
        private float _loggedDashStamina = -1f;   // 스태미나 로그 중복 방지(지구력 중첩 확인용)
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
            int enduranceLevel = _skills.GetLevel("endurance");

            // 0.0.9: 지구력 스킬의 '스태미나 소모 -%' 를 구르기 스태미나에도 **중첩** 적용한다.
            //   게임은 달리기 스태미나(스탯 StaminaDrainRate)와 구르기 스태미나(CA_Dash.staminaCost)를
            //   따로 계산해서, 예전에는 지구력 감소가 구르기에 전혀 들어가지 않았다
            //   (2026-09-26 사용자 요청으로 중첩 적용). 스킬 레벨이 0이어도 원래 값으로 되돌아온다.
            float enduranceFactor = 1f - _skills.EnduranceStaminaReduction(enduranceLevel);
            float factor = level > 0 ? 1f - _skills.DashReduction(level) : 1f;
            float timeFactor = level > 0 ? 1f - _skills.DashTimeReduction(level) : 1f;   // 동작 시간은 최대 20%만 줄임

            _dashAction.coolTime = _originalDashCoolTime * factor;
            // 스태미나 = 원래값 × 구르기 감소 × 지구력 감소 (두 스킬 중첩)
            _dashAction.staminaCost = _originalDashStamina * factor * enduranceFactor;
            // 동작 시간도 함께 줄인다(애니메이션이 잘리지 않는 범위에서)
            if (_originalDashTime > 0f)
            {
                _dashAction.dashTime = _originalDashTime * timeFactor;
            }
            // 동작 시간이 짧아진 만큼 속도를 올려 이동 거리를 유지한다
            //  (레벨 0 일 때도 호출해 예전에 붙어 있던 거리 보정을 확실히 제거한다 — factor 가 1 이라 아무 값도 더하지 않는다)
            ApplyDashSpeedCompensation(_mainItem, level, timeFactor);

            // 값이 바뀔 때만 로그로 남긴다(밸런스 확인용)
            if (Mathf.Abs(_loggedDashCool - _dashAction.coolTime) > 0.001f ||
                Mathf.Abs(_loggedDashStamina - _dashAction.staminaCost) > 0.001f)
            {
                _loggedDashCool = _dashAction.coolTime;
                _loggedDashStamina = _dashAction.staminaCost;
                float staminaCut = _originalDashStamina > 0f
                    ? (1f - (_dashAction.staminaCost / _originalDashStamina)) * 100f
                    : 0f;
                Debug.Log("[Dskill] 구르기 적용 (구르기 Lv." + level + " / 지구력 Lv." + enduranceLevel + ")" +
                          " : 쿨타임 " + _originalDashCoolTime.ToString("0.##") + " → " + _dashAction.coolTime.ToString("0.##") + "초" +
                          " / 스태미나 " + _originalDashStamina.ToString("0.##") + " → " + _dashAction.staminaCost.ToString("0.##") +
                          " (총 -" + staminaCut.ToString("0.#") + "% = 구르기 -" + (_skills.DashReduction(level) * 100f).ToString("0.#") +
                          "% × 지구력 -" + (_skills.EnduranceStaminaReduction(enduranceLevel) * 100f).ToString("0.#") + "%)");
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
                _lastDashStartTime = Time.time;

                // 실측: 이전 구르기와의 간격을 로그로 남긴다(쿨타임 확인용)
                float gap = Time.time - _lastDashLogTime;
                Debug.Log("[Dskill] 구르기 사용 감지: 이전 사용과 " + gap.ToString("0.##") + "초 간격");
                _lastDashLogTime = Time.time;
            }
            _wasDashing = dashing;
        }

        // ------------------------------------------------------------------
        // 구르기 꾹 누르기 → 자동 반복 (0.0.9 · 2026-09-26 사용자 요청)
        // ------------------------------------------------------------------
        //  방식(A안): 게임의 입력 파이프라인을 그대로 이용한다.
        //   스페이스바를 누르고 있고 ① 구르기 중이 아니며 ② 쿨타임이 지났을 때만
        //   "떼기 → (다음 프레임) 누르기" 입력을 주입해, 게임이 평소처럼 구르기를 실행하게 한다.
        //   → 쿨타임·스태미나 검사는 **게임이 그대로** 수행하므로 규칙을 우회하지 않는다.
        private float _lastDashStartTime = -10f;    // 마지막 구르기 시작 시각(쿨타임 게이트용)
        private int _dashInjectPhase;               // 0 = 대기, 1 = '떼기' 주입됨(다음 프레임에 '누르기')
        private float _lastDashInjectTime = -10f;   // 주입 최소 간격
        private readonly List<Key> _dashKeyBuffer = new List<Key>();

        /// <summary>구르기 키를 꾹 누르고 있으면 쿨타임마다 자동으로 다시 구른다 (매 프레임).</summary>
        private void HandleDashHold()
        {
            if (_config == null || !_config.DashHoldRepeat)
            {
                _dashInjectPhase = 0;
                return;
            }
            if (_main == null || _main.dashAction == null)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            // '떼기'를 이미 넣었으면 다음 프레임에 '누르기'를 넣어 한 번의 누름을 완성한다.
            //  (누르기 완료는 무조건 실행해야 한다 — 중간에 취소하면 스페이스가 떼진 상태로 남는다)
            if (_dashInjectPhase == 1)
            {
                PushKeyboardState(keyboard, true);
                _dashInjectPhase = 0;
                _lastDashInjectTime = Time.time;
                return;
            }

            // 아직 안 눌렀거나 구르기 동작 중이면 아무것도 하지 않는다
            if (!keyboard.spaceKey.isPressed || _main.Dashing)
            {
                return;
            }

            // UI(인벤토리·루팅·메뉴)가 열려 있으면 주입하지 않는다 — 메뉴 조작이 꼬이는 것을 막는다
            if (Duckov.UI.View.ActiveView != null)
            {
                _dashInjectPhase = 0;
                return;
            }

            // 이번 세션에서 구르기를 한 번도 못 봤다면 아무것도 하지 않는다.
            //  (구르기 키를 스페이스바가 아닌 다른 키로 바꾼 경우 오작동을 막는 안전장치)
            if (_lastDashStartTime <= 0f)
            {
                return;
            }

            float cool = _main.dashAction.coolTime;
            if (cool < 0.1f)
            {
                cool = 0.1f;
            }
            if (Time.time - _lastDashStartTime < cool)
            {
                return;   // 쿨타임 남음 (게임이 어차피 무시하므로 입력 주입을 아낀다)
            }
            if (Time.time - _lastDashInjectTime < 0.1f)
            {
                return;   // 연속 주입 방지
            }

            PushKeyboardState(keyboard, false);   // 스페이스만 뗀 상태를 주입
            _dashInjectPhase = 1;
        }

        /// <summary>지금 눌려 있는 다른 키는 유지한 채, 스페이스만 뗀 상태/누른 상태를 입력 시스템에 넣는다.
        ///  (입력을 '주입'하므로 게임의 구르기 입력 처리·쿨타임·스태미나 검사가 평소와 동일하게 동작한다)</summary>
        private void PushKeyboardState(Keyboard keyboard, bool spacePressed)
        {
            try
            {
                _dashKeyBuffer.Clear();
                var keys = keyboard.allKeys;
                for (int i = 0; i < keys.Count; i++)
                {
                    KeyControl control = keys[i];
                    if (control == null || !control.isPressed || control.keyCode == Key.Space)
                    {
                        continue;
                    }
                    _dashKeyBuffer.Add(control.keyCode);   // 이동키 등은 그대로 유지
                }
                if (spacePressed)
                {
                    _dashKeyBuffer.Add(Key.Space);
                }
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(_dashKeyBuffer.ToArray()));
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 구르기 홀드 입력 주입 실패(무시): " + e.Message);
                _dashInjectPhase = 0;
            }
        }

        // ------------------------------------------------------------------
        // 근접 공격 꾹 누르기 → 자동 반복 (2026-09-26 사용자 요청)
        // ------------------------------------------------------------------
        //  구르기와 같은 방식(A안): 게임 입력 파이프라인에 "떼기 → (다음 프레임) 누르기"를 주입한다.
        //  대상은 마우스 왼쪽 버튼(게임의 '발사/공격' 입력)이고, **근접 무기를 들었을 때만** 동작한다
        //  (총을 들고 있으면 절대 주입하지 않아 자동 사격이 되지 않는다).
        private int _meleeHoldPhase;                  // 0 = 대기, 1 = '떼기' 주입됨
        private float _lastMeleeInjectTime = -10f;    // 주입 최소 간격
        private float _meleeHoldInterval = 0.5f;      // 휘두름 간격(동작 시간 기준, ApplyMeleeSpeedSettings 가 갱신)

        /// <summary>근접 무기를 들고 공격 버튼을 꾹 누르면 계속 휘두른다 (매 프레임).</summary>
        private void HandleMeleeHold()
        {
            if (_config == null || !_config.MeleeHoldRepeat)
            {
                _meleeHoldPhase = 0;
                return;
            }

            if (_main == null)
            {
                _meleeHoldPhase = 0;
                return;
            }

            // **지금 손에 든 것**이 근접 무기일 때만 동작한다.
            //  ⚠ `_melee`(GetMeleeWeapon) 는 '슬롯에 있기만 하면' 반환되어,
            //    총·아이템을 쓸 때도 주입이 일어나 **모든 좌클릭이 연속 클릭이 되는 버그**가 있었다(2026-09-26 수정).
            var heldItem = _main.CurrentHoldItemAgent;
            if (!(heldItem is ItemAgent_MeleeWeapon) || heldItem.Item == null)
            {
                _meleeHoldPhase = 0;
                return;
            }

            // UI(인벤토리·루팅·메뉴)가 열려 있으면 주입하지 않는다 — UI 클릭이 연속 클릭이 되는 것을 막는다
            if (Duckov.UI.View.ActiveView != null)
            {
                _meleeHoldPhase = 0;
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            // '떼기'를 이미 넣었으면 다음 프레임에 '누르기'를 넣어 한 번의 누름을 완성한다
            //  (중간에 취소하면 왼쪽 버튼이 떼진 상태로 남는다)
            if (_meleeHoldPhase == 1)
            {
                PushMouseState(mouse, true);
                _meleeHoldPhase = 0;
                _lastMeleeInjectTime = Time.time;
                return;
            }

            if (!mouse.leftButton.isPressed)
            {
                return;   // 공격 버튼을 누르고 있지 않음
            }
            if (Time.time - _lastMeleeInjectTime < _meleeHoldInterval)
            {
                return;   // 아직 휘두름 동작 중일 가능성이 큼 (게임이 어차피 무시하므로 주입을 아낀다)
            }

            PushMouseState(mouse, false);
            _meleeHoldPhase = 1;
        }

        /// <summary>마우스의 위치·델타·스크롤·다른 버튼은 그대로 두고, **왼쪽 버튼만** 뗀/누른 상태를 입력 시스템에 넣는다.
        ///  (위치·델타를 유지해야 시점(카메라)에 영향이 없다)</summary>
        private void PushMouseState(Mouse mouse, bool leftPressed)
        {
            try
            {
                MouseState state = new MouseState
                {
                    position = mouse.position.ReadValue(),
                    delta = mouse.delta.ReadValue(),
                    scroll = mouse.scroll.ReadValue(),
                };

                ushort buttons = 0;
                if (leftPressed)
                {
                    buttons |= (ushort)(1 << (int)MouseButton.Left);
                }
                if (mouse.rightButton.isPressed)
                {
                    buttons |= (ushort)(1 << (int)MouseButton.Right);
                }
                if (mouse.middleButton.isPressed)
                {
                    buttons |= (ushort)(1 << (int)MouseButton.Middle);
                }
                state.buttons = buttons;

                InputSystem.QueueStateEvent(mouse, state);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 근접 홀드 입력 주입 실패(무시): " + e.Message);
                _meleeHoldPhase = 0;
            }
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
