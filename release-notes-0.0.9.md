# Duckov Skill 0.0.9

> 0.0.8 과 비교해 **바뀐 것만** 적었습니다.

## 🆕 추가
- **이동 거리 비례 성장** — 이제 **실제로 이동한 거리(m)** 가 경험치가 됩니다 (낙하·점프 같은 수직 이동은 제외)
  - **근력**: 무게 60% 이상으로 이동 → **1m당 1.6점**
  - **지구력**: 달리기 **· 구르기** 이동 → **1m당 1.04점**
  - **은신 이동**: 걷기 → **초당 1점 + 1m당 0.7점**
  - 이동 속도 버프를 받으면 같은 시간에 더 멀리 가므로 **더 많이** 오릅니다
- **구르기 키(스페이스바)를 꾹 누르면 자동 연속 구르기** — `config.ini` 의 `dash_hold_repeat` (기본 켜짐)
  - 쿨타임과 스태미나 검사는 **게임이 그대로** 하므로 무한 구르기가 되지 않습니다

## ⚖️ 변경 (0.0.8 대비)
| 스킬 | 변경 | 구분 |
|---|---|---|
| 근력 | 초당 4점(80% 이상 8점) → **1m당 1.6점** | 🔁 **방식 변경** |
| 지구력 | 초당 4점 → **1m당 1.04점** (구르기도 인정) | 🔼 **버프** |
| 은신 이동 | 초당 1점 → **초당 1점 + 1m당 0.7점** | 🔼 **버프** |
| 근접 전투 | 명중 30 → **45점**, 처치 60 → **90점** | 🔼 **버프** |
| 지구력 (스태미나) | 스태미나 소모 -40% 가 **구르기 스태미나에도 중첩** (둘 다 만렙이면 **-64%**) | 🔼 **버프** |
| 파밍 ★엘리트 | 성공(50%) 시 **상자·시체 안의 모든 아이템을 즉시 감지** | 🔼 **버프** |

> 근력은 "시간당"에서 "거리당"으로 바뀌었습니다 → **가만히 서서 버티는 것으로는 오르지 않습니다**(의도).

## 🐞 버그 수정
| 증상 | 원인 | 수정 |
|---|---|---|
| **근접·사격의 '명중' 경험치가 전혀 오르지 않음** (처치 경험치만 들어옴) | 피해 처리 코드가 "내가 맞은 경우"만 통과시키고 돌아가, **내가 준 피해 처리부가 실행되지 않았음** | 받은 대상과 준 사람을 **각각** 판정하도록 분리 |
| **정밀 사격이 20m 미만에서도** 오름 | 거리 계산에 쓴 좌표가 총알 피해에서 비어 있어(0,0,0) 엉뚱하게 먼 거리로 계산됨 | **맞은 대상의 위치** 기준으로 측정하고, 알 수 없으면 지급하지 않음 |
| 정밀 사격이 **20m 이상에서도** 오르지 않음 | 위와 같은 원인(경로 자체가 죽어 있었음) | 위 수정으로 함께 해결 |
| **구르기로 지구력이 오르지 않음** | 게임은 구르기 중 '달리기' 플래그를 켜지 않아 '걷기'로 분류됨 | 구르기도 달리기로 인정 |
| 파밍 ★엘리트가 성공해도 **절반만** 즉시 감지 | 굴림을 주변 모든 상자에 소비 + 캐시 초기화로 재굴림 + 감지 시작 전 아이템 누락 | **내가 연 상자만** 굴리고 결과를 고정, 성공 시 **전부** 감지 |
| 기존 사용자의 `config.ini` 에 새 옵션 줄이 없음 | 새 옵션은 기존 파일에 생기지 않음 | 로드 시 **자동으로 한 줄 추가** |
| 새로 만든 `config.ini` 의 `xp_step` 이 65로 생성 | 설정 템플릿과 코드 기본값(100) 불일치 | 템플릿을 **100** 으로 수정 |

## 설치
압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣고, 게임 **모드(Mods)** 메뉴에서 **Duckov Skill** 을 켜세요. **F6** 스킬 창 · **F7** 계열 전환.

## AI 제작 표시
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다. 설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다.

---

# English

> Only what **changed versus 0.0.8** is listed.

## 🆕 Added
- **Distance-based movement XP** — the **distance you actually travel (in metres)** now grants XP (vertical movement such as falling/jumping is excluded)
  - **Strength**: moving above 60% carry weight → **1.6 pts/m**
  - **Endurance**: sprinting **or dashing** → **1.04 pts/m**
  - **Covert Movement**: walking → **1 pt/s + 0.7 pts/m**
  - Movement-speed buffs mean you cover more ground in the same time, so you gain **more** XP
- **Hold the dash key (Space) to keep rolling automatically** — `dash_hold_repeat` in `config.ini` (enabled by default)
  - Cooldown and stamina are still checked **by the game itself**, so this never becomes infinite dashing

## ⚖️ Changes (versus 0.0.8)
| Skill | Change | Kind |
|---|---|---|
| Strength | 4 pts/s (8 above 80%) → **1.6 pts/m** | 🔁 **method change** |
| Endurance | 4 pts/s → **1.04 pts/m** (dashing now counts) | 🔼 **buff** |
| Covert Movement | 1 pt/s → **1 pt/s + 0.7 pts/m** | 🔼 **buff** |
| Melee | 30 → **45** pts per hit, 60 → **90** per kill | 🔼 **buff** |
| Endurance (stamina) | the -40% stamina drain now **also stacks on dash stamina** (**-64%** with both maxed) | 🔼 **buff** |
| Looting ★Elite | on success (50%) **every item in the container/corpse is revealed instantly** | 🔼 **buff** |

> Strength moved from "per second" to "per metre" → **standing still no longer earns XP** (intended).

## 🐞 Fixes
| Symptom | Cause | Fix |
|---|---|---|
| **Melee and gun HIT XP never increased** (only kill XP came in) | the damage handler returned early unless *you* were the victim, so the **"damage I dealt" block never ran** | split the checks for "victim" and "attacker" |
| **Marksmanship triggered below 20 m** | the coordinate used for distance was empty (0,0,0) on bullet damage, producing a bogus long distance | measure from the **victim's position**; if unknown, award nothing |
| Marksmanship did **not** trigger beyond 20 m | same cause (the whole path was dead) | fixed by the change above |
| **Dashing did not raise Endurance** | the game never sets the 'running' flag while dashing, so it was treated as walking | dashing now counts as sprinting |
| Looting ★Elite revealed only **half** the items on success | rolls were spent on every nearby container + the cache reset and re-rolled + items not yet being inspected were skipped | roll **only for the container you opened**, lock the result, and reveal **everything** on success |
| New options were missing from existing `config.ini` files | new options are never created in an existing file | one line is **appended automatically** on load |
| Newly created `config.ini` had `xp_step = 65` | template default disagreed with the code default (100) | template fixed to **100** |

## Installation
Extract into `Duckov_Data\Mods\Dskill\`, then enable **Duckov Skill** in the game's **Mods** menu. **F6** opens the skill window · **F7** switches category.

## AI disclosure
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**. Design, balance decisions and in-game verification were done by the author.
