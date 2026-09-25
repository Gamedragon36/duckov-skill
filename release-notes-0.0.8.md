# Duckov Skill 0.0.8

## 🆕 추가
- **속성적응(Elemental Adaptation)** 스킬 추가 — 스킬 **24종**
  - 속성 피해(화염·독·전기·얼음·유령·우주)를 받으면 성장 (피해 1당 20점)
  - 만렙 각 속성 피해 **-10%**, ★엘리트 **-5% 추가** (합계 -15%)
- 스킬 창에 **경험치 정수(현재/필요)** 와 **모든 스킬의 레벨당 수치** 표시
- 스킬 창 제목을 `★ Duckov Skill` 로 정리

## ⚖️ 변경 (🔼 버프 / 🔽 너프 / 🔁 방식 변경)
| 스킬 | 변경 | 구분 |
|---|---|---|
| 파밍 | 아이템 감지 시간 **-100% → -70%** (레벨당 3.5% · 최소 0.2초 보장) | 🔽 **너프** |
| 정밀 사격 | 명중 1회 **45 → 150점**, 헤드샷 **+70 → +200점** | 🔼 **버프** |
| 근력 | 성장 임계 **70/90% → 60/80%** (더 가벼운 상태에서도 성장) | 🔼 **버프** |
| 수리 | 내구도 1당 **30 → 5점** | 🔽 **너프** |
| 회복 | 효과를 **치료 효율 → 치료 속도**(아이템 사용 시간 **-40%**)로 변경 | 🔁 방식 변경 |
| 구르기 | 쿨타임·**동작 시간**·스태미나 **-40%** (동작 시간 감소분은 이동 속도로 보정해 **이동 거리**를 유지) | 🔁 방식 변경 |
| 사격술 | 발사 시 경험치 삭제 → **적중 시에만 8점** | 🔁 방식 변경 |

## 🐞 버그 수정
- **속성적응**: 레벨 효과와 엘리트 수치가 **뒤바뀌어 적용**되던 것 수정
- **엘리트 파밍**: 아이템마다 확률을 굴려 사실상 항상 즉시 감지되던 것 → **상자 단위 1회 굴림**으로 수정
- **감지 시간**: 감소율이 -100% 일 때 0초가 되어 즉시 감지되던 것 → 최소 **0.2초** 보장
- **정밀 사격**: 20m 조건 때문에 경험치가 0에 머물던 문제 → 한 발 가치 상향 (위 표)
- **UI 문구**: 10개 언어가 이전 수치를 표시하던 것 모두 동기화
- **`info.ini`**: 줄바꿈이 손상(`CR CR LF`)되어 값에 불필요한 문자가 섞이던 것 정규화

## ⚠️ 사용자 고지
- **0.0.6 에서 경험치 증가가 과도해 밸런스가 무너졌습니다.** 제작자가 이를 인지하고, 세이브 실측
  (분당 610 XP · 기지 3,370 XP/시간)을 근거로 0.0.7 에서 **곡선 2.6배 + 요율 약 0.5배**로 수정했습니다.
- 정밀 사격의 **20m 기준은 의도된 설계**입니다 (저격용 — 멀리서 잡는 플레이를 보상).
- 곡선이 가팔라진 이후 **표시 레벨이 몇 단계 내려갈 수 있습니다** (정상).

- 물리 피해는 기존 **'방어' 스킬이 담당**하므로 속성적응에서는 제외했습니다 (중복 방지).

## 설치
1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기
3. **F6** 스킬 창 (제목줄 드래그로 이동) · **F7** 계열 전환

> 0.0.7 사용자는 `Dskill.dll` 만 교체하면 됩니다.


## AI 제작 표시
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다.

---

# English

## 🆕 Added
- **Elemental Adaptation** skill — **24 skills total**
  - Grows when you take elemental damage (fire/poison/electric/ice/ghost/space), 20 pts per damage
  - Max level: each element **-10%** · ★Elite: **-5% more** (-15% total)
- Skill window now shows **XP as numbers (current/needed)** and a **per-level value line for every skill**
- Skill window title cleaned up to `★ Duckov Skill`

## ⚖️ Changes (🔼 buff / 🔽 nerf / 🔁 rework)
| Skill | Change | Kind |
|---|---|---|
| Looting | detection time **-100% → -70%** (3.5%/level, 0.2 s floor) | 🔽 **nerf** |
| Marksmanship | per hit **45 → 150**, headshot **+70 → +200** | 🔼 **buff** |
| Strength | threshold **70/90% → 60/80%** (levels up while lighter) | 🔼 **buff** |
| Repair | **30 → 5** pts per durability restored | 🔽 **nerf** |
| Healing | effect changed to **healing speed** (item use time **-40%**) | 🔁 rework |
| Dash | cooldown, **action time** & stamina **-40%** (distance kept by compensating speed) | 🔁 rework |
| Assault | XP on firing removed → **only on hit, 8 pts** | 🔁 rework |

## 🐞 Fixes
- **Elemental Adaptation**: level and elite values were applied **swapped** — corrected
- **Elite looting**: rolled its chance per item (effectively always instant) → now rolled **once per container**
- **Detection time**: could become 0 s at -100% → **0.2 s floor** enforced
- **Marksmanship**: stayed at 0 XP because of the 20 m condition → per-hit value raised (see table)
- **UI text**: all 10 languages still showed old values → synced
- **`info.ini`**: mangled line endings (`CR CR LF`) put stray characters into values → normalized

## ⚠️ Author's disclosure
- **In 0.0.6 XP gain was far too high and the balance broke.** I confirmed this, measured it from save data
  (610 XP/min, 3,370 XP/h in the hideout) and fixed it in 0.0.7 (**curve 2.6x steeper, rates about half**).
- The **20 m requirement for Marksmanship is intentional** (sniping — it rewards long-range play).
- Because the curve is steeper now, **shown levels may drop a few steps** (expected).

- Physical damage is handled by the existing **Armor** skill, so it is excluded from Elemental Adaptation.

## Installation
1. Extract into `Duckov_Data\Mods\Dskill\`
2. Launch the game → enable **Duckov Skill** in the **Mods** menu
3. **F6** opens the skill window (drag the title bar) · **F7** switches category

> On 0.0.7? Replacing `Dskill.dll` is enough.


## AI disclosure
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
Design, balance decisions and in-game verification were done by the author.
