# Duckov Skill 0.0.10

> 0.0.9 와 비교해 **바뀐 것만** 적었습니다.

## 🆕 추가
- **사격술: 조준 시간 -50%** (만렙 기준) — 총을 들고 조준(우클릭)할 때 걸리는 시간이 절반이 됩니다
- **근접 공격 홀드** — 근접 무기를 들고 **공격 버튼(마우스 왼쪽)을 꾹 누르면 계속 휘두릅니다** (`melee_hold_repeat`, 기본 켜짐)

## ⚖️ 변경 (0.0.9 대비)
| 스킬 | 변경 | 구분 |
|---|---|---|
| 구르기 | 1회 **4 → 6점** | 🔼 버프 |
| 정밀 사격 | 기준 **20m → 25m**, 명중 **150 → 600점**, 헤드샷 **+200 → +800점** | 🔼 버프 |
| 투척술 ★엘리트 | 30% 확률 즉시 폭발 → **지면에 닿으면 100% 즉시 폭발** | 🔼 버프 |
| 인지/정찰 | 소리 1회 **4 → 12점**, 최대 처리 간격 **1초 → 0.5초** | 🔼 버프 |
| 제작 | 재료 가치 **×8% → ×10%**, 1회 상한 **400점 폐지** | 🔼 버프 |
| 지구력 | 스태미나 소모 **-40% → -60%** | 🔼 버프 |
| 지구력 | 이동 속도 **+10% 삭제** (근력에 이미 있어 중복이었음) | 🔽 너프 |
| 방어 | 신체·머리 방어구 **+2.0 → +1.5** | 🔽 너프 |
| 재장전 | 1회 **40 → 50점** | 🔼 버프 |
| 흥정 | 거래 1,000당 **5 → 10점** | 🔼 버프 |

## 🐞 버그 수정
| 증상 | 원인 | 수정 |
|---|---|---|
| **칼 외의 모든 좌클릭이 연속 클릭**이 되던 것 | 근접 홀드가 "슬롯에 근접 무기가 있으면" 동작하도록 되어 있었음 | **실제로 손에 든 근접 무기일 때만** 동작 + UI가 열려 있으면 입력을 넣지 않음 |
| 스킬 창에서 **설명이 2줄이 되면 아래 내용이 잘리던 것** | 창 높이를 "줄 수 × 고정값"으로 계산 | 실제 텍스트 높이를 측정해 창 크기·스크롤을 계산 |
| 업로드 후 **설정 파일 복구가 메인 메뉴에서 멈추던 것** | "게임 시간 45초" 대기(메뉴에서는 시간이 멈춤) | 실시간 45초 대기로 변경 |
| 감사: **로그가 과도하게 쌓이던 것** | 명중할 때마다 · 구르기마다 로그 출력 | 명중 로그 제거 · 구르기 로그는 5초에 1번 |
| 독일어 알림 문구가 영어로 남아 있던 것 | 미번역 1건 | `[Fähigkeit] …` 로 수정 |

## 설치
압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣고, 게임 **모드(Mods)** 메뉴에서 **Duckov Skill** 을 켜세요. **F6** 스킬 창 · **F7** 계열 전환.

## AI 제작 표시
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다. 설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다.

---

# English

> Only what **changed versus 0.0.9** is listed.

## 🆕 Added
- **Assault: aim time -50%** (at max level) — aiming down sights takes half the time
- **Melee attack hold** — with a melee weapon equipped, **holding the attack button (left mouse) keeps swinging** (`melee_hold_repeat`, on by default)

## ⚖️ Changes (versus 0.0.9)
| Skill | Change | Kind |
|---|---|---|
| Dash | **4 → 6** pts per roll | 🔼 buff |
| Marksmanship | range **20 m → 25 m**, hit **150 → 600** pts, headshot **+200 → +800** | 🔼 buff |
| Throwing ★Elite | 30% chance of instant detonation → **detonates instantly on ground contact (100%)** | 🔼 buff |
| Awareness | **4 → 12** pts per sound, rate limit **1 s → 0.5 s** | 🔼 buff |
| Crafting | material value **×8% → ×10%**, per-craft cap **400 pts removed** | 🔼 buff |
| Endurance | stamina drain **-40% → -60%** | 🔼 buff |
| Endurance | movement **+10% removed** (it was duplicated on Strength) | 🔽 nerf |
| Armor | body & head armor **+2.0 → +1.5** | 🔽 nerf |
| Reloading | **40 → 50** pts per reload | 🔼 buff |
| Bartering | **5 → 10** pts per 1,000 traded | 🔼 buff |

## 🐞 Fixes
| Symptom | Cause | Fix |
|---|---|---|
| **Left click became auto-click for everything except knives** | the melee-hold gate used "a melee weapon is in a slot" | gate on the **melee weapon actually held** + never inject while a UI is open |
| **Skill window clipped the rows below when a description wrapped to 2 lines** | window height was computed as "rows × fixed value" | measure real text height and size the window/scroll accordingly |
| **Config file restore got stuck at the main menu** after an upload | waited 45 s of *game* time (time is frozen in menus) | wait 45 s of real time |
| Log spam (audit) | a log line per hit and per dash | removed the per-hit log, throttled the dash log to once per 5 s |
| German notification text still in English | one untranslated entry | changed to `[Fähigkeit] …` |

## Installation
Extract into `Duckov_Data\Mods\Dskill\`, then enable **Duckov Skill** in the game's **Mods** menu. **F6** opens the skill window · **F7** switches category.

## AI disclosure
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**. Design, balance decisions and in-game verification were done by the author.

