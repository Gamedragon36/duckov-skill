# Duckov Skill — a Tarkov-style skill system for Escape From Duckov

> 🤖 **This mod was created in collaboration with an AI coding agent (Cline).**
> The code, documentation and translations were written by the AI agent; the design,
> balance decisions and in-game verification were made by the author.
> (Steam Workshop rule 5 — AI-generated content must be disclosed.)
>
> 한국어 설명: [README.md](README.md)

## Features

- **23 skills**, each up to **Lv.20** — level up **by playing** (shooting, running, looting, fishing, crafting…)
- Reaching Lv.20 unlocks a **★Elite** bonus for that skill
- Bonuses are applied through the game's own stat-modifier system (no game-code patching, no Harmony)
- **F6** opens the skill window (drag the title bar to move it), **F7** switches category
- Level-ups appear through the game's own notification UI
- XP is stored **per save slot**
- **Roguelike inheritance**: skill levels earned in *other* save slots increase the XP gain rate of the current run

## Installation

1. Copy the mod folder into `Duckov_Data\Mods\`:
   ```
   <game>\Duckov_Data\Mods\Dskill\
       Dskill.dll
       info.ini
       preview.png
   ```
2. Start the game → **Mods** menu → enable **Duckov Skill** (new mods are OFF by default)
3. Enter the hideout or a raid and the skills start working. Press **F6** to open the window.

`config.ini` is created next to the DLL on the first launch.

## Skills

### Physical

| Icon | Skill | How to level up | Max effect | ★Elite |
|---|---|---|---|---|
| ◆ | Strength | move while above 70% carry weight | max weight **+20**, movement **+10%** | bag **+5 slots**, movement **+10%** |
| ▲ | Endurance | sprint | stamina drain **-40%** | stamina regen **+30%** |
| ● | Vitality | take damage | max health **+20** | **+5** |
| ＋ | Healing | heal with items | healing **+40%** | **+10%** (total +50%) |
| ◉ | Metabolism | eat / drink | hunger & water drain **-30%**, movement **+10%** | **no starvation damage** |
| ○ | Dash | dodge-roll | cooldown **-30%**, stamina cost **-30%** | each **-20%** (total -50%) |
| ▼ | Night Vision | spend time at night (raids) | night vision ability **+0.3** | **+0.2** (total 1.0 = no night penalty) |

### Combat

| Icon | Skill | How to level up | Max effect | ★Elite |
|---|---|---|---|---|
| ★ | Assault | fire your gun | gun damage **+15%** | gun crit rate **+10%** |
| ▽ | Recoil Control | fire your gun (ADS counts more) | recoil control **+0.2** | vertical/horizontal recoil **-15%** |
| ◎ | Marksmanship | hit targets beyond 20 m | gun range **+30%**, spread **-20%** | headshot damage **+10%** |
| ◀ | Melee | hit with melee weapons | melee damage **+50%**, melee crit **+50%**, movement **+10%** | movement **+10%** more |
| ▶ | Throwing | throw explosives | throw distance **+30%**, explosion damage **+50%** | distance **+20%**, **30% chance of instant detonation** |
| ◁ | Perception | hear sounds around you | view/sense range **+20%**, hearing **+30%** | **+10 / +10 / +20%** |

### Practical

| Icon | Skill | How to level up | Max effect | ★Elite |
|---|---|---|---|---|
| ◐ | Covert Movement | walk (not sprint) | walk sound **-20%**, run sound **-10%** | walk/run sound **-25%** more |
| ◇ | Armor | take damage while wearing armor | body & head armor **+2.0** | physical damage taken **-10%** |
| ☆ | Survival | get debuffs | **30% debuff resistance**, fire & poison damage **-10%** | **bleed immunity** |
| ◑ | Reloading | finish a reload | reload speed **+30%** | **+10%** more |
| ◈ | Repair | repair gear | max durability loss **-50%** | **no max durability loss** |
| ※ | Bartering | buy & sell items | sale price **+10%** | black-market refresh **-50%** |
| ▣ | Looting | reveal items in containers | item detection time **-50%** | **50% chance of instant detection** |
| ■ | Hideout | stay at the hideout / construct buildings | merchant restock cooldown **-50%** | bitcoin miner time **-20%** |
| ▪ | Fishing | catch fish | fishing ability **+30%**, fishing luck **+20%** | **+20%** each |
| □ | Crafting | craft items / unlock recipes | **30% chance of a bonus item** | **+20%** (total 50%) |

## Growth speed

| Item | Value |
|---|---|
| XP needed per level | 300 for level 1, +65 per level (300, 365, 430, … 1,535) |
| Total XP to Lv.20 | **18,350** points |
| Approximate pace | about **5 hours** of the related activity (slower in normal play) |

## Roguelike inheritance

Skill levels earned in **other save slots** increase the XP gain rate of the run you are playing now
(the current slot is excluded).

| Item | Value |
|---|---|
| Bonus | **+0.5% XP per level** in other slots, capped at **+150%** |
| Grades | 1–49 Novice / 50–129 Initiate / 130–249 Trained / 250–399 Skilled / 400–599 Hero / 600+ Legend |
| Where to see it | top of the F6 window, e.g. `Hero (2 saves / Lv.420) → XP +150%` |
| Start notice | `[Inheritance] XP +60% from other saves (Skilled)` once per session |

## Configuration (`config.ini`)

```ini
[general]
max_level = 20          # max skill level
xp_base = 300           # XP for level 1
xp_step = 65            # XP added per level
xp_multiplier = 1.0     # global XP gain multiplier (0.5 = half, 2.0 = double)
hotkey = F6             # key that opens the skill window
notify_levelup = true   # show level-up notifications

[meta]
meta_enabled = true          # roguelike inheritance
meta_bonus_per_level = 0.5   # +0.5% XP per level in other save slots
meta_bonus_cap = 150         # maximum bonus (%)

[skill_xp]
strength = 1.0          # per-skill XP multiplier
endurance = 1.0
...

# WARNING: setting this to true and launching the game once resets every skill to 0.
# The value is set back to false automatically after the reset.
reset_skills = false
```

### Resetting skills (`reset_skills`)

Set `reset_skills = true` and launch the game once: **every skill is reset to 0**, and the flag is
automatically set back to `false`. The save file is written by the game/mod itself, so no manual
save editing is needed.

## Build from source

```powershell
cd "C:\any programs\duckov\duckov-skill"
dotnet build -c Release
```

- Target framework `netstandard2.1`; references the game's own assemblies (`TeamSoda.*`, `ItemStatsSystem.dll`, `Unity*`)
- The `DeployMod` target in the `.csproj` copies the DLL, `info.ini` and `preview.png` into
  `Duckov_Data\Mods\Dskill` after every build
- Change `<DuckovPath>` in the `.csproj` if your game is installed somewhere else

## Notes and limitations

| Item | Note |
|---|---|
| Farming / garden | The game contains crop/garden code, strings and seed items, but **no level actually places a garden object**, so the feature cannot be used. The hideout skill therefore does not award XP for harvesting |
| Stat targets (QA) | Only stats the game actually reads **from the player character** are used. Effects that would have to target a weapon/bullet stat are replaced by working equivalents (Armor elite, Covert Movement elite) |
| Survival "debuff -30%" | Implemented in code (chance to cancel a debuff) because the game has no matching character stat |
| Fishing | XP is granted when you actually obtain a fish item (tag `Fish`) |
| Crafting | Bonus items are created with the game's own item-instantiation API |
| Saving | XP is stored **per save slot**; deleting a save deletes its skill progress |
| Reflection | Merchant cooldown and bitcoin-miner time are read/set through reflection. If a future game update renames those fields the effect is skipped and a warning is logged (everything else keeps working) |
| Mod conflicts | No Harmony, no game-code patching, unique stat-modifier tokens — designed to coexist with other mods |

## License

No license file is included. If you want to reuse this code, contact the author first.


