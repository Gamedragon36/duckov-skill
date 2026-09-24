# Duckov Skill 0.0.6 — 경험치 감지 수정 · 회복/구르기 개편 / XP detection fixes, healing & dash rework

## 한국어

### 🐞 경험치가 오르지 않던 문제 (원인은 '감지'였습니다)

| # | 증상 | 원인 | 수정 |
|---|---|---|---|
| 1 | **장비를 수리해도 수리 XP 0** | ① 장착 장비는 `Inventory` 가 아니라 **슬롯(Slots)** 에 있고 ② 기지 수리는 **창고 아이템**을 대상으로 함 | **슬롯 + 창고**까지 감지(창고는 2초 간격) |
| 2 | **투척 XP 0** (던져도 레벨 그대로) | 수류탄 검사 함수에 `레벨 0이면 검사 안 함` 이 있어 **첫 경험치를 영영 못 받는 자기잠금** | 레벨 0에서도 검사 + 확인 주기 2초→1초 |
| 3 | **파밍 XP 0** (상자에서 못 찾을 때) | 위와 같은 자기잠금 | 레벨 0에서도 검사 |

### ⚖️ 밸런스 변경 (요청 반영)

| 항목 | 이전 | 이후 |
|---|---|---|
| **근력 · 지구력** 증가 속도 | 기준 | **2배** |
| **사격술** 성장 조건 | 발사할 때마다 (1발 4점) + 적중 4점 | **총알이 적중했을 때만 8점** (발사로는 오르지 않음) |
| **회복** 효과 | 치료 효율(회복량) +40% | **치료 속도(사용 시간) -40%** + 엘리트 -10% |
| **구르기** 감소 | 쿨타임·스태미나 -30% | **쿨타임·동작 시간·스태미나 -40%** + 엘리트 -10% |

### 🏃 구르기 상세 (요청: 쿨타임과 동작 시간을 함께, 거리는 유지)

- 쿨타임 / **동작 시간** / 스태미나를 **같은 비율**로 감소
- 동작 시간이 줄어도 **이동 거리는 그대로**: 속도(DashSpeed)를 반대로 보정
  - 예) Lv.12(-24%) → 쿨타임 0.38초 / 동작 0.38초 / 속도 +32% → **거리 동일**, 연타 주기 1.0 → **0.76초**
- 로그로 실측값을 확인할 수 있습니다:
  `[Dskill] 구르기 적용 Lv.12 : 쿨타임 0.5 → 0.38초 / 동작 0.5 → 0.38초 (쿨타임·스태미나 -24% / 동작 -24%)`
  `[Dskill] 구르기 사용 감지: 이전 사용과 0.76초 간격`

### 참고
- 레이드에서는 모든 스킬이 정상적으로 오릅니다 (기지 제한은 `hideout_skills` 로 조정)
- 이미 키운 스킬 레벨·세이브·설정은 그대로 유지됩니다 — `Dskill.dll` 만 교체

---

## English

### 🐞 Skills that never gained XP (the root cause was *detection*)

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 1 | **Repair XP stayed 0** | ① equipped gear lives in **Slots**, not `Inventory` ② base repairs target **stash items** | Now scans **slots + stash** (stash every 2 s) |
| 2 | **Throwables XP stayed 0** | The grenade scan had `if level <= 0 return` → **self-locking**: the first XP could never be earned | Scans at level 0 too; check interval 2 s → 1 s |
| 3 | **Scavenging XP stayed 0** (container finds) | Same self-locking gate | Scans at level 0 too |

### ⚖️ Balance changes (from your requests)

| Item | Before | After |
|---|---|---|
| **Strength · Endurance** gain rate | baseline | **2x** |
| **Rifleman** growth | every shot (4 pts) + 4 on hit | **only when your bullet hits — 8 pts** |
| **Healing** effect | healing amount +40% | **healing speed (use time) -40%** + Elite -10% |
| **Roll** reduction | cooldown & stamina -30% | **cooldown, action time & stamina -40%** + Elite -10% |

### 🏃 Roll details (cooldown + action time together, distance preserved)

- Cooldown / **action time** / stamina are reduced by the **same rate**
- Shorter action time would shorten the roll, so the **speed (DashSpeed) is compensated** to keep the distance
  - e.g. Lv.12 (-24%) → cooldown 0.38 s / action 0.38 s / speed +32% → **same distance**, spam interval 1.0 s → **0.76 s**
- The real values are logged for verification:
  `[Dskill] 구르기 적용 Lv.12 : 쿨타임 0.5 → 0.38초 / 동작 0.5 → 0.38초 (쿨타임·스태미나 -24% / 동작 -24%)`
  `[Dskill] 구르기 사용 감지: 이전 사용과 0.76초 간격`

### Notes
- In raids every skill gains XP normally (the hideout limit is configurable via `hideout_skills`)
- Your skill levels, saves and settings are kept — replacing `Dskill.dll` is enough

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 0.0.5 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브·스킬 레벨 유지).
> If you are on 0.0.5, replacing `Dskill.dll` is enough — config, saves and skill levels are kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
