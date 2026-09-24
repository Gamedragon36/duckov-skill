# Duckov Skill 0.0.4 — 제작 경험치 급등 수정 · 밸런스 정리 / Crafting XP fix and balance

## 한국어

### 밸런스 정리 (오마주와 실수 구분)

- **신진대사의 레벨업 속도(음식·물을 먹을 때 오르는 것)는 의도된 오마주**입니다. (타르코프의 Metabolism 스킬)
- 그런데 **이동 속도 보너스(+10%)는 잘못 들어간 것**이었습니다 → 원래 예정이었던 **지구력**으로 돌려놓았습니다.

| 스킬 | 이전 | 이후 |
|---|---|---|
| **지구력** | 스태미나 소모 -40% | 스태미나 소모 -40% **+ 이동 속도 +10%** |
| **신진대사** | 배고픔·수분 소모 -30% **+ 이동 속도 +10%** | 배고픔·수분 소모 -30% (엘리트: 굶주림 피해 없음) |

- 이미 키운 스킬 레벨은 **그대로 유지**되며, 효과만 다음 실행 때 새로 적용됩니다.
- 스킬 창 표시·README(한/영)·10개 언어 성장 조건 문구도 모두 새 내용으로 갱신했습니다.

### 버그 수정 — 작업대 업그레이드 시 제작 스킬이 비정상적으로 급등

작업대를 업그레이드했을 때 제작 스킬이 한 번에 **Lv.8(4,800 XP)** 까지 오르는 문제가 있었습니다.
게임 이벤트 방식 2가지가 원인이었습니다.

| # | 원인 (게임 코드 확인) | 증상 | 수정 |
|---|---|---|---|
| 1 | `UnlockFormula` 가 **레시피 1개마다** `OnFormulaUnlocked` 를 호출 | 작업대 업그레이드 1회로 레시피 12개가 풀려 12 × 400 = **4,800점** | **한꺼번에 풀리는 해금은 1회만 인정** (기본 200점) |
| 2 | `Craft` 가 **결과물 1개마다** `OnItemCrafted` 를 호출 | 결과물이 여러 개인 제작은 XP 가 배수로 상승 | **1회 제작당 1번만** 지급 |
| 3 | (개선) 고정 200점 | 싼 재료로 만든 것과 비싼 재료로 만든 것이 같은 XP | **재료 가치에 비례** (`가치 × 8%`, 1회 최대 400점) |

### 새 설정 — `config.ini` 에서 원하는 값으로 조정

```ini
craft_unlock_xp = 200       # 새 레시피 해금 XP (한꺼번에 여러 개가 풀리면 1회만 인정)
craft_xp_per_value = 0.08   # 제작 1회 XP = 재료 가치 × 이 값 (0.08 = 8%)
craft_xp_cap = 400          # 제작 1회 최대 XP (0 = 제한 없음)
```

- 계산 근거가 로그에 남습니다: `[Dskill] 제작 경험치 187 (재료 가치 2340 × 0.08)`
- 예시: 재료 가치 500 → 40점 / 2,000 → 160점 / 5,000 이상 → 상한 400점
- 값은 게임을 다시 시작하면 적용됩니다.

### 그 밖의 수정

- 제작 스킬 성장 조건 문구를 **10개 언어 모두** 새 규칙으로 갱신
- 내부 수정: 게임의 `CraftingFormula` 는 **구조체**라 null 비교가 불가능 (컴파일 오류 수정)

---

## English

### Balance housekeeping (what is an homage, what was a mistake)

- Metabolism's **level-up rate** (gaining XP from eating and drinking) is **intentional — an homage** to Tarkov's Metabolism skill.
- However the **movement-speed bonus (+10%) was a mistake**, so it has been moved back to **Endurance**, where it was originally planned.

| Skill | Before | After |
|---|---|---|
| **Endurance** | stamina drain -40% | stamina drain -40% **+ movement speed +10%** |
| **Metabolism** | hunger & water drain -30% **+ movement speed +10%** | hunger & water drain -30% (Elite: no starvation damage) |

- Skill levels you already earned are **kept** — only the effects are re-applied on the next launch.
- The skill window text, both READMEs and the growth-condition strings in all 10 languages were updated.

### Bug fix — Crafting skill jumped abnormally when upgrading a workbench

Upgrading a workbench could take the Crafting skill straight to **Lv.8 (4,800 XP)**. Two game-event behaviours caused it.

| # | Cause (from the game code) | Symptom | Fix |
|---|---|---|---|
| 1 | `UnlockFormula` invokes `OnFormulaUnlocked` **once per recipe** | One workbench upgrade unlocking 12 recipes = 12 × 400 = **4,800 pts** | A **batch unlock now counts once** (default 200 pts) |
| 2 | `Craft` invokes `OnItemCrafted` **once per produced item** | Multi-output crafts multiplied the XP | XP is awarded **once per craft** |
| 3 | (improvement) flat 200 pts | Cheap and expensive crafts gave the same XP | **Scales with the material value** (`value × 8%`, capped at 400 per craft) |

### New settings — tune them in `config.ini`

```ini
craft_unlock_xp = 200       # XP for a new recipe (a batch unlock counts once)
craft_xp_per_value = 0.08   # Crafting XP = material value x this value (0.08 = 8%)
craft_xp_cap = 400          # Maximum XP per craft (0 = no limit)
```

- The calculation is logged: `[Dskill] 제작 경험치 187 (재료 가치 2340 × 0.08)`
- Example: material value 500 → 40 pts / 2,000 → 160 pts / 5,000+ → capped at 400 pts
- Values apply after restarting the game.

### Other changes

- The Crafting growth-condition text was updated **in all 10 languages**
- Internal: the game's `CraftingFormula` is a **struct**, so null comparisons are invalid (compile error fixed)

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 0.0.3 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브·스킬 레벨 유지).
> If you are on 0.0.3, replacing `Dskill.dll` is enough — config, saves and skill levels are kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
