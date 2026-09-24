using System;
using System.Collections.Generic;
using System.Reflection;
using Duckov.Bitcoins;
using Duckov.Economy;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 하이드아웃 스킬: 기지 체류 경험치 + 상인 쿨타임/비트코인 채굴 시간 조절.
    /// 상인 쿨타임과 채굴 시간은 게임 내부의 private 값이라 리플렉션으로 읽고 쓴다.
    /// (게임 업데이트로 필드 이름이 바뀌면 그 효과만 조용히 빠지고 경고 로그가 남는다)
    /// </summary>
    public partial class ModBehaviour
    {
        private static FieldInfo _shopRefreshField;
        private static FieldInfo _minerWorkField;
        private static bool _reflectionWarned;

        private readonly Dictionary<StockShop, long> _shopOriginalRefresh = new Dictionary<StockShop, long>();
        private float _hideoutApplyTimer;
        private double _minerOriginalWorkPerCoin = -1.0;

        /// <summary>기지에 머무는 동안 경험치를 준다.</summary>
        private void DetectHideoutTime(float elapsed)
        {
            if (_skills == null)
            {
                return;
            }
            LevelManager manager = LevelManager.Instance;
            if (manager == null || !manager.IsBaseLevel)
            {
                return;
            }
            _skills.AddXp("hideout", elapsed * Rates.HideoutPerSecond);
        }

        /// <summary>하이드아웃 효과 적용(5초에 한 번).</summary>
        private void ApplyHideoutEffects(float elapsed)
        {
            if (_skills == null)
            {
                return;
            }

            _hideoutApplyTimer -= elapsed;
            if (_hideoutApplyTimer > 0f)
            {
                return;
            }
            _hideoutApplyTimer = 5f;

            // 상인은 기지에만 있으므로 기지에서만 검색한다(레이드에서 불필요한 전체 검색 방지)
            LevelManager manager = LevelManager.Instance;
            if (manager != null && manager.IsBaseLevel)
            {
                ApplyMerchantCooldown();
            }
            ApplyMinerTime();
        }

        /// <summary>상인(상점) 재고 갱신 쿨타임을 줄인다.</summary>
        private void ApplyMerchantCooldown()
        {
            if (_shopRefreshField == null)
            {
                _shopRefreshField = typeof(StockShop).GetField("refreshAfterTimeSpan",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (_shopRefreshField == null)
                {
                    WarnReflection("상인 쿨타임");
                    return;
                }
            }

            int level = _skills.GetLevel("hideout");
            float factor = 1f - _skills.MerchantCooldownReduction(level);

            StockShop[] shops = FindObjectsOfType<StockShop>();
            foreach (StockShop shop in shops)
            {
                if (shop == null)
                {
                    continue;
                }

                long original;
                if (!_shopOriginalRefresh.TryGetValue(shop, out original))
                {
                    try
                    {
                        original = (long)_shopRefreshField.GetValue(shop);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    // 다른 모드가 이미 값을 바꿔 놓았을 가능성을 고려한 안전 범위 검사
                    if (original <= 0L || original > TimeSpan.TicksPerDay * 30L)
                    {
                        continue;
                    }
                    _shopOriginalRefresh[shop] = original;
                }

                long value = (long)(original * factor);
                if (value < 0L)
                {
                    value = 0L;
                }
                try
                {
                    _shopRefreshField.SetValue(shop, value);
                }
                catch (Exception)
                {
                    // 개별 상점 실패는 무시
                }
            }

            if (_shopOriginalRefresh.Count > 64)
            {
                _shopOriginalRefresh.Clear();   // 오래된 기록 정리(다음에 원래 값으로 다시 읽음)
            }
        }

        /// <summary>비트코인 채굴기 생산 시간을 줄인다(엘리트 전용).</summary>
        private void ApplyMinerTime()
        {
            if (_minerWorkField == null)
            {
                _minerWorkField = typeof(BitcoinMiner).GetField("workPerCoin",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (_minerWorkField == null)
                {
                    WarnReflection("비트코인 채굴 시간");
                    return;
                }
            }

            BitcoinMiner miner = BitcoinMiner.Instance;
            if (miner == null)
            {
                return;
            }

            if (_minerOriginalWorkPerCoin <= 0.0)
            {
                try
                {
                    _minerOriginalWorkPerCoin = (double)_minerWorkField.GetValue(miner);
                }
                catch (Exception)
                {
                    WarnReflection("비트코인 채굴 시간");
                    return;
                }
            }
            if (_minerOriginalWorkPerCoin <= 0.0)
            {
                return;
            }

            float reduction = _skills.MinerTimeReduction(_skills.GetLevel("hideout"));
            try
            {
                _minerWorkField.SetValue(miner, _minerOriginalWorkPerCoin * (1.0 - reduction));
            }
            catch (Exception)
            {
                // 무시
            }
        }

        /// <summary>모드를 끌 때 상인 쿨타임·채굴 시간을 원래대로 되돌린다.</summary>
        private void RestoreHideoutEffects()
        {
            if (_minerWorkField != null && _minerOriginalWorkPerCoin > 0.0)
            {
                BitcoinMiner miner = BitcoinMiner.Instance;
                if (miner != null)
                {
                    try
                    {
                        _minerWorkField.SetValue(miner, _minerOriginalWorkPerCoin);
                    }
                    catch (Exception)
                    {
                        // 무시
                    }
                }
            }

            if (_shopRefreshField != null && _shopOriginalRefresh.Count > 0)
            {
                foreach (KeyValuePair<StockShop, long> pair in _shopOriginalRefresh)
                {
                    if (pair.Key == null)
                    {
                        continue;
                    }
                    try
                    {
                        _shopRefreshField.SetValue(pair.Key, pair.Value);
                    }
                    catch (Exception)
                    {
                        // 무시
                    }
                }
            }
            _shopOriginalRefresh.Clear();
        }

        private static void WarnReflection(string what)
        {
            if (_reflectionWarned)
            {
                return;
            }
            _reflectionWarned = true;
            Debug.LogWarning("[Dskill] " + what + " 값을 찾지 못했습니다(게임 버전 차이 가능). 그 효과만 적용되지 않습니다.");
        }
    }
}
