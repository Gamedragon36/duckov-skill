using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 게임이 지원하는 10개 언어. 순서(값)를 바꾸면 언어 파일과 어긋나므로 주의.
    /// </summary>
    public enum Lang
    {
        En,
        ZhHans,
        ZhHant,
        Ja,
        De,
        Ru,
        Es,
        Ko,
        Fr,
        PtBr
    }

    /// <summary>
    /// 모드 안의 모든 표시 문장을 담당한다.
    ///  - 각 언어 파일(Locale.*.cs)이 "key|문장" 목록(표)을 제공한다.
    ///  - 없는 키는 [현재 언어] -> [영어] -> [호출부의 한국어 기본값] 순서로 대체한다.
    ///  - 한국어는 코드 안의 기본값을 쓰므로 별도 언어 파일이 없다.
    /// </summary>
    public static class Locale
    {
        private const string EmptyTable = "";

        private static Dictionary<string, string> _active;
        private static Dictionary<string, string> _english;

        /// <summary>지금 사용 중인 언어</summary>
        public static Lang Current { get; private set; }

        /// <summary>지금 사용 중인 언어 코드 (예: ko, zh-hans)</summary>
        public static string CurrentCode { get; private set; }

        /// <summary>config.ini 값이 auto 여서 OS 언어를 따라갔는지</summary>
        public static bool AutoDetected { get; private set; }

        /// <summary>config.ini 의 language 값을 적용한다. "auto"/빈 값이면 OS 언어를 따른다.</summary>
        public static void Setup(string setting)
        {
            Lang lang;
            string value = (setting ?? string.Empty).Trim();

            if (value.Length > 0 && !value.Equals("auto", StringComparison.OrdinalIgnoreCase) && TryParseCode(value, out lang))
            {
                Current = lang;
                AutoDetected = false;
            }
            else
            {
                Current = DetectSystem();
                AutoDetected = true;
            }

            CurrentCode = CodeOf(Current);
            _active = null;   // 다음 조회 때 표를 다시 준비한다
        }

        /// <summary>로그에 남길 언어 코드</summary>
        public static string CodeOf(Lang lang)
        {
            switch (lang)
            {
                case Lang.ZhHans: return "zh-hans";
                case Lang.ZhHant: return "zh-hant";
                case Lang.Ja: return "ja";
                case Lang.De: return "de";
                case Lang.Ru: return "ru";
                case Lang.Es: return "es";
                case Lang.Ko: return "ko";
                case Lang.Fr: return "fr";
                case Lang.PtBr: return "pt-br";
                default: return "en";
            }
        }

        /// <summary>config.ini 에 적은 언어 이름을 해석한다 (ko, zh, zh-hant, ja, de, ru, es, en, fr, pt)</summary>
        public static bool TryParseCode(string code, out Lang lang)
        {
            switch ((code ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "en":
                case "english":
                    lang = Lang.En;
                    return true;
                case "zh":
                case "zh-cn":
                case "zh-hans":
                case "chs":
                case "chinese":
                    lang = Lang.ZhHans;
                    return true;
                case "zh-tw":
                case "zh-hant":
                case "cht":
                case "traditional":
                    lang = Lang.ZhHant;
                    return true;
                case "ja":
                case "jp":
                case "japanese":
                    lang = Lang.Ja;
                    return true;
                case "de":
                case "german":
                    lang = Lang.De;
                    return true;
                case "ru":
                case "russian":
                    lang = Lang.Ru;
                    return true;
                case "es":
                case "spanish":
                    lang = Lang.Es;
                    return true;
                case "ko":
                case "kr":
                case "korean":
                    lang = Lang.Ko;
                    return true;
                case "fr":
                case "french":
                    lang = Lang.Fr;
                    return true;
                case "pt":
                case "pt-br":
                case "portuguese":
                    lang = Lang.PtBr;
                    return true;
                default:
                    lang = Lang.En;
                    return false;
            }
        }

        private const string GameLanguagePref = "language";   // 게임이 언어를 저장하는 PlayerPrefs 키

        /// <summary>표시 언어 결정: 게임에서 고른 언어 -> 운영체제 언어 순서로 확인한다.</summary>
        private static Lang DetectSystem()
        {
            Lang fromGame;
            if (TryGameLanguage(out fromGame))
            {
                return fromGame;
            }

            try
            {
                return Map(Application.systemLanguage);
            }
            catch (Exception)
            {
                return Lang.En;
            }
        }

        /// <summary>게임이 저장해 둔 언어 (PlayerPrefs "language" 에 SystemLanguage 값으로 들어 있다)</summary>
        private static bool TryGameLanguage(out Lang lang)
        {
            lang = Lang.En;
            try
            {
                if (!PlayerPrefs.HasKey(GameLanguagePref))
                {
                    return false;
                }
                int value = PlayerPrefs.GetInt(GameLanguagePref, -1);
                if (value < 0)
                {
                    return false;
                }
                return TryMap((SystemLanguage)value, out lang);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Unity 언어 -> 이 모드의 언어. 지원하지 않는 언어면 false (영어로 대체).</summary>
        private static bool TryMap(SystemLanguage language, out Lang lang)
        {
            switch (language)
            {
                case SystemLanguage.ChineseSimplified:
                    lang = Lang.ZhHans;
                    return true;
                case SystemLanguage.ChineseTraditional:
                    lang = Lang.ZhHant;
                    return true;
                case SystemLanguage.Chinese:
                    lang = Lang.ZhHans;
                    return true;
                case SystemLanguage.Japanese:
                    lang = Lang.Ja;
                    return true;
                case SystemLanguage.Korean:
                    lang = Lang.Ko;
                    return true;
                case SystemLanguage.German:
                    lang = Lang.De;
                    return true;
                case SystemLanguage.Russian:
                    lang = Lang.Ru;
                    return true;
                case SystemLanguage.Spanish:
                    lang = Lang.Es;
                    return true;
                case SystemLanguage.French:
                    lang = Lang.Fr;
                    return true;
                case SystemLanguage.Portuguese:
                    lang = Lang.PtBr;
                    return true;
                default:
                    lang = Lang.En;
                    return false;
            }
        }

        private static Lang Map(SystemLanguage language)
        {
            Lang lang;
            return TryMap(language, out lang) ? lang : Lang.En;
        }

        private static void EnsureTables()
        {
            if (_active != null)
            {
                return;
            }
            if (_english == null)
            {
                _english = Parse(LocEn.Table);
            }
            _active = Parse(TableOf(Current));
        }

        private static string TableOf(Lang lang)
        {
            switch (lang)
            {
                case Lang.ZhHans: return LocZhHans.Table;
                case Lang.ZhHant: return LocZhHant.Table;
                case Lang.Ja: return LocJa.Table;
                case Lang.De: return LocDe.Table;
                case Lang.Ru: return LocRu.Table;
                case Lang.Es: return LocEs.Table;
                case Lang.Ko: return EmptyTable;        // 한국어는 코드 안의 기본값 사용
                case Lang.Fr: return LocFr.Table;
                case Lang.PtBr: return LocPtBr.Table;
                default: return EmptyTable;
            }
        }

        /// <summary>"key|문장" 목록을 사전으로 바꾼다. 빈 줄과 '#' 로 시작하는 줄은 무시.</summary>
        private static Dictionary<string, string> Parse(string table)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(table))
            {
                return map;
            }
            foreach (string raw in table.Split('\n'))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#')
                {
                    continue;
                }
                int bar = line.IndexOf('|');
                if (bar <= 0)
                {
                    continue;
                }
                string key = line.Substring(0, bar).Trim();
                string value = line.Substring(bar + 1).Trim();
                if (key.Length > 0)
                {
                    map[key] = value;
                }
            }
            return map;
        }

        /// <summary>문장 조회. 없으면 영어 -> 호출부의 한국어 기본값 순으로 대체한다.
        ///  (단, 한국어는 표를 두지 않고 코드 안의 문장을 쓰므로 영어로 넘어가면 안 된다)</summary>
        public static string T(string key, string korean)
        {
            try
            {
                EnsureTables();
                string value;
                if (_active.TryGetValue(key, out value))
                {
                    return value;
                }
                if (Current != Lang.Ko && _english.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            catch (Exception)
            {
                // 표를 읽지 못해도 모드는 계속 동작해야 한다
            }
            return korean;
        }

        /// <summary>{0}, {1} 자리를 채워 문장을 만든다.</summary>
        public static string F(string key, string korean, params object[] args)
        {
            string format = T(key, korean);
            try
            {
                return string.Format(format, args);
            }
            catch (Exception)
            {
                return format;
            }
        }

        // ------------------------------------------------------------------
        // 자주 쓰는 조회 (스킬 / 계열 / 스탯)
        // ------------------------------------------------------------------

        public static string SkillName(SkillDef def)
        {
            return def == null ? string.Empty : T("skill." + def.Id + ".name", def.NameKo);
        }

        public static string SkillTrigger(SkillDef def)
        {
            return def == null ? string.Empty : T("skill." + def.Id + ".trigger", def.TriggerKo);
        }

        public static string SkillElite(SkillDef def)
        {
            return def == null ? string.Empty : T("skill." + def.Id + ".elite", def.EliteKo);
        }

        /// <summary>계열 이름 (코드 안의 한국어 값을 열쇠로 쓴다)</summary>
        public static string Category(string korean)
        {
            switch (korean)
            {
                case "신체": return T("cat.body", korean);
                case "전투": return T("cat.combat", korean);
                case "실용": return T("cat.utility", korean);
                default: return korean;
            }
        }

        /// <summary>스탯 키 -> 표시 이름</summary>
        public static string Stat(string statKey, string korean)
        {
            return T("stat." + statKey, korean);
        }

    }
}
