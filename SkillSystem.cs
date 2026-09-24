using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using Saves;
using UnityEngine;

namespace Dskill
{
    /// <summary>
    /// 스킬 경험치/레벨/효과를 관리한다.
    /// - 경험치가 쌓이면 레벨이 오르고, 레벨에 맞는 스탯 보너스를 캐릭터에 적용한다.
    /// - 게임 세이브 슬롯에 경험치를 저장한다.
    /// </summary>
    public class SkillSystem
    {
        public const string SaveKey = "Dskill_Xp";

        /// <summary>이전 이름(DuckovTarkovSkills) 때 쓰던 저장 키 — 데이터 이어받기용</summary>
        public const string LegacySaveKey = "TarkovSkills_Xp";

        private readonly Config _config;
        private readonly MetaProgress _meta;

        /// <summary>스킬 id -> 누적 경험치</summary>
        private readonly Dictionary<string, float> _xp = new Dictionary<string, float>();

        /// <summary>스킬 id -> 스탯 수정자를 구분하기 위한 표식</summary>
        private readonly Dictionary<string, object> _tokens = new Dictionary<string, object>();

        /// <summary>경고를 이미 출력한 스탯 키</summary>
        private readonly HashSet<string> _warned = new HashSet<string>();

        /// <summary>현재 보너스를 적용해 둔 캐릭터 아이템</summary>
        private Item _appliedItem;

        private float _originalWeaponRepairLoss;
        private float _originalEquipmentRepairLoss;
        private bool _repairLossSaved;
        private Item _repairLossItem;

        public event Action<string, int> OnLevelUp;

        // 효과 문장 캐시 (스킬 id -> 문장 / 문장을 만든 레벨)
        // 스킬 창이 열려 있는 동안 매 프레임 문자열을 새로 만들지 않도록 한다(GC 부담 제거)
        private readonly Dictionary<string, string> _effectTextCache = new Dictionary<string, string>();
        private readonly Dictionary<string, int> _effectTextLevel = new Dictionary<string, int>();

        // 하이드아웃(기지) 경험치 제한: 허용 목록 캐시 + 차단 로그(스킬당 1회)
        private HashSet<string> _hideoutAllowed;
        private readonly HashSet<string> _hideoutBlockedLogged = new HashSet<string>();

        /// <summary>스탯 키 -> 화면에 보여줄 한글 이름</summary>
        private static readonly Dictionary<string, string> StatNamesKo = new Dictionary<string, string>
        {
            { "MaxWeight", "최대 무게" },
            { "InventoryCapacity", "가방 칸" },
            { "StaminaDrainRate", "스태미나 소모" },
            { "StaminaRecoverRate", "스태미나 회복" },
            { "MaxHealth", "최대 생명력" },
            { "HealGain", "치료 효율" },
            { "GunDamageMultiplier", "총기 대미지" },
            { "GunCritRateGain", "총기 치명타율" },
            { "RecoilControl", "반동 제어" },
            { "RecoilScaleV", "수직 반동" },
            { "RecoilScaleH", "수평 반동" },
            { "GunDistanceMultiplier", "총기 사거리" },
            { "GunScatterMultiplier", "탄퍼짐" },
            { "GunCritDamageGain", "헤드샷 대미지" },
            { "MeleeDamageMultiplier", "근접 대미지" },
            { "MeleeCritRateGain", "근접 치명타율" },
            { "WalkSpeed", "이동 속도" },
            { "RunSpeed", "이동 속도" },
            { "WalkSoundRange", "보행 소리" },
            { "RunSoundRange", "달리기 소리" },
            { "SoundRange", "소리 범위" },
            { "BodyArmor", "신체 방어구" },
            { "HeadArmor", "머리 방어구" },
            { "DurabilityCost", "내구도 소모" },
            { "BuffChance", "디버프 확률" },
            { "ElementFactor_Fire", "받는 화염 피해" },
            { "ElementFactor_Poison", "받는 독 피해" },
            { "BleedChance", "출혈 확률" },
            { "ReloadSpeedGain", "재장전 속도" },
            { "EnergyCost", "배고픔 소모" },
            { "WaterCost", "수분 소모" },
            { "NightVisionAbility", "야간 시야" },
            { "ViewDistance", "시야 거리" },
            { "SenseRange", "감지 범위" },
            { "HearingAbility", "청력" },
            { "ElementFactor_Physics", "받는 물리 피해" }
        };

        public SkillSystem(Config config, MetaProgress meta)
        {
            _config = config;
            _meta = meta;
            foreach (SkillDef def in SkillDefs.All)
            {
                _xp[def.Id] = 0f;
                _tokens[def.Id] = new object();
            }
        }

        // ------------------------------------------------------------------
        // 레벨 / 경험치 계산
        // ------------------------------------------------------------------

        public int MaxLevel { get { return _config.MaxLevel; } }

        /// <summary>레벨 L 에 도달하기 위해 필요한 누적 경험치</summary>
        public float TotalXpForLevel(int level)
        {
            float total = 0f;
            for (int n = 1; n <= level; n++)
            {
                total += _config.XpBase + _config.XpStep * (n - 1);
            }
            return total;
        }

        public float GetXp(string id)
        {
            float value;
            return _xp.TryGetValue(id, out value) ? value : 0f;
        }

        /// <summary>경험치 -> 레벨 (다른 세이브 데이터를 읽을 때 쓰는 정적 버전)</summary>
        public static int LevelFromXp(float xp, Config config)
        {
            if (config == null)
            {
                return 0;
            }
            int level = 0;
            float total = 0f;
            for (int n = 1; n <= config.MaxLevel; n++)
            {
                total += config.XpBase + config.XpStep * (n - 1);
                if (xp >= total)
                {
                    level = n;
                }
                else
                {
                    break;
                }
            }
            return level;
        }

        /// <summary>저장 문자열("id:xp;...")에서 스킬 레벨 합계를 계산한다(다른 세이브용)</summary>
        public static int TotalLevelFromRaw(string raw, Config config)
        {
            if (string.IsNullOrEmpty(raw) || config == null)
            {
                return 0;
            }

            int sum = 0;
            foreach (string part in raw.Split(';'))
            {
                if (part.Length == 0)
                {
                    continue;
                }
                int index = part.IndexOf(':');
                if (index <= 0)
                {
                    continue;
                }
                float value;
                if (float.TryParse(part.Substring(index + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    sum += LevelFromXp(value, config);
                }
            }
            return sum;
        }

        public int GetLevel(string id)
        {
            float xp = GetXp(id);
            int level = 0;
            while (level < _config.MaxLevel && xp >= TotalXpForLevel(level + 1))
            {
                level++;
            }
            return level;
        }

        public bool IsElite(string id)
        {
            return GetLevel(id) >= _config.MaxLevel;
        }

        /// <summary>현재 레벨의 경험치 진행도(현재/필요)</summary>
        public void GetProgress(string id, out float current, out float needed)
        {
            int level = GetLevel(id);
            float from = TotalXpForLevel(level);
            float to = TotalXpForLevel(level + 1);
            float xp = GetXp(id);
            current = xp - from;
            needed = Mathf.Max(1f, to - from);
            if (level >= _config.MaxLevel)
            {
                current = needed;
            }
        }

        /// <summary>회복: 치료 속도 향상 비율 (0.2 = 사용 시간 20% 감소). 만렙 40% + 엘리트 10%</summary>
        public float HealSpeedBonus(int level)
        {
            if (level <= 0)
            {
                return 0f;
            }
            float bonus = Specials.HealSpeedPerLevel * level;
            if (level >= _config.MaxLevel)
            {
                bonus += Specials.HealSpeedElite;
            }
            return Mathf.Clamp(bonus, 0f, 0.8f);
        }

        /// <summary>지금 장소에서 이 스킬 경험치를 받을 수 있는지 판단한다.</summary>
        private bool IsAllowedHere(string id)
        {
            try
            {
                LevelManager manager = LevelManager.Instance;
                if (manager == null || !manager.IsBaseLevel)
                {
                    return true;   // 기지가 아니면 제한 없음
                }

                if (_hideoutAllowed == null)
                {
                    _hideoutAllowed = new HashSet<string>();
                    string list = _config != null ? _config.HideoutSkills : null;
                    if (!string.IsNullOrEmpty(list))
                    {
                        foreach (string token in list.Split(','))
                        {
                            string trimmed = token.Trim();
                            if (trimmed.Length > 0)
                            {
                                _hideoutAllowed.Add(trimmed);
                            }
                        }
                    }
                }

                if (_hideoutAllowed.Contains(id))
                {
                    return true;
                }
                if (_hideoutBlockedLogged.Add(id))
                {
                    Debug.Log("[Dskill] 기지에서는 '" + id + "' 경험치가 오르지 않습니다 (의도된 제한 — config.ini 의 hideout_skills 로 조정)");
                }
                return false;
            }
            catch (Exception)
            {
                return true;   // 판단에 실패하면 막지 않는다(경험치 손실 방지)
            }
        }

        /// <summary>경험치를 더한다. 레벨이 오르면 OnLevelUp 이벤트가 발생한다.</summary>
        public void AddXp(string id, float amount)
        {
            // 게임 이벤트 안에서 호출되므로, 어떤 예외도 게임으로 전파되지 않게 감싼다.
            try
            {
                if (amount <= 0f)
                {
                    return;
                }
                SkillDef def = SkillDefs.Find(id);
                if (def == null)
                {
                    return;
                }

                // 기지(하이드아웃)에서는 의도된 스킬만 오른다
                if (!IsAllowedHere(id))
                {
                    return;
                }

                float metaMultiplier = _meta != null ? _meta.XpMultiplier : 1f;
                float gain = amount * _config.MultiplierFor(id) * metaMultiplier * _config.XpStageMultiplier;
                if (gain <= 0f)
                {
                    return;
                }

                int before = GetLevel(id);
                float current = GetXp(id);
                float max = TotalXpForLevel(_config.MaxLevel);
                _xp[id] = Mathf.Min(max, current + gain);

                int after = GetLevel(id);
                if (after > before)
                {
                    OnLevelUp?.Invoke(id, after);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[Dskill] 경험치 처리 오류(" + id + "): " + e.Message);
            }
        }

        /// <summary>지금 레벨에서 받고 있는 효과를 사람이 읽을 수 있는 문장으로 만든다.
        ///  같은 레벨이면 캐시된 문장을 돌려준다(매 프레임 새로 만들지 않음).</summary>
        public string DescribeEffects(string id, int level)
        {
            int cachedLevel;
            string cached;
            if (_effectTextLevel.TryGetValue(id, out cachedLevel) && cachedLevel == level &&
                _effectTextCache.TryGetValue(id, out cached))
            {
                return cached;
            }

            string text = BuildEffectText(id, level);
            _effectTextLevel[id] = level;
            _effectTextCache[id] = text;
            return text;
        }

        /// <summary>효과 문장을 실제로 만든다.</summary>
        private string BuildEffectText(string id, int level)
        {
            SkillDef def = SkillDefs.Find(id);
            if (def == null)
            {
                return "";
            }

            // 특수 스킬 (생존술 / 수리 / 흥정 / 투척 / 파밍 / 구르기 / 하이드아웃 / 제작 / 회복)
            if (id == "health")
            {
                // 0.0.6: 치료 '효율' 대신 '치료 속도'(사용 시간 감소)로 표시
                return Locale.F("eff.healSpeed", "치료 속도 +{0}%", (HealSpeedBonus(level) * 100f).ToString("0.#"));
            }
            if (id == "survival")
            {
                // 받는 화염·독 피해는 SkillDefs 의 효과 값을 그대로 쓴다(하드코딩하지 않음)
                float elementPerLevel = 0f;
                foreach (EffectDef elementEffect in def.Effects)
                {
                    if (elementEffect.Key == "ElementFactor_Fire")
                    {
                        elementPerLevel = elementEffect.PerLevel;
                        break;
                    }
                }
                string text = Locale.F("eff.survival", "디버프 저항 {0}%, 받는 화염·독 피해 -{1}%",
                    (SurvivalDebuffResist(level) * 100f).ToString("0.#"),
                    (elementPerLevel * level * 100f).ToString("0.#"));
                if (level >= _config.MaxLevel)
                {
                    text += Locale.T("eff.survivalElite", ", 출혈 면역");
                }
                return text;
            }

            if (def.Effects.Length == 0)
            {
                if (id == "repair")
                {
                    float reduction = RepairLossReduction(level);
                    return Locale.F("eff.repair", "최대 내구도 감소 -{0}%", (reduction * 100f).ToString("0.#"));
                }
                if (id == "barter")
                {
                    return Locale.F("eff.barter", "판매 금액 +{0}%", (BarterBonus(level) * 100f).ToString("0.#"));
                }
                if (id == "throwing")
                {
                    return Locale.F("eff.throwing", "투척 거리 +{0}%, 폭발 대미지 +{1}%",
                        (ThrowingRangeBonus(level) * 100f).ToString("0.#"),
                        (ThrowingDamageBonus(level) * 100f).ToString("0.#"));
                }
                if (id == "looting")
                {
                    return Locale.F("eff.looting", "아이템 감지 시간 -{0}%", ((1f - LootingTimeFactor(level)) * 100f).ToString("0.#"));
                }
                if (id == "dash")
                {
                    float reduction = DashReduction(level) * 100f;
                    return Locale.F("eff.dash", "쿨타임 -{0}%, 스태미나 소모 -{1}%",
                        reduction.ToString("0.#"), reduction.ToString("0.#"));
                }
                if (id == "hideout")
                {
                    string text = Locale.F("eff.hideout", "상인 쿨타임 -{0}%", (MerchantCooldownReduction(level) * 100f).ToString("0.#"));
                    if (level >= _config.MaxLevel)
                    {
                        text += Locale.F("eff.hideoutElite", ", 채굴 시간 -{0}%", (Specials.MinerEliteTimeReduction * 100f).ToString("0"));
                    }
                    return text;
                }
                if (id == "crafting")
                {
                    return Locale.F("eff.crafting", "추가 생산 확률 {0}%", (CraftingBonusChance(level) * 100f).ToString("0.#"));
                }
                return "";
            }

            StringBuilder sb = new StringBuilder();
            List<string> parts = new List<string>();
            foreach (EffectDef effect in def.Effects)
            {
                AddPart(parts, FormatEffect(effect, effect.PerLevel * level));
            }

            if (level >= _config.MaxLevel && def.EliteEffects.Length > 0)
            {
                foreach (EffectDef effect in def.EliteEffects)
                {
                    AddPart(parts, FormatEffect(effect, effect.PerLevel));
                }
            }

            foreach (string part in parts)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(part);
            }
            return sb.ToString();
        }

        /// <summary>같은 효과(예: 보행+달리기)가 중복 표시되지 않게 한 번만 넣는다.</summary>
        private static void AddPart(List<string> parts, string text)
        {
            if (!parts.Contains(text))
            {
                parts.Add(text);
            }
        }

        private static string FormatEffect(EffectDef effect, float value)
        {
            string korean;
            if (!StatNamesKo.TryGetValue(effect.Key, out korean))
            {
                korean = effect.Key;
            }
            // 스탯 이름은 언어별 표를 거친다 (없으면 위의 한국어 이름)
            string name = Locale.Stat(effect.Key, korean);

            string amount;
            if (effect.Kind == StatModKind.Add)
            {
                amount = (value >= 0f ? "+" : "") + value.ToString("0.##");
            }
            else
            {
                amount = (value >= 0f ? "+" : "") + (value * 100f).ToString("0.#") + "%";
            }
            return Locale.F("eff.value", "{0} {1}", name, amount);
        }

        /// <summary>수리 스킬의 "최대 내구도 감소" 경감 비율 (0.5 = 50% 감소)</summary>
        public float RepairLossReduction(int level)
        {
            if (level >= _config.MaxLevel)
            {
                return 1f;   // 엘리트: 내구도 감소 없음
            }
            return Specials.RepairLossPerLevel * level;      // 만렙 직전 50%
        }

        /// <summary>흥정 스킬의 판매 금액 보너스 비율 (0.1 = +10%)</summary>
        public float BarterBonus(int level)
        {
            return Specials.BarterBonusPerLevel * level;     // 만렙 +10%
        }

        /// <summary>투척술: 투척 거리 증가 비율 (0.3 = 30% 더 멀리)</summary>
        public float ThrowingRangeBonus(int level)
        {
            if (level <= 0)
            {
                return 0f;
            }
            float bonus = Specials.ThrowingRangePerLevel * level;
            if (level >= _config.MaxLevel)
            {
                bonus += Specials.ThrowingEliteRange;        // 엘리트: +20% 추가
            }
            return bonus;
        }

        /// <summary>투척술: 폭발 대미지 증가 비율 (0.5 = +50%)</summary>
        public float ThrowingDamageBonus(int level)
        {
            return Specials.ThrowingDamagePerLevel * level;
        }

        /// <summary>파밍: 아이템 감지에 걸리는 시간 비율 (0.5 = 절반 시간)</summary>
        public float LootingTimeFactor(int level)
        {
            float factor = 1f - Specials.LootingSpeedPerLevel * level;
            return Mathf.Clamp(factor, 0.05f, 1f);
        }

        /// <summary>구르기: 쿨타임·스태미나 소모 감소 비율 (0.5 = 절반)</summary>
        public float DashReduction(int level)
        {
            if (level <= 0)
            {
                return 0f;
            }
            float value = Specials.DashReductionPerLevel * level;
            if (level >= _config.MaxLevel)
            {
                value += Specials.DashEliteReduction;      // 엘리트: -20% 추가
            }
            return Mathf.Clamp(value, 0f, 0.95f);
        }

        /// <summary>하이드아웃: 상인(상점) 재고 갱신 쿨타임 감소 비율 (0.5 = 절반)</summary>
        public float MerchantCooldownReduction(int level)
        {
            return Mathf.Clamp(Specials.MerchantCooldownPerLevel * level, 0f, 0.9f);
        }

        /// <summary>하이드아웃 엘리트: 비트코인 채굴 시간 감소 비율 (0.2 = 20% 단축)</summary>
        public float MinerTimeReduction(int level)
        {
            if (level < _config.MaxLevel)
            {
                return 0f;
            }
            return Specials.MinerEliteTimeReduction;
        }

        /// <summary>제작: 추가 생산 확률 (0.3 = 30%)</summary>
        public float CraftingBonusChance(int level)
        {
            if (level <= 0)
            {
                return 0f;
            }
            float chance = Specials.CraftingBonusPerLevel * level;
            if (level >= _config.MaxLevel)
            {
                chance += Specials.CraftingEliteBonus;
            }
            return Mathf.Clamp(chance, 0f, 0.9f);
        }

        /// <summary>생존술: 디버프(출혈·중독 등) 저항 확률 (0.3 = 30%)</summary>
        public float SurvivalDebuffResist(int level)
        {
            return Mathf.Clamp(Specials.SurvivalDebuffResistPerLevel * level, 0f, 0.9f);
        }

        // ------------------------------------------------------------------
        // 스탯 보너스 적용
        // ------------------------------------------------------------------

        /// <summary>캐릭터 아이템에 지금 레벨의 보너스를 적용한다.</summary>
        public void ApplyTo(Item characterItem)
        {
            if (characterItem == null)
            {
                return;
            }
            RemoveFrom(_appliedItem);
            _appliedItem = characterItem;

            foreach (SkillDef def in SkillDefs.All)
            {
                int level = GetLevel(def.Id);
                if (level <= 0)
                {
                    continue;
                }
                ApplySkill(def, characterItem, level);
            }
            ApplyRepairLoss(characterItem);
        }

        /// <summary>아직 적용되지 않았으면 보너스를 적용한다.</summary>
        public void EnsureApplied(Item characterItem)
        {
            if (characterItem == null || _appliedItem == characterItem)
            {
                return;
            }
            ApplyTo(characterItem);
        }

        /// <summary>게임에 없는 스탯 키는 한 번만 경고한다(로그 확인용).</summary>
        private void WarnOnce(string key)
        {
            if (_warned.Add(key))
            {
                Debug.LogWarning("[Dskill] 이 캐릭터에 없는 스탯이라 효과가 적용되지 않습니다: " + key);
            }
        }

        private void ApplySkill(SkillDef def, Item item, int level)
        {
            object token = _tokens[def.Id];
            foreach (EffectDef effect in def.Effects)
            {
                if (!item.AddModifier(effect.Key, new Modifier(ToModifierType(effect.Kind), effect.PerLevel * level, token)))
                {
                    WarnOnce(effect.Key);
                }
            }

            if (level >= _config.MaxLevel)
            {
                foreach (EffectDef effect in def.EliteEffects)
                {
                    if (!item.AddModifier(effect.Key, new Modifier(ToModifierType(effect.Kind), effect.PerLevel, token)))
                    {
                        WarnOnce(effect.Key);
                    }
                }
            }
        }

        /// <summary>적용 중인 보너스를 모두 제거한다.</summary>
        public void RemoveFrom(Item characterItem)
        {
            if (characterItem == null)
            {
                return;
            }
            foreach (SkillDef def in SkillDefs.All)
            {
                characterItem.RemoveAllModifiersFrom(_tokens[def.Id]);
            }
            if (_appliedItem == characterItem)
            {
                _appliedItem = null;
            }
        }

        /// <summary>모드를 끌 때 호출: 수리 계수를 원래대로 돌리고 보너스를 제거한다.</summary>
        public void Detach()
        {
            if (_appliedItem != null)
            {
                RestoreRepairLoss(_appliedItem);
                RemoveFrom(_appliedItem);
            }
        }

        private static ModifierType ToModifierType(StatModKind kind)
        {
            switch (kind)
            {
                case StatModKind.PercentAdd:
                    return ModifierType.PercentageAdd;
                case StatModKind.PercentMultiply:
                    return ModifierType.PercentageMultiply;
                default:
                    return ModifierType.Add;
            }
        }

        /// <summary>수리 스킬: 게임은 수리 손실 계수를 'Constants' 에서 읽으므로 직접 값을 넣어 준다.</summary>
        private void ApplyRepairLoss(Item item)
        {
            float reduction = RepairLossReduction(GetLevel("repair"));
            if (reduction <= 0f)
            {
                if (!_repairLossSaved)
                {
                    return;   // 스킬 레벨이 없으면 게임 값에 아예 손대지 않는다(세이브·다른 모드 보호)
                }
                RestoreRepairLoss(item);
                return;
            }

            // 아이템이 바뀌면(다른 캐릭터·다른 세이브) 이전에 읽어 둔 원래 값을 쓸 수 없다
            if (_repairLossSaved && _repairLossItem != item)
            {
                _repairLossSaved = false;
            }

            if (!_repairLossSaved)
            {
                _originalWeaponRepairLoss = item.GetFloat("WeaponRepairLossFactor", 1f);
                _originalEquipmentRepairLoss = item.GetFloat("EquipmentRepairLossFactor", 1f);
                _repairLossItem = item;
                _repairLossSaved = true;
            }

            item.SetFloat("WeaponRepairLossFactor", Mathf.Max(0f, _originalWeaponRepairLoss * (1f - reduction)));
            item.SetFloat("EquipmentRepairLossFactor", Mathf.Max(0f, _originalEquipmentRepairLoss * (1f - reduction)));
        }

        private void RestoreRepairLoss(Item item)
        {
            if (!_repairLossSaved)
            {
                return;
            }
            item.SetFloat("WeaponRepairLossFactor", _originalWeaponRepairLoss);
            item.SetFloat("EquipmentRepairLossFactor", _originalEquipmentRepairLoss);
            // 보너스를 다시 적용할 때 원래 값을 새로 읽도록 초기화한다
            _repairLossSaved = false;
            _repairLossItem = null;
        }

        // ------------------------------------------------------------------
        // 저장 / 불러오기 (게임 세이브 슬롯에 저장)
        // ------------------------------------------------------------------

        public void Load()
        {
            foreach (SkillDef def in SkillDefs.All)
            {
                _xp[def.Id] = 0f;
            }

            try
            {
                string raw = null;
                if (SavesSystem.KeyExisits(SaveKey))
                {
                    raw = SavesSystem.Load<string>(SaveKey);
                }
                else if (SavesSystem.KeyExisits(LegacySaveKey))
                {
                    // 모드 이름을 바꾸기 전에 쌓은 데이터를 그대로 이어받는다
                    raw = SavesSystem.Load<string>(LegacySaveKey);
                    Debug.Log("[Dskill] 이전 버전의 스킬 데이터를 이어받았습니다.");
                }

                if (string.IsNullOrEmpty(raw))
                {
                    return;
                }

                foreach (string part in raw.Split(';'))
                {
                    if (part.Length == 0)
                    {
                        continue;
                    }
                    int index = part.IndexOf(':');
                    if (index <= 0)
                    {
                        continue;
                    }
                    string id = part.Substring(0, index);
                    if (!_xp.ContainsKey(id))
                    {
                        continue;
                    }
                    float value;
                    if (float.TryParse(part.Substring(index + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                    {
                        _xp[id] = value;
                    }
                }
                Debug.Log("[Dskill] 스킬 데이터를 불러왔습니다.");
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 스킬 데이터 불러오기 실패: " + e.Message);
            }
        }

        public void Save()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (SkillDef def in SkillDefs.All)
                {
                    sb.Append(def.Id).Append(':')
                      .Append(GetXp(def.Id).ToString("0.##", CultureInfo.InvariantCulture))
                      .Append(';');
                }
                SavesSystem.Save(SaveKey, sb.ToString());
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 스킬 데이터 저장 실패: " + e.Message);
            }
        }

        /// <summary>모든 스킬 경험치를 0으로 초기화하고 즉시 저장한다(config.ini 의 reset_skills 요청용).</summary>
        public void ResetAll()
        {
            foreach (SkillDef def in SkillDefs.All)
            {
                _xp[def.Id] = 0f;
            }
            Save();
            Debug.Log("[Dskill] 모든 스킬 경험치를 0으로 초기화했습니다.");
        }

    }
}
