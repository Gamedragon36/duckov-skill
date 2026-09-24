using System;
using System.IO;
using Saves;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 로그라이크(계승) 시스템.
    /// '다른 세이브 슬롯'에서 키운 스킬 레벨 합계가
    /// 지금 플레이 중인 세이브의 경험치 획득 속도를 올려준다.
    /// </summary>
    public class MetaProgress
    {
        private const int MaxSlotsToScan = 12;      // 1~12번 슬롯 확인 (없는 슬롯은 자동 건너뜀)
        private const float RescanInterval = 300f;  // 다시 계산하는 최소 간격(초) — 파일 읽기 부담 최소화

        private readonly Config _config;

        private int _otherTotalLevel;
        private int _scannedSlots;
        private float _lastScanTime = -9999f;
        private bool _scanned;

        public MetaProgress(Config config)
        {
            _config = config;
        }

        /// <summary>다른 세이브들의 스킬 레벨 합계</summary>
        public int OtherTotalLevel => _otherTotalLevel;

        /// <summary>계산에 포함된 다른 세이브 개수</summary>
        public int ScannedSlots => _scannedSlots;

        /// <summary>경험치 보너스 비율 (0.6 = +60%)</summary>
        public float BonusRatio
        {
            get
            {
                if (_config == null || !_config.MetaEnabled)
                {
                    return 0f;
                }
                float ratio = _otherTotalLevel * (_config.MetaBonusPercent / 100f);
                return Mathf.Clamp(ratio, 0f, _config.MetaBonusCapPercent / 100f);
            }
        }

        /// <summary>경험치 배율 (1.0 = 보너스 없음)</summary>
        public float XpMultiplier => 1f + BonusRatio;

        /// <summary>계승 등급 (0 = 없음, 6 = 최고)</summary>
        public int Grade
        {
            get
            {
                int lv = _otherTotalLevel;
                if (lv >= 600) return 6;
                if (lv >= 400) return 5;
                if (lv >= 250) return 4;
                if (lv >= 130) return 3;
                if (lv >= 50) return 2;
                if (lv >= 1) return 1;
                return 0;
            }
        }

        /// <summary>등급 이름</summary>
        public string GradeName
        {
            get
            {
                switch (Grade)
                {
                    case 6: return "전설";
                    case 5: return "영웅";
                    case 4: return "숙련";
                    case 3: return "단련";
                    case 2: return "초심";
                    case 1: return "입문";
                    default: return "없음";
                }
            }
        }

        /// <summary>다른 세이브들의 기록을 다시 계산한다(60초에 한 번만 실제로 계산).</summary>
        public void Rescan(bool force = false)
        {
            if (!force && Time.realtimeSinceStartup - _lastScanTime < RescanInterval)
            {
                return;
            }
            _lastScanTime = Time.realtimeSinceStartup;
            Scan();
        }

        private void Scan()
        {
            _otherTotalLevel = 0;
            _scannedSlots = 0;
            _scanned = true;

            if (_config == null || !_config.MetaEnabled)
            {
                return;
            }

            int current = SavesSystem.CurrentSlot;

            for (int slot = 1; slot <= MaxSlotsToScan; slot++)
            {
                if (slot == current)
                {
                    continue;
                }

                try
                {
                    // 파일이 실제로 있을 때만 게임 저장 API를 호출한다(없는 슬롯에서 오류 로그가 나오는 것 방지)
                    string fullPath = Path.Combine(SavesSystem.GetFullPathToSavesFolder(), SavesSystem.GetSaveFileName(slot));
                    if (!File.Exists(fullPath))
                    {
                        continue;
                    }
                    if (!SavesSystem.KeyExisits(SkillSystem.SaveKey, slot))
                    {
                        // 이전 이름으로 저장된 기록도 계승에 포함한다
                        if (!SavesSystem.KeyExisits(SkillSystem.LegacySaveKey, slot))
                        {
                            continue;
                        }
                        string legacyRaw = SavesSystem.Load<string>(SkillSystem.LegacySaveKey, slot);
                        if (string.IsNullOrEmpty(legacyRaw))
                        {
                            continue;
                        }
                        _scannedSlots++;
                        _otherTotalLevel += SkillSystem.TotalLevelFromRaw(legacyRaw, _config);
                        continue;
                    }

                    string raw = SavesSystem.Load<string>(SkillSystem.SaveKey, slot);
                    if (string.IsNullOrEmpty(raw))
                    {
                        continue;
                    }

                    _scannedSlots++;
                    _otherTotalLevel += SkillSystem.TotalLevelFromRaw(raw, _config);
                }
                catch (Exception)
                {
                    // 개별 슬롯 실패는 무시하고 나머지를 계속 확인
                }
            }

            Debug.Log("[Dskill] 계승 계산 완료: 다른 세이브 " + _scannedSlots + "개 / 스킬 레벨 합계 " +
                      _otherTotalLevel + " / 경험치 +" + (BonusRatio * 100f).ToString("0.#") + "%");
        }

        /// <summary>화면에 표시할 요약 문장</summary>
        public string Describe()
        {
            if (_config == null || !_config.MetaEnabled)
            {
                return "계승: 사용 안 함";
            }
            if (!_scanned || _scannedSlots == 0)
            {
                return "계승: 다른 세이브 기록 없음";
            }
            return "계승 " + GradeName + " (다른 세이브 " + _scannedSlots + "개 / Lv." + _otherTotalLevel +
                   ") → 경험치 +" + (BonusRatio * 100f).ToString("0.#") + "%";
        }

        /// <summary>게임 시작 시 한 번 보여줄 알림 문구 (보너스가 없으면 null)</summary>
        public string StartupNotice()
        {
            if (_config == null || !_config.MetaEnabled || _otherTotalLevel <= 0)
            {
                return null;
            }
            return "[계승] 다른 세이브의 기록으로 경험치 +" + (BonusRatio * 100f).ToString("0.#") + "% (" + GradeName + ")";
        }
    }
}
