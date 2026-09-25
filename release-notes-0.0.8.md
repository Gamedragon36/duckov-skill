# Duckov Skill 0.0.8

> 0.0.7 과 비교해 **바뀐 것만** 적었습니다.

## 🆕 추가
- **속성적응(Elemental Adaptation)** 스킬 — 스킬 **24종**
  - 속성 피해(화염·독·전기·얼음·유령·우주)를 받으면 성장 (피해 1당 20점)
  - 만렙 각 속성 피해 **-10%**, ★엘리트 **-5% 추가** (합계 -15%)
- **근접 전투**에 **근접 공격속도** 효과 — 만렙 **+30%**, ★엘리트 **+20% 추가** (🔼 버프)
- 스킬 창에 **경험치 정수(현재/필요)** 와 **모든 스킬의 레벨당 수치**를 표시

## ⚖️ 변경 (직전 0.0.7 대비)
| 스킬 | 변경 | 구분 |
|---|---|---|
| 파밍 | 아이템 감지 시간 감소 **-100% → -70%** (감지가 느려짐) | 🔽 **너프** |
| 근력 | 성장 임계 **70/90% → 60/80%** (더 가벼운 상태에서도 성장) | 🔼 **버프** |
| 수리 | 내구도 1당 **30 → 5점** | 🔽 **너프** |

## 🐞 버그 수정
- **속성적응**: 레벨 효과와 엘리트 수치가 **뒤바뀌어 적용**되던 것
- **엘리트 파밍**: 아이템마다 확률을 굴려 사실상 항상 즉시 감지되던 것 → **상자 단위 1회 굴림**
- **감지 시간**: -100% 일 때 0초가 되어 즉시 감지되던 것 → 최소 **0.2초** 보장 (감지가 느려지는 방향)
- **UI 문구**: 10개 언어가 이전 수치를 표시하던 것 모두 동기화
- **`info.ini`**: 줄바꿈이 손상(`CR CR LF`)되어 값에 불필요한 문자가 섞이던 것 정규화

## ⚠️ 사용자 고지 (0.0.8 신규)
- 물리 피해는 기존 **'방어' 스킬이 담당**하므로 속성적응에서는 제외했습니다 (중복 방지).

## 설치
압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣고, 게임 **모드(Mods)** 메뉴에서 **Duckov Skill** 을 켜세요. **F6** 스킬 창 · **F7** 계열 전환.

## AI 제작 표시
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다. 설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다.

---

# English

> Only what **changed versus 0.0.7** is listed.

## 🆕 Added
- **Elemental Adaptation** skill — **24 skills total**
  - Grows when you take elemental damage (fire/poison/electric/ice/ghost/space), 20 pts per damage
  - Max level: each element **-10%** · ★Elite: **-5% more** (-15% total)
- **Melee** now also grants **melee attack speed** — **+30%** at max level, ★Elite **+20% more** (🔼 buff)
- Skill window shows **XP as numbers (current/needed)** and a **per-level value line for every skill**

## ⚖️ Changes (versus 0.0.7)
| Skill | Change | Kind |
|---|---|---|
| Looting | detection-time reduction **-100% → -70%** (detection is slower) | 🔽 **nerf** |
| Strength | threshold **70/90% → 60/80%** (levels up while lighter) | 🔼 **buff** |
| Repair | **30 → 5** pts per durability restored | 🔽 **nerf** |

## 🐞 Fixes
- **Elemental Adaptation**: level and elite values were applied **swapped**
- **Elite looting**: rolled its chance per item (effectively always instant) → now rolled **once per container**
- **Detection time**: could reach 0 s at -100% → **0.2 s floor** enforced (detection becomes slower)
- **UI text**: all 10 languages still showed old values → synced
- **`info.ini`**: mangled line endings (`CR CR LF`) put stray characters into values → normalized

## ⚠️ Author's disclosure (new in 0.0.8)
- Physical damage is handled by the existing **Armor** skill, so it is excluded from Elemental Adaptation.

## Installation
Extract into `Duckov_Data\Mods\Dskill\`, then enable **Duckov Skill** in the game's **Mods** menu. **F6** opens the skill window · **F7** switches category.

## AI disclosure
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**. Design, balance decisions and in-game verification were done by the author.
