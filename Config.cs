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
        public string Language = "auto";     // 표시 언어 (auto = OS 언어를 따름)
        public int XpStage = 5;              // 경험치 획득 난이도 1~5 (1=가장 빠름, 5=기본)

        // ----- 제작 스킬 밸런스 (값은 여기서 자유롭게 조정) -----
        public float CraftUnlockXp = 200f;    // 새 레시피 해금 XP (여러 개가 한꺼번에 풀리면 1회만 인정)
        public float CraftXpPerValue = 0.08f; // 제작 1회 XP = 재료 가치 × 이 값 (0.08 = 8%)
        public float CraftXpCap = 400f;       // 제작 1회 최대 XP (0 = 제한 없음)

        /// <summary>제작자용: true 로 두고 실행하면 창작마당 업로드를 1회 실행한다(실행 후 자동 false)</summary>
        public bool UploadNow = false;

        // ----- 로그라이크(메타 진행도) 설정 -----
        public bool MetaEnabled = true;              // 다른 세이브의 스킬 레벨로 경험치 보너스
        public float MetaBonusPercent = 0.5f;        // 다른 세이브 스킬 레벨 1당 보너스 (%)
        public float MetaBonusCapPercent = 150f;     // 최대 보너스 (%)

        /// <summary>true 로 두고 게임을 실행하면 모든 스킬 경험치를 1회 초기화한다(실행 후 자동으로 false)</summary>
        public bool ResetSkills = false;

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
                    case "reset_skills": ResetSkills = ParseBool(value, ResetSkills); break;
                    case "language": Language = value; break;
                    case "xp_stage": XpStage = ParseInt(value, XpStage); break;
                    case "craft_unlock_xp": CraftUnlockXp = ParseFloat(value, CraftUnlockXp); break;
                    case "craft_xp_per_value": CraftXpPerValue = ParseFloat(value, CraftXpPerValue); break;
                    case "craft_xp_cap": CraftXpCap = ParseFloat(value, CraftXpCap); break;
                    case "upload_now": UploadNow = ParseBool(value, UploadNow); break;
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
            if (XpStage < 1) XpStage = 1;
            if (XpStage > 5) XpStage = 5;
            if (CraftUnlockXp < 0f) CraftUnlockXp = 0f;
            if (CraftXpPerValue < 0f) CraftXpPerValue = 0f;
            if (CraftXpCap < 0f) CraftXpCap = 0f;
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

        /// <summary>기본값 설정 파일을 만들어 준다(선택한 언어의 설명 포함).</summary>
        private void WriteDefault()
        {
            List<string> lines = new List<string>();
            lines.Add("# ============================================");
            lines.Add("#  " + Locale.T("cfg.header", "Tarkov Skills (타르코프식 스킬 시스템) 설정"));
            lines.Add("#  " + Locale.T("cfg.headerNote1", "값을 고친 뒤 게임을 다시 시작하면 적용됩니다."));
            lines.Add("#  " + Locale.T("cfg.headerNote2", "'#' 뒤의 내용은 설명이므로 지워도 됩니다."));
            lines.Add("# ============================================");
            lines.Add("");
            lines.Add("[general]");
            lines.Add("max_level = 20        # " + Locale.T("cfg.maxLevel", "스킬 최대 레벨 (만렙)"));
            lines.Add("xp_base = 300         # " + Locale.T("cfg.xpBase", "1레벨에 필요한 경험치"));
            lines.Add("xp_step = 65          # " + Locale.T("cfg.xpStep", "레벨마다 늘어나는 경험치"));
            lines.Add("xp_multiplier = 1.0   # " + Locale.T("cfg.xpMultiplier", "전체 경험치 배율 (0.5=절반, 2.0=두배)"));
            lines.Add("hotkey = F6           # " + Locale.T("cfg.hotkey", "스킬 창 여는 키 (예: F6, F7, F8)"));
            lines.Add("notify_levelup = true # " + Locale.T("cfg.notifyLevelUp", "레벨업 때 알림 표시 (true/false)"));
            lines.Add("language = auto       # " + Locale.T("cfg.language", "표시 언어: auto(게임에서 고른 언어) / ko / en / zh / zh-hant / ja / de / ru / es / fr / pt-br"));
            lines.Add("xp_stage = 5          # " + Locale.T("cfg.xpStage", "경험치 난이도 1~5 (1=가장 빠름, 5=기본)"));
            lines.Add("craft_unlock_xp = 200    # " + Locale.T("cfg.craftUnlockXp", "새 레시피 해금 XP (한꺼번에 여러 개가 풀리면 1회만 인정)"));
            lines.Add("craft_xp_per_value = 0.08 # " + Locale.T("cfg.craftXpPerValue", "제작 1회 XP = 재료 가치 × 이 값 (0.08 = 8%)"));
            lines.Add("craft_xp_cap = 400       # " + Locale.T("cfg.craftXpCap", "제작 1회 최대 XP (0 = 제한 없음)"));
            lines.Add("upload_now = false    # " + Locale.T("cfg.uploadNow", "true 로 두고 실행하면 창작마당 업로드를 1회 실행 (제작자용)"));
            lines.Add("");
            lines.Add("[meta]");
            lines.Add("# " + Locale.T("cfg.metaNote1", "로그라이크(계승): '다른 세이브'에서 키운 스킬 레벨 합계가"));
            lines.Add("# " + Locale.T("cfg.metaNote2", "이번에 플레이하는 세이브의 경험치 획득 속도를 올려줍니다."));
            lines.Add("meta_enabled = true          # " + Locale.T("cfg.metaEnabled", "기능 사용 (true/false)"));
            lines.Add("meta_bonus_per_level = 0.5   # " + Locale.T("cfg.metaPerLevel", "다른 세이브의 스킬 레벨 1당 경험치 +0.5%"));
            lines.Add("meta_bonus_cap = 150         # " + Locale.T("cfg.metaCap", "최대 보너스 (%)"));
            lines.Add("");
            lines.Add("# " + Locale.T("cfg.resetNote1", "위험: true 로 두고 게임을 한 번 실행하면 모든 스킬 경험치가 0으로 초기화됩니다."));
            lines.Add("# " + Locale.T("cfg.resetNote2", "초기화가 끝나면 이 값은 자동으로 false 로 되돌아갑니다."));
            lines.Add("reset_skills = false");
            lines.Add("");
            lines.Add("[skill_xp]");
            lines.Add("# " + Locale.T("cfg.skillXpNote", "스킬별 경험치 배율 (0.5=절반 속도, 2.0=두배 속도)"));
            foreach (SkillDef def in SkillDefs.All)
            {
                lines.Add(def.Id + " = 1.0   # " + Locale.SkillName(def));
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

        /// <summary>경험치 난이도(1~5단계) 배율. 5단계(기본) = 1.0 (지금까지의 밸런스)</summary>
        public float XpStageMultiplier
        {
            get
            {
                int index = XpStage - 1;
                if (index < 0 || index >= Specials.XpStageMultipliers.Length)
                {
                    return 1f;
                }
                return Specials.XpStageMultipliers[index];
            }
        }

        /// <summary>게임 안(스킬 창)에서 난이도를 바꾸고 설정 파일에 저장한다.</summary>
        public void SetXpStage(int stage)
        {
            if (stage < 1)
            {
                stage = 1;
            }
            if (stage > 5)
            {
                stage = 5;
            }
            if (stage == XpStage)
            {
                return;
            }
            XpStage = stage;
            SaveXpStage();
        }

        /// <summary>xp_stage 값만 설정 파일에서 찾아 바꾼다(없으면 맨 끝에 추가).</summary>
        private void SaveXpStage()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                {
                    return;
                }

                string comment = Locale.T("cfg.xpStage", "경험치 난이도 1~5 (1=가장 빠름, 5=기본)");
                string[] lines = File.ReadAllLines(ConfigPath);
                bool found = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("xp_stage"))
                    {
                        lines[i] = "xp_stage = " + XpStage + "   # " + comment;
                        found = true;
                    }
                }
                if (!found)
                {
                    string[] bigger = new string[lines.Length + 2];
                    Array.Copy(lines, bigger, lines.Length);
                    bigger[lines.Length] = "";
                    bigger[lines.Length + 1] = "xp_stage = " + XpStage + "   # " + comment;
                    lines = bigger;
                }
                File.WriteAllLines(ConfigPath, lines, new System.Text.UTF8Encoding(false));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Dskill] 설정 파일 갱신 실패: " + e.Message);
            }
        }

        /// <summary>upload_now 플래그를 false 로 되돌려 설정 파일에 다시 쓴다(1회 업로드 후 자동 해제).</summary>
        public void ClearUploadFlag()
        {
            UploadNow = false;
            try
            {
                if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(ConfigPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("upload_now"))
                    {
                        lines[i] = "upload_now = false    # " + Locale.T("cfg.uploadNow", "true 로 두고 실행하면 창작마당 업로드를 1회 실행 (제작자용)");
                    }
                }
                File.WriteAllLines(ConfigPath, lines, new System.Text.UTF8Encoding(false));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Dskill] 설정 파일 갱신 실패: " + e.Message);
            }
        }

        /// <summary>reset_skills 플래그를 false 로 되돌려 설정 파일에 다시 쓴다(1회 초기화 후 자동 해제).</summary>
        public void ClearResetFlag()
        {
            ResetSkills = false;
            try
            {
                if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(ConfigPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("reset_skills"))
                    {
                        lines[i] = "reset_skills = false   # " + Locale.T("cfg.resetSkills", "true 로 두고 실행하면 1회 초기화");
                    }
                }
                File.WriteAllLines(ConfigPath, lines, new System.Text.UTF8Encoding(false));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Dskill] 설정 파일 갱신 실패: " + e.Message);
            }
        }
    }
}
