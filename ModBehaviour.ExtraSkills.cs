using System;
using System.Collections.Generic;
using ItemStatsSystem;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 낚시 · 야간시야 · 인지/정찰 · 제작 스킬 처리.
    /// </summary>
    public partial class ModBehaviour
    {
        // ---- 인지/정찰 ----
        private float _lastSoundXpTime = -10f;

        // ---- 제작 (급등 방지용 타이머) ----
        private float _lastCraftXpTime = -10f;      // 제작 XP 는 1회 제작당 1번만
        private float _lastFormulaXpTime = -10f;    // 레시피 해금 XP 는 한꺼번에 풀려도 1회만

        /// <summary>소리를 감지할 때마다 인지/정찰 경험치 (1초에 최대 1회).</summary>
        private void HandlePlayerHearSound(AISound sound)
        {
            if (_skills == null)
            {
                return;
            }
            // 2026-09-26 사용자 요청 B안: 캡 1초 → 0.5초 (소리 1회당 점수는 Rates.PerceptionPerSound = 12)
            if (Time.time - _lastSoundXpTime < 0.5f)
            {
                return;
            }
            _lastSoundXpTime = Time.time;
            _skills.AddXp("perception", Rates.PerceptionPerSound);
        }

        /// <summary>밤 시간에 머무는 동안 야간시야 경험치(레이드에서만).</summary>
        private void DetectNightTime(float elapsed)
        {
            if (_skills == null)
            {
                return;
            }
            LevelManager manager = LevelManager.Instance;
            if (manager == null || !manager.IsRaidMap)
            {
                return;   // 기지에서는 쌓이지 않게 함(밤에 가만히 있어도 오르는 것 방지)
            }
            TimeOfDayController timeOfDay = TimeOfDayController.Instance;
            if (timeOfDay == null || !timeOfDay.AtNight)
            {
                return;
            }
            _skills.AddXp("nightvision", elapsed * Rates.NightVisionPerSecond);
        }

        /// <summary>제작 완료: 재료 가치에 비례한 경험치 + 확률로 결과물 1개 추가 지급.
        ///  주의: 게임은 **결과물 1개마다** 이 이벤트를 호출한다 → 그대로 주면 배수로 오른다(1회 제작당 1번만 지급).</summary>
        private void HandleItemCrafted(CraftingFormula formula, Item result)
        {
            if (_skills == null || result == null)
            {
                return;
            }

            // 같은 제작에서 나온 추가 결과물은 경험치를 주지 않는다
            float now = Time.time;
            bool firstOfThisCraft = now - _lastCraftXpTime >= 0.5f;
            _lastCraftXpTime = now;

            if (firstOfThisCraft)
            {
                float xp = CraftXpFor(formula);
                _skills.AddXp("crafting", xp);
            }

            float chance = _skills.CraftingBonusChance(_skills.GetLevel("crafting"));
            if (chance <= 0f || UnityEngine.Random.value >= chance)
            {
                return;
            }

            try
            {
                Item extra = ItemAssetsCollection.InstantiateSync(result.TypeID);
                if (extra != null)
                {
                    ItemUtilities.SendToPlayer(extra);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 제작 추가 생산 실패: " + e.Message);
            }
        }

        /// <summary>제작 1회 경험치 = 재료 가치 × craft_xp_per_value (상한 craft_xp_cap).
        ///  재료 가치를 못 구하면 기존 고정값으로 대체한다. 계산 결과는 로그에 남겨 조정에 쓸 수 있게 한다.</summary>
        private float CraftXpFor(CraftingFormula formula)
        {
            float xp;
            long value = 0L;
            bool known = false;

            try
            {
                // CraftingFormula 는 구조체라 null 비교를 할 수 없다. 비어 있는지로 판단한다.
                if (formula.cost.money > 0L || (formula.cost.items != null && formula.cost.items.Length > 0))
                {
                    value = MaterialValue(formula.cost);
                    known = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 재료 가치 계산 실패(고정값 사용): " + e.Message);
            }

            if (known && _config != null && _config.CraftXpPerValue > 0f)
            {
                xp = value * _config.CraftXpPerValue;
                if (_config.CraftXpCap > 0f && xp > _config.CraftXpCap)
                {
                    xp = _config.CraftXpCap;
                }
            }
            else
            {
                xp = Rates.CraftingPerCraft;
                value = -1L;
            }

            Debug.Log("[Dskill] 제작 경험치 " + xp.ToString("0.#") +
                      (value >= 0L ? (" (재료 가치 " + value + " × " + _config.CraftXpPerValue.ToString("0.###") + ")") : " (고정값)"));
            return xp;
        }

        /// <summary>제작에 들어간 재료의 가치 합계 = 지불한 돈 + (아이템 단가 × 개수).</summary>
        private static long MaterialValue(Duckov.Economy.Cost cost)
        {
            long total = cost.money;
            if (cost.items != null)
            {
                for (int i = 0; i < cost.items.Length; i++)
                {
                    ItemMetaData meta = ItemAssetsCollection.GetMetaData(cost.items[i].id);
                    total += (long)meta.priceEach * cost.items[i].amount;
                }
            }
            return total;
        }

        /// <summary>새 레시피를 해금했을 때.
        ///  주의: 작업대 업그레이드처럼 여러 레시피가 한꺼번에 풀리면 이벤트가 여러 번 온다
        ///  → 예전에는 레시피 1개당 400점이라 12개가 풀리면 4,800점이 한 번에 올랐다.
        ///  지금은 짧은 시간에 몰린 해금을 1회로 인정한다.</summary>
        private void HandleFormulaUnlocked(string formulaId)
        {
            if (_skills == null)
            {
                return;
            }

            float now = Time.time;
            if (now - _lastFormulaXpTime < 1.5f)
            {
                return;   // 같은 업그레이드/건설로 인한 대량 해금
            }
            _lastFormulaXpTime = now;

            float xp = _config != null && _config.CraftUnlockXp > 0f ? _config.CraftUnlockXp : Rates.CraftingPerUnlock;
            _skills.AddXp("crafting", xp);
        }

        /// <summary>물고기 아이템인지 확인 (낚시 경험치용).</summary>
        private static bool IsFishItem(Item item)
        {
            if (item == null || item.Tags == null)
            {
                return false;
            }
            return item.Tags.Contains("Fish");
        }
    }
}
