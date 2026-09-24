using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 스킬 창. 이미지 없이 텍스트·특수문자·색 사각형만 사용해 깔끔하게 표시한다.
    /// 카테고리 탭으로 나눠서 한 화면에 들어오게 했다.
    /// </summary>
    public static class SkillWindow
    {
        private const float BarWidth = 105f;
        private const float BarHeight = 12f;

        private static int _tab;                     // 0 = 신체, 1 = 전투, 2 = 실용
        private static GUIStyle _title, _nameLabel, _levelLabel, _percentLabel, _effectLabel, _subLabel, _footerLabel, _iconLabel;
        private static bool _stylesReady;

        // 창 위치 (제목줄을 마우스로 끌어서 옮길 수 있고, 게임을 끌 때까지 유지된다)
        private static Rect _windowRect = new Rect(70f, 36f, 880f, 520f);
        private static float _autoHeight = 520f;
        private static SkillSystem _drawSystem;
        private static Config _drawConfig;
        private static MetaProgress _drawMeta;
        private static bool _positionInitialized;
        private static Vector2 _listScroll;
        private const int WindowId = 710426;

        /// <summary>Tab 키로 카테고리 전환</summary>
        public static void CycleTab()
        {
            if (SkillDefs.Categories.Length == 0)
            {
                return;
            }
            _tab = (_tab + 1) % SkillDefs.Categories.Length;
        }

        public static void Draw(SkillSystem system, Config config, MetaProgress meta)
        {
            EnsureStyles();
            ApplyFont();

            _drawSystem = system;
            _drawConfig = config;
            _drawMeta = meta;

            // F7 키로 계열 전환 (Tab 은 인벤토리 키와 겹쳐서 사용하지 않는다)
            if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F7)
            {
                CycleTab();
            }

            // 창 크기: 계산식으로 정한다(측정값이 아니라서 창을 어디로 옮겨도 안정적)
            _windowRect.width = Mathf.Min(880f, Mathf.Max(420f, Screen.width - 40f));
            _windowRect.height = Mathf.Max(240f, _autoHeight);

            // 처음 열 때는 화면 가운데 위쪽에 배치한다(다른 UI와 덜 겹치게)
            if (!_positionInitialized)
            {
                _positionInitialized = true;
                _windowRect.x = Mathf.Max(20f, (Screen.width - _windowRect.width) * 0.5f);
                _windowRect.y = Mathf.Max(20f, Screen.height * 0.08f);
            }

            _windowRect = GUI.Window(WindowId, _windowRect, DrawWindow,
                "   " + Locale.T("ui.title", "★ 타르코프 스킬 시스템   —   제목줄을 마우스로 끌어서 이동"));
        }

        private static void DrawWindow(int id)
        {
            SkillSystem system = _drawSystem;
            Config config = _drawConfig;
            if (system == null || config == null)
            {
                GUI.DragWindow();
                return;
            }

            string category = SkillDefs.Categories[_tab];        // 비교에 쓰는 코드 기준 값
            string categoryLabel = Locale.Category(category);    // 화면에 보여줄 이름
            int totalLevel = 0;
            foreach (SkillDef def in SkillDefs.All)
            {
                totalLevel += system.GetLevel(def.Id);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Locale.F("ui.series", "계열: {0}", categoryLabel), _title);
            GUILayout.FlexibleSpace();
            GUILayout.Label(Locale.F("ui.total", "총합 Lv.{0} / {1}", totalLevel, system.MaxLevel * SkillDefs.All.Length), _footerLabel);
            GUILayout.EndHorizontal();

            if (_drawMeta != null)
            {
                GUILayout.Label("<color=#FFD24A>" + _drawMeta.Describe() + "</color>", _subLabel);
            }

            GUILayout.BeginHorizontal();
            for (int i = 0; i < SkillDefs.Categories.Length; i++)
            {
                string prefix = i == _tab ? "▶ " : "     ";
                if (GUILayout.Button(prefix + Locale.Category(SkillDefs.Categories[i]), GUILayout.Width(112f)))
                {
                    _tab = i;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label(Locale.F("ui.tabs", "[F7] 계열 전환    [{0} / ESC] 닫기", config.Hotkey), _footerLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            DrawDivider();

            // 스킬이 많아도 창이 화면을 넘지 않도록 목록만 필요할 때 스크롤한다
            int rowCount = 0;
            foreach (SkillDef def in SkillDefs.All)
            {
                if (def.Category == category)
                {
                    rowCount++;
                }
            }
            float listHeight = rowCount * 46f;
            float maxListHeight = Mathf.Max(160f, Screen.height * 0.6f);
            float displayListHeight = Mathf.Min(listHeight, maxListHeight);

            // 창 높이 = 헤더/탭/푸터(약 140) + 목록 높이 (+ 계승 줄)
            // 화면보다 커지지 않게 제한한다.
            _autoHeight = 140f + displayListHeight + (_drawMeta != null ? 18f : 0f);
            _autoHeight = Mathf.Min(_autoHeight, Mathf.Max(240f, Screen.height - 40f));

            if (listHeight > maxListHeight)
            {
                _listScroll = GUILayout.BeginScrollView(_listScroll, GUILayout.Height(maxListHeight));
                for (int i = 0; i < SkillDefs.All.Length; i++)
                {
                    SkillDef def = SkillDefs.All[i];
                    if (def.Category == category)
                    {
                        DrawRow(system, def);
                    }
                }
                GUILayout.EndScrollView();
            }
            else
            {
                for (int i = 0; i < SkillDefs.All.Length; i++)
                {
                    SkillDef def = SkillDefs.All[i];
                    if (def.Category == category)
                    {
                        DrawRow(system, def);
                    }
                }
            }

            DrawDivider();
            GUILayout.Label(Locale.T("ui.footer", "경험치는 게임을 저장할 때 세이브 슬롯에 함께 저장됩니다."), _footerLabel);

            // 제목줄을 끌면 창이 움직인다
            GUI.DragWindow(new Rect(0f, 0f, _windowRect.width, 26f));
        }

        private static void DrawRow(SkillSystem system, SkillDef def)
        {
            int level = system.GetLevel(def.Id);
            bool elite = system.IsElite(def.Id);

            float current;
            float needed;
            system.GetProgress(def.Id, out current, out needed);
            float ratio = Mathf.Clamp01(current / needed);

            GUILayout.BeginHorizontal();
            GUILayout.Label(def.Icon, _iconLabel, GUILayout.Width(22f));
            string skillName = Locale.SkillName(def);
            GUILayout.Label(elite ? "<color=#FFD24A>" + skillName + "</color>" : skillName, _nameLabel, GUILayout.Width(125f));
            GUILayout.Label(elite ? "<color=#FFD24A>★" + level + "/" + system.MaxLevel + "</color>" : level + " / " + system.MaxLevel,
                _levelLabel, GUILayout.Width(76f));
            DrawBar(ratio);
            GUILayout.Space(8f);
            GUILayout.Label((ratio * 100f).ToString("0") + "%", _percentLabel, GUILayout.Width(42f));
            GUILayout.Label(system.DescribeEffects(def.Id, level), _effectLabel);
            GUILayout.EndHorizontal();

            string growthText = "<color=#8FA3B8>       " + Locale.F("ui.growth", "성장: {0}", Locale.SkillTrigger(def));
            if (elite)
            {
                // 만렙이면 엘리트 효과도 함께 보여 준다
                growthText += "   ★" + Locale.SkillElite(def);
            }
            GUILayout.Label(growthText + "</color>", _subLabel);
            GUILayout.Space(4f);
        }

        /// <summary>경험치 막대를 색 사각형으로 그린다.</summary>
        private static void DrawBar(float ratio)
        {
            Rect rect = GUILayoutUtility.GetRect(BarWidth, BarHeight, GUILayout.Width(BarWidth), GUILayout.Height(BarHeight));

            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            GUI.color = ratio >= 1f ? new Color(1f, 0.82f, 0.29f) : new Color(0.36f, 0.78f, 0.45f);
            GUI.DrawTexture(new Rect(rect.x + 1f, rect.y + 1f, Mathf.Max(0f, (rect.width - 2f) * ratio), rect.height - 2f),
                Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private static void DrawDivider()
        {
            Rect rect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
            GUI.color = new Color(1f, 1f, 1f, 0.15f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUILayout.Space(6f);
        }

        private static void EnsureStyles()
        {
            if (_stylesReady)
            {
                return;
            }
            _title = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, richText = true };
            _iconLabel = new GUIStyle(GUI.skin.label) { fontSize = 15, alignment = TextAnchor.MiddleLeft };
            _nameLabel = new GUIStyle(GUI.skin.label) { fontSize = 15, richText = true };
            _levelLabel = new GUIStyle(GUI.skin.label) { fontSize = 14, richText = true };
            _percentLabel = new GUIStyle(GUI.skin.label) { fontSize = 13, alignment = TextAnchor.MiddleRight };
            _effectLabel = new GUIStyle(GUI.skin.label) { fontSize = 14, richText = true };
            _subLabel = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = true };
            _footerLabel = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            _stylesReady = true;
        }

        /// <summary>모드가 매 프레임 폰트를 바꾸므로 스타일에도 같은 폰트를 넣어 준다.</summary>
        private static void ApplyFont()
        {
            Font font = GUI.skin.font;
            _title.font = font;
            _iconLabel.font = font;
            _nameLabel.font = font;
            _levelLabel.font = font;
            _percentLabel.font = font;
            _effectLabel.font = font;
            _subLabel.font = font;
            _footerLabel.font = font;
        }
    }
}
