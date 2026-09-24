namespace Dskill
{
    /// <summary>Français (French)</summary>
    internal static class LocFr
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ Système de compétences style Tarkov   —   glissez la barre de titre pour déplacer
ui.series|Branche : {0}
ui.total|Total Lv.{0} / {1}
ui.growth|Croissance : {0}
ui.xpStage|Taux d'EXP : palier {0}/5 (x{1})
ui.xpStageHint|1 = le plus rapide, 5 = par défaut
ui.tabs|[F7] changer de branche    [{0} / ESC] fermer
ui.footer|L'expérience est enregistrée dans l'emplacement lors de la sauvegarde.
cat.body|Corps
cat.combat|Combat
cat.utility|Pratique
# ==== Héritage ====
grade.6|Légendaire
grade.5|Héroïque
grade.4|Expert
grade.3|Entraîné
grade.2|Novice
grade.1|Débutant
grade.0|Aucun
meta.off|Héritage : désactivé
meta.none|Héritage : aucune donnée d'autre sauvegarde
meta.line|Héritage {0} (autres sauvegardes : {1} / Lv.{2}) -> XP +{3}%
meta.notice|[Héritage] XP +{0}% grâce aux autres sauvegardes ({1})
# ==== Notifications ====
notify.levelup|[Compétence] {0} Lv.{1}
notify.elite|  ★Niveau élite atteint !
# ==== Textes d'effet ====
eff.survival|Résistance aux malus {0}%, dégâts de feu/poison subis -{1}%
eff.survivalElite|, immunité aux saignements
eff.repair|Perte de durabilité max. -{0}%
eff.barter|Prix de vente +{0}%
eff.throwing|Distance de lancer +{0}%, dégâts d'explosion +{1}%
eff.looting|Temps de détection des objets -{0}%
eff.dash|Recharge -{0}%, temps d'action -{1}%, coût d'endurance -{2}%
eff.hideout|Réapprovisionnement du marchand -{0}%
eff.hideoutElite|, temps d'extraction -{0}%
eff.crafting|Chance de production supplémentaire {0}%
eff.healSpeed|Vitesse de soin +{0}%
eff.value|{0} {1}
# ==== Statistiques ====
stat.MaxWeight|Poids max.
stat.InventoryCapacity|Emplacements d'inventaire
stat.StaminaDrainRate|Consommation d'endurance
stat.StaminaRecoverRate|Récupération d'endurance
stat.MaxHealth|Santé max.
stat.HealGain|Efficacité de soin
stat.GunDamageMultiplier|Dégâts d'arme
stat.GunCritRateGain|Chance de critique d'arme
stat.RecoilControl|Contrôle du recul
stat.RecoilScaleV|Recul vertical
stat.RecoilScaleH|Recul horizontal
stat.GunDistanceMultiplier|Portée de l'arme
stat.GunScatterMultiplier|Dispersion des balles
stat.GunCritDamageGain|Dégâts à la tête
stat.MeleeDamageMultiplier|Dégâts au corps à corps
stat.MeleeCritRateGain|Critique au corps à corps
stat.WalkSpeed|Vitesse de déplacement
stat.RunSpeed|Vitesse de déplacement
stat.WalkSoundRange|Bruit de marche
stat.RunSoundRange|Bruit de course
stat.SoundRange|Rayon sonore
stat.BodyArmor|Armure corporelle
stat.HeadArmor|Armure de tête
stat.DurabilityCost|Consommation de durabilité
stat.BuffChance|Chance de malus
stat.ElementFactor_Fire|Dégâts de feu subis
stat.ElementFactor_Poison|Dégâts de poison subis
stat.ElementFactor_Physics|Dégâts physiques subis
stat.BleedChance|Chance de saignement
stat.ReloadSpeedGain|Vitesse de rechargement
stat.EnergyCost|Consommation de faim
stat.WaterCost|Consommation d'eau
stat.NightVisionAbility|Vision nocturne
stat.ViewDistance|Distance de vision
stat.SenseRange|Rayon de détection
stat.HearingAbility|Ouïe
# ==== 23 compétences ====
skill.strength.name|Force
skill.strength.trigger|Se déplacer avec plus de 70% de poids (plus rapide si plus lourd)
skill.strength.elite|Inventaire +5 emplacements, vitesse de déplacement +10%
skill.endurance.name|Endurance
skill.endurance.trigger|En courant
skill.endurance.elite|Récupération d'endurance +30%
skill.vitality.name|Vitalité
skill.vitality.trigger|En subissant des dégâts (10 pts par dégât)
skill.vitality.elite|Santé max. +5
skill.health.name|Soin
skill.health.trigger|En récupérant de la santé avec un soin (10 pts par point)
skill.health.elite|Vitesse de soin +10% (supplémentaire)
skill.metabolism.name|Métabolisme
skill.metabolism.trigger|En remplissant faim/soif avec nourriture ou eau (10 pts par point)
skill.metabolism.elite|Aucun dégât même à faim/soif 0
skill.dash.name|Roulade
skill.dash.trigger|En utilisant la roulade (4 pts par fois)
skill.dash.elite|Recharge, temps d'action et coût d'endurance -10% de plus chacun
skill.nightvision.name|Vision nocturne
skill.nightvision.trigger|En activité la nuit (2 pts/s)
skill.nightvision.elite|Vision nocturne +0.2 (annule totalement le malus nocturne)
skill.assault.name|Tir
skill.assault.trigger|Uniquement quand la balle touche
skill.assault.elite|Chance de critique d'arme +10%
skill.recoil.name|Contrôle du recul
skill.recoil.trigger|En tirant (2 pts par balle, 4 pts en visant)
skill.recoil.elite|Recul vertical et horizontal -15%
skill.marksmanship.name|Tir de précision
skill.marksmanship.trigger|Toucher à 20 m ou plus (150 pts, +200 pour un tir à la tête)
skill.marksmanship.elite|Dégâts à la tête +10%
skill.melee.name|Corps à corps
skill.melee.trigger|Quand une attaque au corps à corps touche (30 pts, +60 pour une élimination)
skill.melee.elite|Vitesse de déplacement +10% (supplémentaire)
skill.throwing.name|Lancers
skill.throwing.trigger|En lançant un explosif (100 pts, 2 pts par dégât d'explosion)
skill.throwing.elite|Portée +20% (supplémentaire), 30% de chance d'exploser aussitôt
skill.perception.name|Perception
skill.perception.trigger|En remarquant un son proche (4 pts, au plus 1 fois par seconde)
skill.perception.elite|Vision et détection +10%, ouïe +20% (supplémentaire)
skill.covert.name|Déplacement furtif
skill.covert.trigger|En marchant (course exclue, 1 pt/s)
skill.covert.elite|Bruit de marche et de course -25% de plus (-45% / -35% au total)
skill.armor.name|Défense
skill.armor.trigger|En subissant des dégâts avec armure (12 pts par dégât)
skill.armor.elite|Dégâts physiques subis -10%
skill.survival.name|Survie
skill.survival.trigger|En subissant un état (saignement/poison/brûlure) (300 pts, 12 pts par tick)
skill.survival.elite|Immunité aux saignements (30% de résistance est l'effet de base)
skill.reload.name|Rechargement
skill.reload.trigger|En terminant un rechargement (40 pts)
skill.reload.elite|Vitesse de rechargement +10% (supplémentaire)
skill.repair.name|Réparation
skill.repair.trigger|En réparant de l'équipement (30 pts par point de durabilité)
skill.repair.elite|Aucune perte de durabilité max.
skill.barter.name|Marchandage
skill.barter.trigger|En achetant ou vendant (5 pts par 1 000 échangés)
skill.barter.elite|Réapprovisionnement du marché noir -50%
skill.looting.name|Fouille
skill.looting.trigger|En fouillant caisses et corps (20 pts par trouvaille, 10 pts par prise)
skill.looting.elite|50% de chance de détection immédiate
skill.hideout.name|Planque
skill.hideout.trigger|Rester à la base (0.5 pt/s) + 500 pts par construction ou amélioration
skill.hideout.elite|Temps de production du mineur de bitcoins -20%
skill.fishing.name|Pêche
skill.fishing.trigger|En attrapant un poisson (250 pts par poisson)
skill.fishing.elite|Compétence de pêche et chance +20% chacune (supplémentaire)
skill.crafting.name|Fabrication
skill.crafting.trigger|Fabrication (selon la valeur des matériaux) + chaque nouvelle recette
skill.crafting.elite|Chance de production supplémentaire +20% (50% au total)

";
    }
}
