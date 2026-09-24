# Duckov Skill 0.0.3 — 버그 수정 · 성능 최적화 / Bug fixes and performance

## 한국어

### 고친 버그 (전수조사 결과)

| # | 증상 | 원인 | 수정 |
|---|---|---|---|
| 1 | **한국어 사용자인데 모드 UI가 영어로 표시됨** | 문장 조회 순서가 `현재 언어 → 영어 → 한국어` 였는데, 한국어는 별도 표를 두지 않아 항상 영어가 먼저 잡혔습니다 | 한국어일 때는 영어 표를 건너뛰고 코드 안의 한국어 문장을 사용 |
| 2 | **근접 무기로 때려도 '근접 전투' 경험치가 오르지 않음** (명중 30점 누락) | `MeleePerHit`(30점) 값이 정의만 되고 코드에서 한 번도 쓰이지 않았습니다 (처치 보너스 60점만 적용) | 근접 명중 시 +30점 지급 추가 |
| 3 | 수리 스킬: 캐릭터·세이브를 바꾸면 **다른 아이템의 수리 값이 잘못 적용**될 수 있음 | 원래 값을 아이템별이 아니라 모드 전체에서 한 번만 저장 | 아이템이 바뀌면 원래 값을 다시 읽도록 수정 |
| 4 | 아주 긴 레이드에서 **처리한 수류탄·아이템이 다시 처리**될 수 있음 (중복 대미지·중복 경험치) | 추적 목록을 '개수 초과'로 비울 때 중복 위험 | 레벨(맵) 이동 시 추적 목록을 확실히 초기화 |
| 5 | 생존술 효과 문장의 화염·독 피해 수치가 **실제 밸런스 값과 따로 하드코딩**되어 어긋날 수 있음 | `0.005` 하드코딩 | `SkillDefs` 의 실제 효과 값을 읽어 표시 |

### 최적화 (프레임·렌더링 부담 감소)

| # | 항목 | 이전 | 이후 |
|---|---|---|---|
| 1 | 수류탄 확인(씬 전체 검색) | 폭발물 사용 후 6초간 0.1초, **평소 0.5초마다** | 폭발물 사용 후 6초간 0.1초, **평소 2초마다** (4배 감소) |
| 2 | 상인 쿨타임 확인(씬 전체 검색) | 기지에서 **0.5초마다** | **5초마다** (10배 감소) |
| 3 | 스킬 창 효과 문장 | 매 프레임 새로 생성 (초당 수백 개 문자열 쓰레기) | 레벨이 바뀔 때만 만들고 **캐시 재사용** |
| 4 | 글꼴 | 한국어 폰트 위주 | **중국어(간체·번체)·일본어 폰트 추가** (비한국어 환경에서 글자 깨짐 방지) |

### 참고
- 스킬·세이브·설정은 그대로 유지됩니다 → **`Dskill.dll` 만 교체**하면 됩니다
- 경험치 난이도(1~5단계)와 10개 언어 지원은 0.0.2와 동일합니다

---

## English

### Bugs fixed (from a full code audit)

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 1 | **Korean users saw the mod UI in English** | The lookup order was `current → English → Korean`, but Korean has no separate table, so English always matched first | Korean now skips the English table and uses the built-in Korean strings |
| 2 | **Melee hits gave no "Melee Combat" XP** (30 pts missing) | `MeleePerHit` was defined but never used in code (only the 60-pt kill bonus applied) | Melee hits now award +30 pts |
| 3 | The repair skill could apply the **wrong repair value after changing character or save** | The original value was stored once per mod instance instead of per item | The originals are re-read whenever the item changes |
| 4 | In very long raids, handled grenades/items could be **processed twice** (double damage / double XP) | Tracking sets were cleared by size, allowing re-processing | Tracking sets are now cleared reliably on level change |
| 5 | The survival effect text used a **hardcoded fire/poison value** that could drift from the real balance value | Hardcoded `0.005` | Reads the real effect value from `SkillDefs` |

### Optimizations (less frame/render work)

| # | Item | Before | After |
|---|---|---|---|
| 1 | Grenade check (full-scene search) | 0.1 s for 6 s after throwing, **otherwise every 0.5 s** | 0.1 s for 6 s after throwing, **otherwise every 2 s** (4x less) |
| 2 | Merchant cooldown check (full-scene search) | **Every 0.5 s** in the hideout | **Every 5 s** (10x less) |
| 3 | Skill window effect text | Rebuilt every frame (hundreds of strings/second) | Built only when the level changes, then **cached** |
| 4 | Fonts | Mostly Korean fonts | **Added Chinese (Simplified/Traditional) and Japanese fonts** (fixes missing glyphs on non-Korean systems) |

### Notes
- Skills, saves and settings are unchanged → **replacing `Dskill.dll` is enough**
- The 5-stage XP difficulty and 10-language support are the same as 0.0.2

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 0.0.2 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브 유지).
> If you are on 0.0.2, replacing `Dskill.dll` is enough — your config and saves are kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
