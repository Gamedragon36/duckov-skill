using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Dskill
{
    /// <summary>
    /// 모드 설정. 모드 폴더의 config.ini 파일을 읽고 쓴다.
    /// 파일이 없으면 기본값으로 새로 만들어 준다(사용자가 직접 수정 가능).
    /// </summary>
    public class Config
    {
        // ----- 전역 설정 -----
        public int MaxLevel = 20;            // 스킬 최대 레벨
        public float XpBase = 300f;          // 1레벨에 필요한 경험치
        public float XpStep = 65f;           // 레벨마다 늘어나는 경험치
        public float XpMultiplier = 1f;      // 모든 경험치 획득 배율
        public string Hotkey = "F6";         // 스킬 창 열기 키
        public bool NotifyLevelUp = true;    // 레벨업 알림 표시

        // ----- 로그라이크(메타 진행도) 설정 -----
        public bool MetaEnabled = true;              // 다른 세이브의 스킬 레벨로 경험치 보너스
        public float MetaBonusPercent = 0.5f;        // 다른 세이브 스킬 레벨 1당 보너스 (%)
        public float MetaBonusCapPercent = 150f;     // 최대 보너스 (%)

        /// <summary>스킬별 경험치 배율 (스킬 id -> 배율)</summary>
        public readonly Dictionary<string, float> SkillXpMultiplier = new Dictionary<string, float>();

        public string ConfigPath { get; private set; }

        private const string FileName = "config.ini";

        /// <summary>dll 옆의 config.ini 를 읽는다. 없으면 기본값 파일을 만든다.</summary>
        public static Config Load()
        {
            Config config = new Config();
            try
            {
                string dir = Path.GetDirectoryName(typeof(Config).Assembly.Location);
                config.ConfigPath = Path.Combine(dir ?? ".", FileName);
                if (File.Exists(config.ConfigPath))
                {
                    config.Parse(File.ReadAllLines(config.ConfigPath));
                }
                else
                {
                    config.WriteDefault();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Dskill] 설정 파일 처리 실패: " + e.Message);
            }

            // 스킬별 배율 기본값 채우기
            foreach (SkillDef def in SkillDefs.All)
            {
                if (!config.SkillXpMultiplier.ContainsKey(def.Id))
                {
                    config.SkillXpMultiplier[def.Id] = 1f;
                }
            }
            return config;
        }

        private void Parse(string[] lines)
        {
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("["))
                {
                    continue;
                }

                int index = line.IndexOf('=');
                if (index <= 0)
                {
                    continue;
                }

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                // 값 뒤의 주석 제거
                int comment = value.IndexOf('#');
                if (comment >= 0)
                {
                    value = value.Substring(0, comment).Trim();
                }

                switch (key)
                {
                    case "max_level": MaxLevel = ParseInt(value, MaxLevel); break;
                    case "xp_base": XpBase = ParseFloat(value, XpBase); break;
                    case "xp_step": XpStep = ParseFloat(value, XpStep); break;
                    case "xp_multiplier": XpMultiplier = ParseFloat(value, XpMultiplier); break;
                    case "hotkey": Hotkey = value; break;
                    case "notify_levelup": NotifyLevelUp = ParseBool(value, NotifyLevelUp); break;
                    case "meta_enabled": MetaEnabled = ParseBool(value, MetaEnabled); break;
                    case "meta_bonus_per_level": MetaBonusPercent = ParseFloat(value, MetaBonusPercent); break;
                    case "meta_bonus_cap": MetaBonusCapPercent = ParseFloat(value, MetaBonusCapPercent); break;
                    default:
                        SkillXpMultiplier[key] = ParseFloat(value, 1f);
                        break;
                }
            }

            if (MaxLevel < 1) MaxLevel = 1;
            if (MaxLevel > 200) MaxLevel = 200;
            if (XpMultiplier <= 0f) XpMultiplier = 0.01f;
            if (MetaBonusPercent < 0f) MetaBonusPercent = 0f;
            if (MetaBonusPercent > 20f) MetaBonusPercent = 20f;
            if (MetaBonusCapPercent < 0f) MetaBonusCapPercent = 0f;
            if (MetaBonusCapPercent > 1000f) MetaBonusCapPercent = 1000f;
        }

        private static int ParseInt(string text, int fallback)
        {
            int result;
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : fallback;
        }

        private static float ParseFloat(string text, float fallback)
        {
            float result;
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result) ? result : fallback;
        }

        private static bool ParseBool(string text, bool fallback)
        {
            if (string.IsNullOrEmpty(text))
            {
                return fallback;
            }
            string v = text.Trim().ToLowerInvariant();
            if (v == "true" || v == "1" || v == "on" || v == "yes" || v == "켜기" || v == "예")
            {
                return true;
            }
            if (v == "false" || v == "0" || v == "off" || v == "no" || v == "끄기" || v == "아니오")
            {
                return false;
            }
            return fallback;
        }

        /// <summary>기본값 설정 파일을 만들어 준다(한글 설명 포함).</summary>
        private void WriteDefault()
        {
            List<string> lines = new List<string>();
            lines.Add("# ============================================");
            lines.Add("#  Tarkov Skills (타르코프식 스킬 시스템) 설정");
            lines.Add("#  값을 고친 뒤 게임을 다시 시작하면 적용됩니다.");
            lines.Add("#  '#' 뒤의 내용은 설명이므로 지워도 됩니다.");
            lines.Add("# ============================================");
            lines.Add("");
            lines.Add("[general]");
            lines.Add("max_level = 20        # 스킬 최대 레벨 (만렙)");
            lines.Add("xp_base = 300         # 1레벨에 필요한 경험치");
            lines.Add("xp_step = 65          # 레벨마다 늘어나는 경험치");
            lines.Add("xp_multiplier = 1.0   # 전체 경험치 배율 (0.5=절반, 2.0=두배)");
            lines.Add("hotkey = F6           # 스킬 창 여는 키 (예: F6, F7, F8)");
            lines.Add("notify_levelup = true # 레벨업 때 알림 표시 (true/false)");
            lines.Add("");
            lines.Add("[meta]");
            lines.Add("# 로그라이크(계승): '다른 세이브'에서 키운 스킬 레벨 합계가");
            lines.Add("# 이번에 플레이하는 세이브의 경험치 획득 속도를 올려줍니다.");
            lines.Add("meta_enabled = true          # 기능 사용 (true/false)");
            lines.Add("meta_bonus_per_level = 0.5   # 다른 세이브의 스킬 레벨 1당 경험치 +0.5%");
            lines.Add("meta_bonus_cap = 150         # 최대 보너스 (%)");
            lines.Add("");
            lines.Add("[skill_xp]");
            lines.Add("# 스킬별 경험치 배율 (0.5=절반 속도, 2.0=두배 속도)");
            foreach (SkillDef def in SkillDefs.All)
            {
                lines.Add(def.Id + " = 1.0   # " + def.NameKo);
            }

            try
            {
                File.WriteAllLines(ConfigPath, lines.ToArray(), new System.Text.UTF8Encoding(false));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Dskill] 설정 파일 생성 실패: " + e.Message);
            }
        }

        /// <summary>스킬 하나의 최종 경험치 배율</summary>
        public float MultiplierFor(string skillId)
        {
            float value;
            if (SkillXpMultiplier.TryGetValue(skillId, out value))
            {
                return XpMultiplier * value;
            }
            return XpMultiplier;
        }
    }
}
