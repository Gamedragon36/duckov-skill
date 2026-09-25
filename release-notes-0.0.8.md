# Duckov Skill 0.0.8 — 속성적응 스킬 추가 · 파밍/속성 밸런스 수정 · 스킬 창 정보 개선

## 🆕 새 스킬: 속성적응 (24번째)
| 항목 | 내용 |
|---|---|
| 성장 조건 | 속성 피해(**화염·독·전기·얼음·유령·우주**)를 받을 때 — 피해 1당 **20점** (물리 피해는 '방어' 스킬 담당) |
| 만렙(20) 효과 | 각 속성 피해 **-10%** |
| ★엘리트 | 각 속성 **-5% 추가** (합계 **-15%**) |

## ⚖️ 밸런스 변경
| 항목 | 내용 |
|---|---|
| **파밍 감지 시간** | **-70%** (레벨당 3.5%) · **최소 0.2초** 보장(0초가 되어 즉시 감지되지 않게) |
| **정밀 사격** | 20m 기준 **유지**(저격용 의도) · 명중 1회 **150점**, 헤드샷 **+200점** |
| 근력 성장 임계 | 70/90% → **60/80%** |
| 수리 경험치 | 내구도 1당 **5점** |

## 🐞 수정
- **속성적응 효과/엘리트 수치가 뒤바뀌어 있었음** → 레벨 누적 **-10%** / 엘리트 **-5%** 로 정정 (합계 -15% 동일)
- **엘리트 파밍 굴림**: 아이템마다 굴리던 것을 **상자 단위 1회**로 수정
  (0.4초마다 재굴림해서 사실상 항상 즉시 감지되던 문제)
- **감지 시간 하한** `0.2초` 적용 (`Mathf.Max`) — -100%에서도 즉시 감지가 되지 않게
- **UI 문구 10개 언어 동기화** (파밍·반동·구르기·기지·투척·회복·거래·정밀사격 + 속성적응 신규)
- `info.ini` 줄바꿈 손상(`CR CR LF`) 정규화 — 값에 불필요한 문자가 섞이지 않게

## ✨ 스킬 창 개선
- 경험치를 **퍼센트 + 정수(현재/필요)** 로 함께 표시
- **모든 스킬에 '레벨당 수치' 한 줄** 추가 (수치가 안 보이던 스킬 포함)
- 제목을 `★ Duckov Skill` 로 정리

## ⚠️ 0.0.6 밸런스 붕괴 고지 (유지)
0.0.6 에서 경험치 증가가 과도해 **밸런스가 무너졌던 것**을 인지하고, 세이브 실측 데이터(분당 610 XP·기지 3,370/시간)를
근거로 0.0.7 에서 **곡선 2.6배 + 요율 약 0.5배**로 수정했습니다. 0.0.8 은 그 위에서의 추가 조정입니다.

---

# English

## 🆕 New skill: Elemental Adaptation (24th)
Takes elemental damage (**fire/poison/electric/ice/ghost/space**) — **20 pts per damage** (physical is covered by Armor).
Level effect: each element **-10%** · ★Elite: each element **-5%** more (**-15% total**).

## ⚖️ Balance
| Item | Detail |
|---|---|
| **Looting detection time** | **-70%** (3.5% per level), with a **0.2 s floor** so it never becomes instant |
| **Marksmanship** | 20 m requirement **kept** (intended for sniping) · **150** pts per hit, headshot **+200** |
| Strength threshold | 70/90% → **60/80%** |
| Repair | 5 pts per durability restored |

## 🐞 Fixes
- **Elemental Adaptation had its level and elite values swapped** → corrected to level **-10%** / elite **-5%** (-15% total unchanged)
- **Elite looting roll**: was rolled per item; now **rolled once per container** (re-rolling every 0.4 s made it effectively always instant)
- **Detection floor** of 0.2 s applied via `Mathf.Max`
- **UI text synced across all 10 languages** (looting, recoil, roll, hideout, throwing, healing, barter, marksmanship + the new skill)
- Normalized mangled line endings (`CR CR LF`) in `info.ini`

## ✨ Skill window
- Shows XP as **percent + numbers (current/needed)**
- Adds a **"per level" line for every skill** (including ones that showed no numbers before)
- Title cleaned up to `★ Duckov Skill`

## ⚠️ Notice about 0.0.6 (unchanged)
XP gain in 0.0.6 was far too high and broke the balance. It was fixed in 0.0.7 (curve 2.6x steeper, rates about half),
based on measured save data (610 XP/min, 3,370 XP/h in the hideout). 0.0.8 adjusts further on top of that.

---

## 설치 / Installation
1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 스킬 창 · Press **F6** for the skill window

> 0.0.7 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브·스킬 레벨 유지).
> If you are on 0.0.7, replacing `Dskill.dll` is enough — config, saves and skill levels are kept.

## AI 제작 표시 / AI disclosure
이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
