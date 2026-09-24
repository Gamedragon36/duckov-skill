# Duckov Skill 0.0.2 — 경험치 난이도(5단계) / XP difficulty stages

## 한국어

### 새 기능: 경험치 획득 난이도를 게임 안에서 5단계로 조절

**F6 스킬 창 상단**에 난이도 줄이 생겼습니다. 버튼을 누르면 **즉시 적용**되고 설정 파일에 저장됩니다.

```
경험치 난이도: 5/5 (×1)    [▶5] [1] [2] [3] [4] [5]    1=가장 빠름 … 5=기본 속도
```

| 단계 | 경험치 배율 | 스킬 1개 만렙까지(달리기 기준) |
|---|---|---|
| **1** (가장 빠름) | ×5.0 | **약 1시간** |
| 2 | ×2.5 | 약 2시간 |
| 3 | ×1.5 | 약 3시간 20분 |
| 4 | ×1.2 | 약 4시간 10분 |
| **5** (기본) | ×1.0 | **약 5시간** |

- **5단계가 기본값이며 지금까지의 밸런스와 완전히 동일합니다** → 기존 사용자는 아무 것도 바뀌지 않습니다
- `config.ini` 의 `xp_stage = 5` 로도 설정할 수 있습니다 (게임 안에서 바꾼 값이 우선)
- 로그라이크 계승 보너스와 `xp_multiplier` 는 이 배율에 **곱해집니다**

### 그 밖의 변경
- 창작마당 아이템 ID를 `info.ini` 에 고정 기록 (업로드가 새 아이템을 만드는 사고 방지)
- `config.ini` 에 `upload_now` (제작자 전용, 1회 업로드 후 자동 false) 추가
- 만렙(★엘리트) 시 엘리트 효과 설명 표시 (0.0.1 포함)

---

## English

### New: 5-step XP difficulty, adjustable in-game

A difficulty row now appears at the **top of the F6 skill window**. Clicking a button applies it **immediately** and saves it to the config file.

```
XP difficulty: stage 5/5 (x1)    [▶5] [1] [2] [3] [4] [5]    1 = fastest, 5 = default rate
```

| Stage | XP multiplier | Time to max one skill (sprinting) |
|---|---|---|
| **1** (fastest) | x5.0 | **about 1 hour** |
| 2 | x2.5 | about 2 hours |
| 3 | x1.5 | about 3h 20m |
| 4 | x1.2 | about 4h 10m |
| **5** (default) | x1.0 | **about 5 hours** |

- **Stage 5 is the default and is exactly the previous balance** → existing users see no change
- Also settable with `xp_stage = 5` in `config.ini` (an in-game change takes priority)
- The roguelike inheritance bonus and `xp_multiplier` are **multiplied** on top of this rate

### Other changes
- The Steam Workshop item id is now pinned in `info.ini` (prevents accidental duplicate items on upload)
- `config.ini` gained `upload_now` (author-only one-shot upload, auto-resets to false)
- Elite effect description shown at max level (from 0.0.1)

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 0.0.1 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브 유지).
> If you are on 0.0.1, replacing `Dskill.dll` is enough — your config and saves are kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
