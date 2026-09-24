# Duckov Skill 0.0.5 — 핫픽스: 하이드아웃(기지)에서 스킬 경험치가 오르던 문제 / Hotfix: skills gaining XP in the hideout

## 한국어

### 🐞 치명적 버그 수정 — 기지에서 스킬 경험치가 오르던 문제

기지(하이드아웃)에서 배회·정리만 해도 **사격·이동·파밍 같은 스킬이 계속 오르는** 문제가 있었습니다.
원인: 경험치 지급이 **장소를 구분하지 않아** 기지에서도 모든 행동 경험치가 그대로 들어왔습니다.

**수정**: 경험치의 단일 통로(`AddXp`)에서 **기지일 때는 허용된 스킬만** 지급하도록 했습니다.

| 기지에서 **오르는** 스킬 (의도된 것) | 기지에서 **오르지 않는** 스킬 |
|---|---|
| **하이드아웃** (기지 체류 · 건설) | 은신 이동 · 지구력 · 근력 · 생명력 · 회복 |
| **제작** (작업대) | 사격술 · 반동 제어 · 정밀 사격 · 근접 전투 · 투척술 |
| **수리** (작업대) | 파밍 · 인지/정찰 · 야간시야 · 방어 · 생존술 |
| **흥정** (상인 거래) | 재장전 · 구르기 · 낚시 |
| **신진대사** (음식·물 섭취 — 의도된 오마주) | |

- 차단된 스킬은 로그에 1회씩 남습니다:
  `[Dskill] 기지에서는 'covert' 경험치가 오르지 않습니다 (의도된 제한 — config.ini 의 hideout_skills 로 조정)`
- **기지에서도 오르게 하고 싶은 스킬이 있으면** `config.ini` 의 `hideout_skills` 목록에 추가하세요 (쉼표 구분, 게임 재시작 후 적용)

```ini
hideout_skills = hideout,crafting,repair,barter,metabolism
```

### 참고
- **레이드에서는 모든 스킬이 정상적으로** 오릅니다 (제한은 기지에서만 적용)
- 이미 오른 스킬 레벨은 그대로 유지됩니다

---

## English

### 🐞 Critical bug fixed — skills were gaining XP in the hideout

Simply walking around and sorting loot in the hideout could keep leveling **combat / movement / looting** skills.
Cause: XP awards did not check the location, so every action also gave XP in the base.

**Fix**: the single XP entry point (`AddXp`) now awards XP in the hideout only for an **allowed list** of skills.

| Skills that **do** gain in the hideout (intended) | Skills that **no longer** gain there |
|---|---|
| **Hideout** (base time & building) | Stealth Movement · Endurance · Strength · Vitality · Healing |
| **Crafting** (workbench) | Rifleman · Recoil Control · Marksmanship · Melee · Throwables |
| **Repair** (workbench) | Scavenging · Awareness · Night Vision · Armor · Survival |
| **Bartering** (traders) | Reloading · Roll · Fishing |
| **Metabolism** (eat / drink — the intended homage) | |

- Blocked skills are logged once each:
  `[Dskill] 기지에서는 'covert' 경험치가 오르지 않습니다 (의도된 제한 — config.ini 의 hideout_skills 로 조정)`
- **Want a skill to keep gaining in the base?** Add it to `hideout_skills` in `config.ini` (comma separated, applies after a restart)

```ini
hideout_skills = hideout,crafting,repair,barter,metabolism
```

### Notes
- **In raids every skill gains XP normally** (the restriction applies to the hideout only)
- Skill levels you already earned are kept

---

## 설치 / Installation

1. 압축을 풀어 `Duckov_Data\Mods\Dskill\` 에 넣습니다 · Extract into `Duckov_Data\Mods\Dskill\`
2. 게임 실행 → **모드(Mods)** 에서 **Duckov Skill** 켜기 · Enable **Duckov Skill** in the **Mods** menu
3. **F6** 으로 스킬 창 · Press **F6** for the skill window

> 0.0.4 사용자는 `Dskill.dll` 만 교체하면 됩니다 (설정·세이브·스킬 레벨 유지).
> If you are on 0.0.4, replacing `Dskill.dll` is enough — config, saves and skill levels are kept.

## AI 제작 표시 / AI disclosure

이 모드의 코드·문서·번역은 **AI 코딩 에이전트(Cline)** 와 협업해 제작했습니다.
The code, documentation and translations were created in collaboration with an **AI coding agent (Cline)**.
설계·밸런스 결정과 게임 내 검증은 제작자가 진행했습니다 / Design, balance and in-game verification by the author.
