using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
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

                    // 엘리트: 확률로 즉시 감지
                    if (elite && UnityEngine.Random.value < Specials.LootingInstantChance)
                    {
                        RevealItem(item);
                        continue;
                    }

                    float normal = GameplayDataSettings.LootingData.GetInspectingTime(item);
                    _pendingInspect[item] = Time.time + normal * factor;
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
        private bool _wasDashing;

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

            int level = _skills.GetLevel("dash");
            if (level <= 0)
            {
                // 아직 스킬 레벨이 없으면 원래 값으로 돌려놓는다
                _dashAction.coolTime = _originalDashCoolTime;
                _dashAction.staminaCost = _originalDashStamina;
                return;
            }

            float factor = 1f - _skills.DashReduction(level);
            _dashAction.coolTime = _originalDashCoolTime * factor;
            _dashAction.staminaCost = _originalDashStamina * factor;
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
            }
            _wasDashing = dashing;
        }

        /// <summary>모드를 끌 때 구르기 값을 원래대로 돌려놓는다.</summary>
        private void RestoreDash()
        {
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
            _dashAction = null;
        }
    }
}
