# Duckov Skill 0.0.7 — 경험치 밸런스 재조정 + 정밀사격 수정 / XP balance rework + marksmanship fix

## ⚠️ 먼저 — 밸런스가 무너진 것에 대한 고지 / Disclosure

**0.0.6 에서 경험치 증가량이 과도해져 스킬 레벨 밸런스가 사실상 무너졌습니다.** 제작자가 이를 인지하고,
세이브 데이터를 직접 비교해 **실측**한 뒤 0.0.7 에서 수정했습니다.

| 실측 (0.0.6, 7분 레이드) | 값 |
|---|---|
| 파밍 | **172 XP/분** (시간당 10,340) |
| 투척 | 수류탄 1개 = 200 XP |
| 회복 | 80 XP/분 |
| 거래(기지) | 80 XP/분 (시간당 4,810) |
| 기지 체류 | **가만히 있어도** 시간당 3,370 |
| 합계 | **분당 610 XP → 만렙(18,350)이 레이드 3~4회** |

**0.0.7 변경 결과: 곡선 2.6배 × 획득량 약 0.5배 → 체감 약 5배 이상 느려집니다.**

> ⚠️ **In English:** In 0.0.6 skill XP gain was far too generous and the balance broke. I confirmed it by
> measuring the save data, published the numbers above, and fixed it in 0.0.7 — the curve is 2.6x steeper
> and the gain rates are about half, so progression is **5x+ slower** overall.

---

## ⚖️ 밸런스 변경 (요청 반영)

### 1) 레벨 곡선 상향 (config: `xp_base`, `xp_step`)
| 레벨 | 0.0.6 누적 | **0.0.7 누적** |
|---|---|---|
| 1레벨 | 300 | **500** |
| 5레벨 | 2,150 | **4,500** |
| 10레벨 | 5,925 | **14,000** |
| 15레벨 | 11,325 | **28,500** |
| **20레벨(만렙)** | 18,350 | **48,000 (2.6배)** |

### 2) 획득 요율 하향
| 스킬 | 항목 | 이전 | **이후** |
|---|---|---|---|
| 파밍 | 아이템 감지 / 획득 | 40 / 15 | **20 / 10** |
| 거래 | 1,000 골드당 | 10 | **5** |
| 투척 | 폭발물 1개 | 200 | **100** |
| 회복 | 회복량 1포인트당 | 20 | **10** |
| 반동 | 발사 / 조준 발사 | 3 / 6 | **2 / 4** |
| 구르기 | 1회 | 8 | **4** |
| 기지 체류 | 초당 | 1 | **0.5** |

*(0.0.6 에서 2배로 올렸던 근력·지구력 요율은 그대로 두고, 위 항목과 곡선으로 전체 속도를 맞췄습니다.)*

---

## 🐞 버그 수정

### 정밀사격(marksmanship) — 20m 기준은 유지, 획득량 상향
- 20m 기준은 **의도된 설계**(저격용 — 멀리서만 잡으라는 의미)이므로 그대로 두고, **한 발 가치를 올렸습니다.**
- 이전: 1회 45점 / 헤드샷 +70 → **1회 150점 / 헤드샷 +200점 (약 3.3배)**
- 참고: 실제 세이브에서 사격 577회 적중에도 정밀사격 누적이 0 이었습니다(20m 이상 조건이 좀처럼 성립하지 않음).
  이제 같은 조건에서 한 발당 3배 이상 오르므로 저격 플레이가 보상받습니다.


### 0.0.6 수정 사항 포함
- 수리 경험치 감지(장착 슬롯 + 창고), 투척·파밍 자기잠금 해제, 회복 = 치료 속도, 구르기(쿨타임·동작 시간·거리 보정)

### UI 설명 문구가 이전 수치를 그대로 쓰던 문제 (같은 0.0.7 에서 수정)
- 스킬 창의 성장 조건·엘리트 설명이 0.0.6 이전 값을 표시하고 있었습니다. **10개 언어 전부** 최신 수치로 교체했습니다.
  - 회복 20→**10**점 · 구르기 1회 8→**4**점 · 반동 3·6→**2·4** · 정밀사격 45·70→**150·200** (20m 기준 유지) · 투척 200→**100**점
  - 거래 1,000당 10→**5** · 파밍 40·15→**20·10** · 기지 체류 초당 1→**0.5**
  - 구르기 엘리트 문구: "-20% 추가" → "쿨타임·동작 시간·스태미나 **-10% 추가**" (실제 값과 일치)
  - 구르기 효과 문구에 누락되어 있던 **동작 시간** 감소를 추가


---

## English

### ⚖️ Balance changes (as requested)

**1) Steeper level curve** (`xp_base`, `xp_step`) — Lv.1 300→**500**, Lv.10 5,925→**14,000**,
**Lv.20 (max) 18,350 → 48,000 (2.6x)**

**2) Lower gain rates**
| Skill | Item | Before | **After** |
|---|---|---|---|
| Scavenging | item scan / pickup | 40 / 15 | **20 / 10** |
| Barter | per 1,000 gold | 10 | **5** |
| Throwing | per explosive | 200 | **100** |
| Healing | per heal point | 20 | **10** |
| Recoil | shot / ADS shot | 3 / 6 | **2 / 4** |
| Roll | per use | 8 | **4** |
| Hideout | per second | 1 | **0.5** |

### 🐞 Fixes
- **Marksmanship**: the 20 m requirement is **intentional** (sniper play), so it stays — the reward per hit was raised
  instead: 45 → **150** pts per hit, headshot +70 → **+200** (~3.3x). In a real save, 577 rifle hits still left it at
  0 XP because 20 m+ hits are rare; now each qualifying shot is worth 3x more.

- Includes the 0.0.6 fixes: repair detection (equipped slots + stash), throwing/scavenging self-lock,
  healing speed instead of amount, roll (cooldown + action time + distance compensation).

### Stale UI text (fixed in the same 0.0.7 update)
- The skill window's trigger/elite descriptions still showed pre-0.0.6 numbers. **All 10 languages** were updated:
  - Healing 20→**10** pts · Roll 8→**4** pts per use · Recoil 3 & 6→**2 & 4** · Marksmanship 45 & 70→**150 & 200** (20 m kept) · Throwing 200→**100**
  - Barter 10→**5** per 1,000 · Scavenging 40 & 15→**20 & 10** · Hideout 1→**0.5** per second
  - Roll elite text: "-20% more" → "cooldown, action time and stamina **-10% more**" (now matches the real value)
  - Added the missing **action time** reduction to the roll effect line


### ⚠️ Note on 0.0.6
In 0.0.6 XP gain was far too high and the balance broke (measured: 610 XP/min, max level in 3-4 raids).
0.0.7 fixes this: **curve 2.6x steeper × rates about half → 5x+ slower progression.**

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 기존 스킬 레벨·세이브·설정은 그대로 유지됩니다. 곡선이 가팔라져 **표시 레벨이 몇 단계 내려갈 수 있습니다** (정상).
> Your levels, saves and settings are kept. Because the curve is steeper, **shown levels may drop a few steps** (expected).
> 곡선을 직접 바꾸려면 `config.ini` 의 `xp_base` / `xp_step` 을 수정하세요.
> To tune it yourself, edit `xp_base` / `xp_step` in `config.ini`.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
