namespace Dskill
{
    /// <summary>영어 (기본 언어이자 대체 언어)</summary>
    internal static class LocEn
    {
        public const string Table = @"
# ==== 창(UI) ====
ui.title|★ Tarkov Skill System   —   drag the title bar to move this window
ui.series|Tree: {0}
ui.total|Total Lv.{0} / {1}
ui.growth|Growth: {0}
ui.xpStage|XP difficulty: stage {0}/5 (x{1})
ui.xpStageHint|1 = fastest, 5 = default rate
ui.tabs|[F7] switch tree    [{0} / ESC] close
ui.footer|XP is stored in the save slot when you save the game.
cat.body|Physical
cat.combat|Combat
cat.utility|Utility
# ==== 계승(메타 진행도) ====
grade.6|Legendary
grade.5|Heroic
grade.4|Expert
grade.3|Trained
grade.2|Novice
grade.1|Beginner
grade.0|None
meta.off|Inheritance: disabled
meta.none|Inheritance: no other-save record yet
meta.line|Inheritance {0} (other saves: {1} / Lv.{2}) -> XP +{3}%
meta.notice|[Inheritance] XP +{0}% from your other saves ({1})
# ==== 알림 ====
notify.levelup|[Skill] {0} Lv.{1}
notify.elite|  ★Elite reached!
# ==== 효과 문장 ====
eff.survival|Debuff resistance {0}%, fire/poison damage taken -{1}%
eff.survivalElite|, bleed immunity
eff.repair|Max durability loss -{0}%
eff.barter|Sell price +{0}%
eff.throwing|Throw range +{0}%, explosion damage +{1}%
eff.looting|Item detection time -{0}%
eff.dash|Cooldown -{0}%, action time -{1}%, stamina cost -{2}%
eff.hideout|Trader restock cooldown -{0}%
eff.hideoutElite|, mining time -{0}%
eff.crafting|Extra output chance {0}%
eff.healSpeed|Healing speed +{0}%
eff.value|{0} {1}
# ==== 스탯 이름 ====
stat.MaxWeight|Max weight
stat.InventoryCapacity|Inventory slots
stat.StaminaDrainRate|Stamina drain
stat.StaminaRecoverRate|Stamina recovery
stat.MaxHealth|Max health
stat.HealGain|Healing efficiency
stat.GunDamageMultiplier|Gun damage
stat.GunCritRateGain|Gun crit chance
stat.RecoilControl|Recoil control
stat.RecoilScaleV|Vertical recoil
stat.RecoilScaleH|Horizontal recoil
stat.GunDistanceMultiplier|Gun range
stat.GunScatterMultiplier|Bullet spread
stat.GunCritDamageGain|Headshot damage
stat.MeleeDamageMultiplier|Melee damage
stat.MeleeCritRateGain|Melee crit chance
stat.WalkSpeed|Movement speed
stat.RunSpeed|Movement speed
stat.WalkSoundRange|Walking noise
stat.RunSoundRange|Sprint noise
stat.SoundRange|Noise range
stat.BodyArmor|Body armor
stat.HeadArmor|Head armor
stat.DurabilityCost|Durability cost
stat.BuffChance|Debuff chance
stat.ElementFactor_Fire|Fire damage taken
stat.ElementFactor_Poison|Poison damage taken
stat.ElementFactor_Physics|Physical damage taken
stat.BleedChance|Bleed chance
stat.ReloadSpeedGain|Reload speed
stat.EnergyCost|Hunger drain
stat.WaterCost|Thirst drain
stat.NightVisionAbility|Night vision
stat.ViewDistance|View distance
stat.SenseRange|Detection range
stat.HearingAbility|Hearing
# ==== 스킬 23종 ====
skill.strength.name|Strength
skill.strength.trigger|Move while over 70% weight (faster the heavier you are)
skill.strength.elite|Inventory space +5 slots, movement speed +10%
skill.endurance.name|Endurance
skill.endurance.trigger|While sprinting
skill.endurance.elite|Stamina recovery rate +30%
skill.vitality.name|Vitality
skill.vitality.trigger|When you take damage (10 pts per damage)
skill.vitality.elite|Max health +5
skill.health.name|Healing
skill.health.trigger|When healing items restore your health (10 pts per HP)
skill.health.elite|Healing speed +10% (extra)
skill.metabolism.name|Metabolism
skill.metabolism.trigger|When food or water restores hunger/thirst (20 pts per point)
skill.metabolism.elite|No damage even at zero hunger/thirst
skill.dash.name|Roll
skill.dash.trigger|When you roll (4 pts per roll)
skill.dash.elite|Cooldown, action time and stamina cost -10% more each
skill.nightvision.name|Night Vision
skill.nightvision.trigger|While active at night (2 pts/s)
skill.nightvision.elite|Night vision ability +0.2 (fully cancels the night penalty)
skill.assault.name|Rifleman
skill.assault.trigger|Only when your bullet hits
skill.assault.elite|Gun crit chance +10%
skill.recoil.name|Recoil Control
skill.recoil.trigger|When you fire a gun (2 pts per shot, 4 pts when aiming)
skill.recoil.elite|Vertical and horizontal recoil -15%
skill.marksmanship.name|Marksmanship
skill.marksmanship.trigger|Hit a target from 8 m or more (45 pts, +70 for a headshot)
skill.marksmanship.elite|Headshot damage +10%
skill.melee.name|Melee Combat
skill.melee.trigger|When a melee attack hits (30 pts, +60 for a melee kill)
skill.melee.elite|Movement speed +10% (extra)
skill.throwing.name|Throwables
skill.throwing.trigger|When you throw an explosive (100 pts, 2 pts per explosion damage)
skill.throwing.elite|Range +20% (extra), 30% chance to detonate instantly
skill.perception.name|Awareness
skill.perception.trigger|When you notice a sound nearby (4 pts, at most once per second)
skill.perception.elite|View and detection +10%, hearing +20% (extra)
skill.covert.name|Stealth Movement
skill.covert.trigger|While walking (sprinting excluded, 1 pt/s)
skill.covert.elite|Walking and sprint noise -25% more (-45% / -35% total)
skill.armor.name|Armor
skill.armor.trigger|When you take damage while wearing armor (12 pts per damage)
skill.armor.elite|Physical damage taken -10%
skill.survival.name|Survival
skill.survival.trigger|When you get a status effect (bleed/poison/burn) (300 pts, 12 pts per damage-over-time tick)
skill.survival.elite|Bleed immunity (30% debuff resistance is the base effect)
skill.reload.name|Reloading
skill.reload.trigger|When you finish a reload (40 pts)
skill.reload.elite|Reload speed +10% (extra)
skill.repair.name|Repair
skill.repair.trigger|When you repair gear (30 pts per durability restored)
skill.repair.elite|No max durability loss
skill.barter.name|Bartering
skill.barter.trigger|When you buy or sell items (5 pts per 1,000 traded)
skill.barter.elite|Black market restock cooldown -50%
skill.looting.name|Scavenging
skill.looting.trigger|When you search containers or bodies (20 pts per find, 10 pts per pickup)
skill.looting.elite|50% chance to detect instantly
skill.hideout.name|Hideout
skill.hideout.trigger|Staying at your base (0.5 pt/s) plus 500 pts per building built or upgraded
skill.hideout.elite|Bitcoin miner production time -20%
skill.fishing.name|Fishing
skill.fishing.trigger|When you catch a fish (250 pts per fish)
skill.fishing.elite|Fishing skill and luck +20% each (extra)
skill.crafting.name|Crafting
skill.crafting.trigger|Crafting (scales with the material value) plus each new recipe unlocked
skill.crafting.elite|Extra output chance +20% (50% total)
# ==== config.ini 주석 ====
cfg.header|Tarkov-style skill system - settings
cfg.headerNote1|Edit a value and restart the game to apply it.
cfg.headerNote2|Text after # is a comment and can be deleted.
cfg.maxLevel|Skill max level (cap)
cfg.xpBase|XP needed for level 1
cfg.xpStep|Extra XP required per level
cfg.xpMultiplier|Global XP multiplier (0.5 = half, 2.0 = double)
cfg.hotkey|Key that opens the skill window (e.g. F6, F7, F8)
cfg.notifyLevelUp|Show a notification on level up (true/false)
cfg.metaNote1|Inheritance (roguelike): the total skill levels earned in
cfg.metaNote2|OTHER saves increase your XP gain rate in the current save.
cfg.metaEnabled|Enable this feature (true/false)
cfg.metaPerLevel|XP +0.5% per skill level from other saves
cfg.metaCap|Maximum bonus (%)
cfg.resetNote1|DANGER: set true and start the game once to reset all skill XP.
cfg.resetNote2|The value automatically returns to false after the reset.
cfg.resetSkills|true = reset all skill XP once on the next launch
cfg.skillXpNote|Per-skill XP multiplier (0.5 = half speed, 2.0 = double speed)
cfg.language|UI language: auto (follows the language you chose in the game), or en, ko, zh, zh-hant, ja, de, ru, es, fr, pt-br
cfg.xpStage|XP gain difficulty stage 1-5 (1 = fastest, 5 = default rate)
cfg.uploadNow|Developer: set true and launch to upload this mod to the Steam Workshop once
cfg.craftUnlockXp|XP for unlocking a new recipe (a batch unlock counts once)
cfg.craftXpPerValue|Crafting XP = material value x this value (0.08 = 8%)
cfg.craftXpCap|Maximum XP per craft (0 = no limit)
cfg.hideoutSkills|Skills that can gain XP in the hideout (comma separated) - all others do not gain there


";
    }
}
