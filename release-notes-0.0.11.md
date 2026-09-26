# Duckov Skill 0.0.11

> 0.0.10 과 비교해 **바뀐 것만** 적었습니다.

## ⚠️ 먼저 알려드립니다
0.0.10 에서 안내한 **사격술 "조준 시간 -50%"는 실제로 동작하지 않았습니다.** (스탯 키 이름을 잘못 지정해 게임이 무시했습니다)
이번 버전에서 **정상적으로 동작**합니다 — 실측으로 확인했습니다: 조준 시간 `0.36초 → 0.21초`, `0.55초 → 0.32초` (조준 속도 1.74배).
확인에 도움을 주신 모든 분께 감사드립니다.

## ⚖️ 변경 (0.0.10 대비)
| 스킬 | 변경 | 구분 |
|---|---|---|
| 인지/정찰 | 감지 거리 **+20% → +50%** (만렙, 엘리트 포함) | 🔼 버프 |
| 인지/정찰 | 효과 이름 **"시야 거리" → "플레이어 감지 거리"** | 🔁 표기 변경 |

## 🐞 버그 수정
| 증상 | 원인 | 수정 |
|---|---|---|
| **사격술 "조준 시간 -50%"가 아무 효과가 없던 것** | 게임이 읽는 스탯 키는 `ADSTime` 인데 `AdsTime` 으로 지정해 **게임이 그 효과를 무시**했습니다 | 올바른 키로 수정 → **정상 적용**(실측 1.74배) |
| 수리 ★엘리트 **"최대 내구도 감소 없음"이 체감되지 않던 것** | 최대치는 되돌렸지만 게임이 **줄어든 최대치까지만** 내구도를 채우고, 화면(툴팁)도 갱신되지 않았습니다 | 손실을 되돌리는 즉시 **한 번 수리로 최대까지 채우고**, 게임 UI까지 갱신 |
| 스킬 창에서 **낚시 효과가 `FishingTime +30%` 처럼 영어로 보이던 것** | 번역표에 그 효과 키가 없어서 키가 그대로 표시됐습니다 | 낚시 속도/등급 번역 추가 (**10개 언어 전체**) + 조준 시간 번역 추가 |

## 설치
압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣고, 게임 **모드(Mods)** 메뉴에서 **Duckov Skill** 을 켜세요. **F6** 스킬 창 · **F7** 계열 전환.

## AI 제작 표시
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다. 설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다.

---

# English

> Only what **changed versus 0.0.10** is listed.

## ⚠️ First, an important note
The **Assault "aim time -50%"** announced in 0.0.10 **did not actually work** (the stat key was misspelled, so the game ignored it).
It **works properly** now — verified by measurement: aim time `0.36 s → 0.21 s` and `0.55 s → 0.32 s` (1.74× faster aiming).

## ⚖️ Changes (versus 0.0.10)
| Skill | Change | Kind |
|---|---|---|
| Awareness | detection range **+20% → +50%** (max level, including elite) | 🔼 buff |
| Awareness | effect label **"view distance" → "player detection range"** | 🔁 rename |

## 🐞 Fixes
| Symptom | Cause | Fix |
|---|---|---|
| **Assault "aim time -50%" had no effect at all** | the game reads the stat key `ADSTime`, but the mod used `AdsTime`, so **the game ignored it** | corrected the key → **now applies** (measured 1.74×) |
| Repair ★Elite **"no max durability loss" was not noticeable** | the max value was restored, but the game only refills durability **up to the reduced max**, and the UI (tooltip) was not refreshed | on restoring the loss it now **fills to full in one repair** and refreshes the game UI |
| **Fishing effects showed as `FishingTime +30%` in English** in the skill window | the translation table had no entry for those effect keys | added fishing speed/grade translations (**all 10 languages**) and aim time |

## Installation
Extract into `Duckov_Data\Mods\Dskill\`, then enable **Duckov Skill** in the game's **Mods** menu. **F6** opens the skill window · **F7** switches category.

## AI disclosure
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**. Design, balance decisions and in-game verification were done by the author.
