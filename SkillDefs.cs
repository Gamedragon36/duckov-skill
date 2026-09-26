using System;

namespace Dskill
{
    /// <summary>스탯에 값을 더하는 방식</summary>
    public enum StatModKind
    {
        /// <summary>그냥 더하기 (예: 최대 무게 +2)</summary>
        Add,
        /// <summary>퍼센트로 더하기</summary>
        PercentAdd,
        /// <summary>퍼센트로 곱하기 (예: -0.2 = 20% 감소)</summary>
        PercentMultiply
    }

    /// <summary>스킬이 올려주는 스탯 한 줄</summary>
    public class EffectDef
    {
        public readonly string Key;       // 게임 스탯 키 (예: MaxWeight)
        public readonly StatModKind Kind;
        public readonly float PerLevel;   // 레벨 1당 값

        public EffectDef(string key, StatModKind kind, float perLevel)
        {
            Key = key;
            Kind = kind;
            PerLevel = perLevel;
        }
    }

    /// <summary>스킬 하나의 정의</summary>
    public class SkillDef
    {
        public string Id;                 // 내부 이름(영문)
        public string NameKo;             // 표시 이름
        public string Icon;               // 아이콘(한글 폰트에서 안전한 특수문자만 사용)
        public string Category;           // 신체 / 전투 / 실용
        public string TriggerKo;          // 성장 조건 설명
        public string EliteKo;            // 엘리트(만렙) 효과 설명
        public EffectDef[] Effects;       // 레벨당 효과
        public EffectDef[] EliteEffects;  // 만렙 도달 시 추가 효과
    }

    /// <summary>경험치 획득량 (게임에서 1단위 일어날 때 얻는 점수)</summary>
    public static class Rates
    {
        // 근력·지구력의 '초당' 성장은 2026-09-26 사용자 요청으로 **삭제**했다(거리 비례만 사용).
        public const float CovertPerSecond = 1f;            // 은신 이동: 걷기(초) — 초당 성장을 남겨둔 유일한 이동 스킬
        // ---- 이동 거리 비례 (0.0.9 · 2026-09-26 사용자 요청) ----
        //   Y축(낙하·점프) 제외 · 속도 버프를 그대로 반영(빠를수록 더 많이 얻음).
        //   근력·지구력은 '초당' 몫을 지우고 이 거리 비례만 쓴다.
        public const float EndurancePerMeter = 1.04f;       // 달리기 1m당 — 2026-09-26: 1.3배(0.8 → 1.04)
        public const float CovertPerMeter = 0.7f;           // 걷기 1m당 (은신 이동은 초당 1점 + 거리 몫)
        public const float StrengthPerMeter = 1.6f;         // 과적(무게 60% 이상) 1m당 — 80% 차등 없음
        /// <summary>한 틱에 이보다 많이 움직이면 순간이동으로 보고 거리에서 제외 (m). 틱이 0.5초라 20m/s 상당</summary>
        public const float MovementMaxStep = 10f;
        public const float VitalityPerDamage = 10f;         // 받은 피해
        public const float ArmorPerDamage = 12f;            // 방어구 착용 중 받은 피해
        public const float HealthPerHeal = 10f;             // 회복량 (0.0.7: 20 → 10)
        public const float AssaultPerHit = 8f;              // 적중 시 (0.0.6: 발사 XP 를 없애고 적중에만 지급)
        public const float RecoilPerShot = 2f;              // 발사 1발 (0.0.7: 3 → 2)
        public const float RecoilPerAdsShot = 4f;           // 조준 사격 1발 (0.0.7: 6 → 4)
        public const float MarksmanshipPerHit = 600f;       // 원거리 명중 — 2026-09-26: 사용자 요청 4배(150 → 600)
        public const float MarksmanshipPerCrit = 800f;       // 원거리 헤드샷 보너스 — 2026-09-26: 4배(200 → 800)
        public const float MarksmanshipMinDistance = 25f;   // 원거리 기준 거리(m) — 2026-09-26: 사용자 요청 20 → 25
        public const float MeleePerHit = 45f;               // 근접 명중 — 2026-09-26: 사용자 요청으로 1.5배(30 → 45)
        public const float MeleePerKill = 90f;              // 근접 처치 보너스 — 2026-09-26: 1.5배(60 → 90)
        public const float SurvivalPerDebuff = 300f;        // 상태이상 1회
        public const float SurvivalPerTickDamage = 12f;     // 지속 피해
        public const float ElementPerDamage = 20f;          // 받은 '속성' 피해 1당 (속성적응 — 화염·독·전기·얼음·유령·우주)
        public const float ReloadPerComplete = 50f;         // 재장전 1회 — 2026-09-26: 사용자 요청 40 → 50
        public const float RepairPerDurability = 5f;        // 수리한 내구도 1당 점수
        // 2026-09-25 확인·수정: 30 → 5.  30 이면 **수리 1회(내구도 30 회복)에 900점**이라
        //   Lv.1 필요치(500)를 한 번에 넘고 만렙(누적 48,000)까지 53회면 끝났습니다(비정상적으로 빠름).
        //   5 로 낮추면 같은 수리가 150점 → 만렙까지 320회(내구도 9,600)로 다른 스킬과 비슷해집니다.
        public const float BarterPer1000 = 10f;             // 거래 금액 1,000당 — 2026-09-26: 사용자 요청 5 → 10
        public const float MetabolismPerPoint = 20f;        // 포만감/수분 회복량
        public const float ThrowingPerThrow = 100f;         // 폭발물 투척 1회 (0.0.7: 200 → 100)
        public const float ThrowingPerExplosionDamage = 2f; // 폭발 피해 1당
        public const float LootingPerItem = 20f;            // 아이템 감지 완료 1개 (0.0.7: 40 → 20)
        public const float LootingPerPickup = 10f;          // 아이템 획득 1개 (0.0.7: 15 → 10)
        public const float DashPerUse = 6f;                 // 구르기 1회 — 2026-09-26: 사용자 요청 4 → 6
        public const float HideoutPerSecond = 0.5f;         // 기지(하이드아웃) 체류(초) (0.0.7: 1 → 0.5)
        public const float HideoutPerBuilding = 500f;       // 건물 신축·업그레이드 1회
        public const float FishingPerCatch = 250f;          // 물고기 1마리
        public const float PerceptionPerSound = 12f;        // 소리 감지 1회 — 2026-09-26 사용자 요청 B안(4 → 12, 캡 0.5초)
        public const float NightVisionPerSecond = 2f;       // 밤 시간(초)
        public const float CraftingPerCraft = 200f;         // 제작 완료 1회
        public const float CraftingPerUnlock = 400f;        // 새 레시피 해금 1회

        /// <summary>이 값보다 크게 늘어나야 '음식을 먹은 것'으로 인정 (기지에서 서서 회복하는 것 방지)</summary>
        public const float MetabolismMinDelta = 1f;
    }

    /// <summary>특수 스킬(수리·흥정·투척·파밍)의 수치. 여기 값만 바꾸면 밸런스가 조절됩니다.</summary>
    public static class Specials
    {
        public const float RepairLossPerLevel = 0.025f;     // 수리: 레벨당 최대 내구도 감소 경감 (만렙 -50%)
        public const float BarterBonusPerLevel = 0.005f;    // 흥정: 레벨당 판매 보너스 (만렙 +10%)
        public const float ThrowingRangePerLevel = 0.015f;  // 투척: 레벨당 거리 증가 (만렙 +30%)
        public const float ThrowingEliteRange = 0.20f;      // 투척 엘리트: 거리 +20%
        public const float ThrowingDamagePerLevel = 0.025f; // 투척: 레벨당 폭발 대미지 (만렙 +50%)
        // 2026-09-26 사용자 요청: 기존 '30% 확률 즉시 폭발'(ThrowingInstantChance)을 폐지하고
        //   ★엘리트 = **지면에 닿으면 100% 즉시 폭발**(신관 지연 0)로 변경했다.
        public const float LootingSpeedPerLevel = 0.035f;   // 파밍: 레벨당 감지 시간 감소 (만렙 -70%) — 2026-09-25: 0.05 → 0.035 (사용자 요청: -70%)
        public const float LootingMinInspectTime = 0.2f;    // 파밍: 만렙(-70%)에서도 0초가 되지 않게 유지하는 최소 감지 시간(초)
        public const float LootingInstantChance = 0.50f;    // 파밍 엘리트: 즉시 감지 확률
        /// <summary>사격술: 레벨당 조준(ADS) 시간 감소 (만렙 -50%).
        ///  조준 시간은 총기(무기)가 읽는 스탯 `AdsTime` 이라 **장착한 총 아이템**에 적용한다 (2026-09-26 사용자 요청)</summary>
        public const float AssaultAdsTimePerLevel = 0.025f;
        public const float DashReductionPerLevel = 0.02f;  // 구르기: 레벨당 쿨타임·스태미나 감소 (만렙 -40%)
        public const float DashEliteReduction = 0.10f;      // 구르기 엘리트: 각각 -10% 추가
        public const float EnduranceStaminaPerLevel = 0.03f;  // 지구력: 레벨당 스태미나 소모 감소 (만렙 -60%) — 2026-09-26: 0.02 → 0.03
        public const float EnduranceStaminaMax = 0.60f;       // 지구력: 스태미나 소모 감소 상한 (-60%)
        public const float MerchantCooldownPerLevel = 0.025f;  // 하이드아웃: 상인 쿨타임 감소 (만렙 -50%)
        public const float MinerEliteTimeReduction = 0.20f;    // 하이드아웃 엘리트: 채굴 시간 -20%
        public const float CraftingBonusPerLevel = 0.015f;     // 제작: 레벨당 추가 생산 확률 (만렙 30%)
        public const float CraftingEliteBonus = 0.20f;         // 제작 엘리트: 확률 +20% (총 50%)
        public const float SurvivalDebuffResistPerLevel = 0.015f;  // 생존술: 디버프 저항 (만렙 30%)

        /// <summary>근접 전투: 레벨당 근접 공격속도 증가 (만렙 +30%) — CA_Attack 의 쿨타임·동작 시간을 줄인다 (0.0.8)</summary>
        public const float MeleeSpeedPerLevel = 0.015f;
        /// <summary>근접 전투 엘리트: 근접 공격속도 추가 +20%</summary>
        public const float MeleeSpeedElite = 0.20f;


        /// <summary>회복: 레벨당 치료 속도(사용 시간 감소) 비율. 만렙 40%</summary>
        public const float HealSpeedPerLevel = 0.02f;
        /// <summary>회복 엘리트: 치료 속도 추가 10%</summary>
        public const float HealSpeedElite = 0.10f;

        /// <summary>
        /// 경험치 획득 난이도(1~5단계) 배율. 게임 안 스킬 창에서 바꿀 수 있다.
        /// 5단계 = 1.0(지금까지의 기본 밸런스), 1단계 = 5.0(가장 빠름).
        /// 대략 스킬 1개 만렙까지: 5단계 약 5시간 / 1단계 약 1시간 (달리기 기준)
        /// </summary>
        public static readonly float[] XpStageMultipliers = { 5f, 2.5f, 1.5f, 1.2f, 1f };
    }

    /// <summary>스킬 정의 (2026-09-25 기준 24종: 신체 7 · 전투 6 · 실용 11)</summary>
    public static class SkillDefs
    {
        public static readonly SkillDef[] All =
        {
            // ---------------- 신체 ----------------
            new SkillDef
            {
                Id = "strength", NameKo = "근력", Icon = "◆", Category = "신체",
                TriggerKo = "무게 60% 이상으로 이동 (1m당 1.6점)",
                EliteKo = "가방 공간 +5칸, 이동 속도 +10%",
                Effects = new[]
                {
                    new EffectDef("MaxWeight", StatModKind.Add, 1.0f),                  // 만렙 +20
                    new EffectDef("WalkSpeed", StatModKind.PercentMultiply, 0.005f),    // 만렙 +10%
                    new EffectDef("RunSpeed", StatModKind.PercentMultiply, 0.005f)
                },
                EliteEffects = new[]
                {
                    new EffectDef("InventoryCapacity", StatModKind.Add, 5f),
                    new EffectDef("WalkSpeed", StatModKind.PercentMultiply, 0.10f),     // 엘리트 +10% 추가
                    new EffectDef("RunSpeed", StatModKind.PercentMultiply, 0.10f)
                }
            },
            new SkillDef
            {
                Id = "endurance", NameKo = "지구력", Icon = "▲", Category = "신체",
                TriggerKo = "달리기·구르기로 이동한 거리 (1m당 1.04점)",
                EliteKo = "스태미나 회복 속도 +30%",
                Effects = new[]
                {
                    new EffectDef("StaminaDrainRate", StatModKind.PercentMultiply, -Specials.EnduranceStaminaPerLevel),  // 만렙 -60% (구르기 중첩 계산과 같은 상수 공유)
                    // 2026-09-26 사용자 요청: 이동 속도(+10%)는 **근력**으로 옮겼다(근력에 이미 있음).
                },
                EliteEffects = new[]
                {
                    new EffectDef("StaminaRecoverRate", StatModKind.PercentMultiply, 0.30f)
                }
            },
            new SkillDef
            {
                Id = "vitality", NameKo = "생명력", Icon = "●", Category = "신체",
                TriggerKo = "피해를 받을 때 (피해 1당 10점)",
                EliteKo = "최대 생명력 +5",
                Effects = new[]
                {
                    new EffectDef("MaxHealth", StatModKind.Add, 1.0f)           // 만렙 +20
                },
                EliteEffects = new[]
                {
                    new EffectDef("MaxHealth", StatModKind.Add, 5f)
                }
            },
            new SkillDef
            {
                Id = "health", NameKo = "회복", Icon = "＋", Category = "신체",
                TriggerKo = "회복 아이템으로 체력을 채울 때 (회복량 1당 10점)",
                EliteKo = "치료 속도 +10% (추가)",
                Effects = new EffectDef[0],        // 특수 처리: 사용 시간 감소(치료 속도)
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "metabolism", NameKo = "신진대사", Icon = "◉", Category = "신체",
                TriggerKo = "음식·물로 배고픔/수분을 채울 때 (1당 20점)",
                EliteKo = "배고픔·수분이 0이어도 피해를 받지 않음",
                Effects = new[]
                {
                    new EffectDef("EnergyCost", StatModKind.PercentMultiply, -0.015f),   // 만렙 -30%
                    new EffectDef("WaterCost", StatModKind.PercentMultiply, -0.015f)     // 만렙 -30%
                    // 이동 속도 보너스는 지구력으로 옮겼습니다 (2026-09-24 밸런스 조정)
                },
                EliteEffects = new EffectDef[0]
            },

            new SkillDef
            {
                Id = "dash", NameKo = "구르기", Icon = "○", Category = "신체",
                TriggerKo = "구르기를 사용할 때 (1회 6점)",
                EliteKo = "쿨타임·동작 시간·스태미나 소모 각각 -10% 추가",
                Effects = new EffectDef[0],        // 특수 처리: 쿨타임 -30%, 스태미나 소모 -30%
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "nightvision", NameKo = "야간시야", Icon = "▼", Category = "신체",
                TriggerKo = "밤 시간에 활동할 때 (초당 2점)",
                EliteKo = "야간 시야 능력 +0.2 추가 (밤 패널티 완전 무효)",
                Effects = new[]
                {
                    new EffectDef("NightVisionAbility", StatModKind.Add, 0.015f)   // 만렙 +0.3
                },
                EliteEffects = new[]
                {
                    new EffectDef("NightVisionAbility", StatModKind.Add, 0.20f)
                }
            },

            // ---------------- 전투 ----------------
            new SkillDef
            {
                Id = "assault", NameKo = "사격술", Icon = "★", Category = "전투",
                TriggerKo = "총알이 적중했을 때만 (1회 8점)",
                EliteKo = "총기 치명타율 +10%",
                Effects = new[]
                {
                    new EffectDef("GunDamageMultiplier", StatModKind.PercentMultiply, 0.0075f)  // 만렙 +15%
                },
                EliteEffects = new[]
                {
                    new EffectDef("GunCritRateGain", StatModKind.PercentMultiply, 0.10f)
                }
            },
            new SkillDef
            {
                Id = "recoil", NameKo = "반동 제어", Icon = "▽", Category = "전투",
                TriggerKo = "총을 발사할 때 (1발 2점, 조준 사격은 4점)",
                EliteKo = "수직·수평 반동 -15%",
                Effects = new[]
                {
                    new EffectDef("RecoilControl", StatModKind.Add, 0.01f)      // 만렙 +0.2
                },
                EliteEffects = new[]
                {
                    new EffectDef("RecoilScaleV", StatModKind.PercentMultiply, -0.15f),
                    new EffectDef("RecoilScaleH", StatModKind.PercentMultiply, -0.15f)
                }
            },
            new SkillDef
            {
                Id = "marksmanship", NameKo = "정밀 사격", Icon = "◎", Category = "전투",
                TriggerKo = "25m 이상 거리에서 명중 (1회 600점, 헤드샷은 +800점)",
                EliteKo = "헤드샷 대미지 +10%",
                Effects = new[]
                {
                    new EffectDef("GunDistanceMultiplier", StatModKind.PercentMultiply, 0.015f),  // 만렙 +30%
                    new EffectDef("GunScatterMultiplier", StatModKind.PercentMultiply, -0.01f)    // 만렙 -20%
                },
                EliteEffects = new[]
                {
                    new EffectDef("GunCritDamageGain", StatModKind.PercentMultiply, 0.10f)
                }
            },
            new SkillDef
            {
                Id = "melee", NameKo = "근접 전투", Icon = "◀", Category = "전투",
                TriggerKo = "근접 공격이 명중할 때 (1회 45점, 근접 처치 +90점)",
                EliteKo = "이동 속도 +10%, 근접 공격속도 +20% (추가)",
                Effects = new[]
                {
                    new EffectDef("MeleeDamageMultiplier", StatModKind.PercentMultiply, 0.025f),  // 만렙 +50%
                    new EffectDef("MeleeCritRateGain", StatModKind.PercentMultiply, 0.025f),      // 만렙 +50%
                    new EffectDef("WalkSpeed", StatModKind.PercentMultiply, 0.005f),              // 만렙 +10%
                    new EffectDef("RunSpeed", StatModKind.PercentMultiply, 0.005f)
                },
                EliteEffects = new[]
                {
                    new EffectDef("WalkSpeed", StatModKind.PercentMultiply, 0.10f),
                    new EffectDef("RunSpeed", StatModKind.PercentMultiply, 0.10f)
                }
            },
            new SkillDef
            {
                Id = "throwing", NameKo = "투척술", Icon = "▶", Category = "전투",
                TriggerKo = "폭발물을 던질 때 (1회 100점, 폭발 피해 1당 2점)",
                EliteKo = "거리 +20% (추가), 지면에 닿으면 즉시 폭발",
                Effects = new EffectDef[0],        // 특수 처리: 투척 거리 +30%, 폭발 대미지 +50%
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "perception", NameKo = "인지/정찰", Icon = "◁", Category = "전투",
                TriggerKo = "주변 소리를 감지할 때 (1회 12점, 0.5초에 최대 1번)",
                EliteKo = "시야·감지 +10%, 청력 +20% 추가",
                Effects = new[]
                {
                    new EffectDef("ViewDistance", StatModKind.PercentMultiply, 0.01f),      // 만렙 +20%
                    new EffectDef("SenseRange", StatModKind.PercentMultiply, 0.01f),        // 만렙 +20%
                    new EffectDef("HearingAbility", StatModKind.PercentMultiply, 0.015f)    // 만렙 +30%
                },
                EliteEffects = new[]
                {
                    new EffectDef("ViewDistance", StatModKind.PercentMultiply, 0.10f),
                    new EffectDef("SenseRange", StatModKind.PercentMultiply, 0.10f),
                    new EffectDef("HearingAbility", StatModKind.PercentMultiply, 0.20f)
                }
            },

            // ---------------- 실용 ----------------
            new SkillDef
            {
                Id = "covert", NameKo = "은신 이동", Icon = "◐", Category = "실용",
                TriggerKo = "걸어서 이동할 때 (달리기 제외, 초당 1점 + 1m당 0.7점)",
                EliteKo = "보행·달리기 소리 -25% 추가 (총 -45% / -35%)",
                Effects = new[]
                {
                    new EffectDef("WalkSoundRange", StatModKind.PercentMultiply, -0.01f),   // 만렙 -20%
                    new EffectDef("RunSoundRange", StatModKind.PercentMultiply, -0.005f)   // 만렙 -10%
                },
                EliteEffects = new[]
                {
                    // 원래 "SoundRange -15%" 요청이었지만 그 값은 총기 쪽에서 읽혀서 캐릭터에 적용되지 않습니다.
                    // 대신 실제로 동작하는 보행/달리기 소리 범위를 추가로 줄입니다.
                    new EffectDef("WalkSoundRange", StatModKind.PercentMultiply, -0.25f),
                    new EffectDef("RunSoundRange", StatModKind.PercentMultiply, -0.25f)
                }
            },
            new SkillDef
            {
                Id = "armor", NameKo = "방어", Icon = "◇", Category = "실용",
                TriggerKo = "방어구를 착용한 채 피해를 받을 때 (피해 1당 12점)",
                EliteKo = "받는 물리 피해 -10%",
                Effects = new[]
                {
                    new EffectDef("BodyArmor", StatModKind.Add, 0.075f),   // 만렙 +1.5 — 2026-09-26: 사용자 요청 2.0 → 1.5
                    new EffectDef("HeadArmor", StatModKind.Add, 0.075f)    // 만렙 +1.5 — 위와 동일
                },
                EliteEffects = new[]
                {
                    // 원래 "방어구 내구도 소모 -20%" 요청이었지만, 그 값은 게임이 총알/장비 쪽에서 읽어서
                    // 캐릭터에게 적용할 수 없습니다. 대신 실제로 동작하는 "받는 물리 피해 감소"로 대체했습니다.
                    new EffectDef("ElementFactor_Physics", StatModKind.PercentMultiply, -0.10f)
                }
            },
            new SkillDef
            {
                Id = "elemental", NameKo = "속성적응", Icon = "⊙", Category = "실용",
                TriggerKo = "속성 피해(화염·독·전기·얼음·유령·우주)를 받을 때 (피해 1당 20점)",
                EliteKo = "속성 피해 -5% 추가 (각 속성 합계 -15%)",
                Effects = new[]
                {
                    // 만렙(20)까지 각 속성 피해 -10% = 레벨당 0.5%. (0.0.7 수정: 레벨/엘리트 수치가 뒤바뀌어 있었음)
                    // 물리(physics)는 '방어' 스킬이 담당하므로 여기서는 제외합니다(중복 방지).
                    new EffectDef("ElementFactor_Fire", StatModKind.PercentMultiply, -0.005f),
                    new EffectDef("ElementFactor_Poison", StatModKind.PercentMultiply, -0.005f),
                    new EffectDef("ElementFactor_Electricity", StatModKind.PercentMultiply, -0.005f),
                    new EffectDef("ElementFactor_Ice", StatModKind.PercentMultiply, -0.005f),
                    new EffectDef("ElementFactor_Ghost", StatModKind.PercentMultiply, -0.005f),
                    new EffectDef("ElementFactor_Space", StatModKind.PercentMultiply, -0.005f)
                },
                EliteEffects = new[]
                {
                    // 엘리트(만렙): 각 속성 -5% 추가 → 합계 -15%
                    new EffectDef("ElementFactor_Fire", StatModKind.PercentMultiply, -0.05f),
                    new EffectDef("ElementFactor_Poison", StatModKind.PercentMultiply, -0.05f),
                    new EffectDef("ElementFactor_Electricity", StatModKind.PercentMultiply, -0.05f),
                    new EffectDef("ElementFactor_Ice", StatModKind.PercentMultiply, -0.05f),
                    new EffectDef("ElementFactor_Ghost", StatModKind.PercentMultiply, -0.05f),
                    new EffectDef("ElementFactor_Space", StatModKind.PercentMultiply, -0.05f)
                }
            },
            new SkillDef
            {
                Id = "survival", NameKo = "생존술", Icon = "☆", Category = "실용",
                TriggerKo = "상태이상(출혈·중독·화상)에 걸릴 때 (1회 300점, 지속피해 1당 12점)",
                EliteKo = "출혈에 걸리지 않음 (디버프 30% 저항은 기본 효과)",
                Effects = new[]
                {
                    new EffectDef("ElementFactor_Fire", StatModKind.PercentMultiply, -0.005f),   // 만렙 -10%
                    new EffectDef("ElementFactor_Poison", StatModKind.PercentMultiply, -0.005f)  // 만렙 -10%
                },
                EliteEffects = new EffectDef[0]   // 출혈 면역은 코드로 처리(게임에 관련 스탯이 없음)
            },
            new SkillDef
            {
                Id = "reload", NameKo = "재장전", Icon = "◑", Category = "실용",
                TriggerKo = "재장전을 끝낼 때 (1회 50점)",
                EliteKo = "재장전 속도 +10% (추가)",
                Effects = new[]
                {
                    new EffectDef("ReloadSpeedGain", StatModKind.PercentMultiply, 0.015f)   // 만렙 +30%
                },
                EliteEffects = new[]
                {
                    new EffectDef("ReloadSpeedGain", StatModKind.PercentMultiply, 0.10f)
                }
            },
            new SkillDef
            {
                Id = "repair", NameKo = "수리", Icon = "◈", Category = "실용",
                TriggerKo = "장비를 수리할 때 (수리한 내구도 1당 5점)",
                EliteKo = "최대 내구도 감소 없음",
                Effects = new EffectDef[0],        // 특수 처리: 최대 내구도 감소 -50%
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "barter", NameKo = "흥정", Icon = "※", Category = "실용",
                TriggerKo = "아이템을 사고팔 때 (거래 금액 1,000당 10점)",
                EliteKo = "암시장 갱신 쿨타임 -50%",
                Effects = new EffectDef[0],        // 특수 처리: 판매 금액 보너스
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "looting", NameKo = "파밍", Icon = "▣", Category = "실용",
                TriggerKo = "상자·시체에서 아이템을 찾아낼 때 (1개 20점, 획득 1개 10점)",
                EliteKo = "상자·시체를 열면 50% 확률로 그 안의 아이템을 모두 즉시 감지",
                Effects = new EffectDef[0],        // 특수 처리: 아이템 감지 시간 -70% (레벨당 3.5%)
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "hideout", NameKo = "하이드아웃", Icon = "■", Category = "실용",
                TriggerKo = "기지 체류(초당 0.5점) + 건물 신축·업그레이드 500점",
                EliteKo = "비트코인 채굴기 생산 시간 -20%",
                Effects = new EffectDef[0],        // 특수 처리: 상인 쿨타임 -50%
                EliteEffects = new EffectDef[0]
            },
            new SkillDef
            {
                Id = "fishing", NameKo = "낚시", Icon = "▪", Category = "실용",
                TriggerKo = "물고기를 낚아서 가질 때 (1마리 250점)",
                EliteKo = "낚시 능력·운 각각 +20% 추가",
                Effects = new[]
                {
                    new EffectDef("FishingTime", StatModKind.PercentMultiply, 0.015f),           // 만렙 +30%
                    new EffectDef("FishingQualityFactor", StatModKind.PercentMultiply, 0.01f)    // 만렙 +20%
                },
                EliteEffects = new[]
                {
                    new EffectDef("FishingTime", StatModKind.PercentMultiply, 0.20f),
                    new EffectDef("FishingQualityFactor", StatModKind.PercentMultiply, 0.20f)
                }
            },
            new SkillDef
            {
                Id = "crafting", NameKo = "제작", Icon = "□", Category = "실용",
                TriggerKo = "제작 완료 (재료 가치 × 10%) · 새 레시피 해금 (200점)",
                EliteKo = "추가 생산 확률 +20% (총 50%)",
                Effects = new EffectDef[0],        // 특수 처리: 30% 확률로 결과물 1개 추가
                EliteEffects = new EffectDef[0]
            },
        };

        public static SkillDef Find(string id)
        {
            if (_byId == null)
            {
                _byId = new System.Collections.Generic.Dictionary<string, SkillDef>();
                foreach (SkillDef def in All)
                {
                    _byId[def.Id] = def;
                }
            }
            SkillDef found;
            return _byId.TryGetValue(id, out found) ? found : null;
        }

        /// <summary>스킬 id -> 정의 (첫 호출 때 한 번만 만든다)</summary>
        private static System.Collections.Generic.Dictionary<string, SkillDef> _byId;

        public static readonly string[] Categories = { "신체", "전투", "실용" };
    }
}
