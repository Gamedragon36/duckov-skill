# Duckov Skill 0.0.1 — 다국어 지원 / Multi-language support

## 한국어

### 이번 버전의 핵심: 게임이 지원하는 10개 언어 로케일 추가

모드 UI가 **게임이 지원하는 10개 언어 전부**로 번역됩니다.

**추가한 나라 / 언어**

| 언어 | 표기 |
|---|---|
| 한국어 | Korean |
| 영어 | English |
| 중국어(간체) | 简体中文 (Simplified Chinese) |
| 중국어(번체) | 繁體中文 (Traditional Chinese) |
| 일본어 | 日本語 (Japanese) |
| 독일어 | Deutsch (German) |
| 러시아어 | Русский (Russian) |
| 스페인어 | Español (Spanish, LatAm) |
| 프랑스어 | Français (French) |
| 포르투갈어(브라질) | Português (Brazil) |

**번역 범위**
- 스킬 창(F6)의 모든 문구 — 창 제목 · 계열 탭 · 총합 · 성장 조건 · 하단 안내
- 스킬 **23종**의 이름 · 성장 조건 · ★엘리트 효과
- 스탯 이름 40종 (최대 무게, 반동 제어, 치료 효율 등)
- 레벨업 알림과 계승(로그라이크) 문구
- `config.ini` 주석 (한국어 · 영어 · 중국어 간체/번체)
  - 독일어·러시아어·스페인어·프랑스어·포르투갈어는 `config.ini` 주석만 영어로 표시됩니다 (게임 내 텍스트는 모두 번역됨)

**언어가 정해지는 순서** (기본값 `language = auto`)
1. **게임에서 고른 언어**를 그대로 읽습니다 → 게임 UI와 모드 UI가 항상 같은 언어가 됩니다
2. 게임 설정을 읽을 수 없으면 **운영체제(Windows) 언어**
3. 둘 다 지원 대상이 아니면 **영어**

`config.ini` 에서 직접 지정할 수도 있습니다:
`auto`, `ko`, `en`, `zh`, `zh-hant`, `ja`, `de`, `ru`, `es`, `fr`, `pt-br`

### 그 밖의 개선
- 만렙(★엘리트)일 때 **엘리트 효과 설명이 스킬 창에 표시**됩니다 (이전에는 보이지 않았습니다)
- 스탯 이름 5종 추가 — 야간 시야 · 시야 거리 · 감지 범위 · 청력 · 받는 물리 피해
- 스킬 이름 열 너비를 늘려 영어·독일어 등 긴 이름이 잘리지 않게 조정
- 창작마당 설명에 중국어(간체/번체) · 일본어 · 영어 로케일 안내 추가

### 설계 원칙 (0.0.0과 동일)
- **게임 코드를 패치하지 않습니다** (Harmony 미사용) → 다른 모드와 충돌이 적습니다
- 스탯 수정자를 자체 토큰으로 격리해 게임·다른 모드의 효과를 건드리지 않습니다
- 적용할 수 없는 값은 로그로 경고하고 그 효과만 건너뜁니다

---

## English

### Headline: localization for all 10 languages the game supports

The mod UI is now translated into **every language Escape From Duckov supports**.

**Languages added**

| Language | Native |
|---|---|
| Korean | 한국어 |
| English | English |
| Chinese (Simplified) | 简体中文 |
| Chinese (Traditional) | 繁體中文 |
| Japanese | 日本語 |
| German | Deutsch |
| Russian | Русский |
| Spanish (LatAm) | Español |
| French | Français |
| Portuguese (Brazil) | Português |

**What is translated**
- Every string in the skill window (F6) — title, category tabs, totals, growth conditions, footer
- The **name, growth condition and ★Elite bonus of all 23 skills**
- 40 stat labels (max weight, recoil control, healing efficiency, …)
- Level-up notifications and the roguelike inheritance text
- The `config.ini` comments (Korean / English / Simplified + Traditional Chinese)
  - German, Russian, Spanish, French and Portuguese-Brazilian keep the config comments in English (all in-game text is translated)

**How the language is chosen** (`language = auto` by default)
1. **The language you selected in the game** is read directly → the mod always matches the game UI
2. If that cannot be read, your **operating-system language**
3. If neither is supported → **English**

You can also force one in `config.ini`:
`auto`, `ko`, `en`, `zh`, `zh-hant`, `ja`, `de`, `ru`, `es`, `fr`, `pt-br`

### Other improvements
- At max level the **★Elite effect description is now shown** in the skill window (it was missing before)
- 5 new stat labels — night vision, view distance, detection range, hearing, physical damage taken
- Wider skill-name column so long English/German names are not cut off
- Workshop description now includes Chinese (Simplified/Traditional), Japanese and an English locale section

### Design principles (unchanged from 0.0.0)
- **No game-code patching** (no Harmony) → minimal conflict with other mods
- Stat modifiers are isolated with their own token, so game/other-mod effects are never touched
- If a value cannot be applied it is logged as a warning and only that effect is skipped

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다
   Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 메뉴에서 **Duckov Skill** 을 켭니다 (새 모드는 기본 꺼짐)
   Launch the game → **Mods** menu → enable **Duckov Skill** (new mods are off by default)
3. **F6** 으로 스킬 창을 엽니다 · Press **F6** to open the skill window

> 0.0.0 사용자는 `Dskill.dll` 과 `info.ini` 만 교체하면 됩니다 (설정은 `config.ini` 에 그대로 유지됩니다).
> If you already use 0.0.0, replace only `Dskill.dll` and `info.ini` — your `config.ini` is kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
(Steam 창작마당 규칙 5 / Steam Workshop rule 5)
