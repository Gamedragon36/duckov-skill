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

        /// <summary>소리를 감지할 때마다 인지/정찰 경험치 (1초에 최대 1회).</summary>
        private void HandlePlayerHearSound(AISound sound)
        {
            if (_skills == null)
            {
                return;
            }
            if (Time.time - _lastSoundXpTime < 1f)
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

        /// <summary>제작 완료: 경험치 + 확률로 결과물 1개 추가 지급.</summary>
        private void HandleItemCrafted(CraftingFormula formula, Item result)
        {
            if (_skills == null || result == null)
            {
                return;
            }
            _skills.AddXp("crafting", Rates.CraftingPerCraft);

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

        /// <summary>새 레시피를 해금했을 때.</summary>
        private void HandleFormulaUnlocked(string formulaId)
        {
            if (_skills == null)
            {
                return;
            }
            _skills.AddXp("crafting", Rates.CraftingPerUnlock);
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
