namespace Dskill
{
    /// <summary>Deutsch (German)</summary>
    internal static class LocDe
    {
        public const string Table = @"
# ==== UI ====
        ui.title|★ Duckov Skill   —   Titelleiste zum Verschieben ziehen
ui.series|Baum: {0}
ui.total|Gesamt Lv.{0} / {1}
ui.growth|Wachstum: {0}
        ui.perLevel|Pro Stufe: {0}
ui.xpStage|EP-Tempo: Stufe {0}/5 (x{1})
ui.xpStageHint|1 = am schnellsten, 5 = Standard
ui.tabs|[F7] Baum wechseln    [{0} / ESC] schließen
ui.footer|EP werden beim Speichern im Speicherplatz abgelegt.
cat.body|Körper
cat.combat|Kampf
cat.utility|Praktisch
# ==== Vererbung ====
grade.6|Legendär
grade.5|Heroisch
grade.4|Meister
grade.3|Geübt
grade.2|Anfänger
grade.1|Einsteiger
grade.0|Keine
meta.off|Vererbung: deaktiviert
meta.none|Vererbung: noch kein Eintrag aus anderen Spielständen
meta.line|Vererbung {0} (andere Spielstände: {1} / Lv.{2}) -> EP +{3}%
meta.notice|[Vererbung] EP +{0}% durch andere Spielstände ({1})
# ==== Mitteilungen ====
notify.levelup|[Skill] {0} Lv.{1}
notify.elite|  ★Elite erreicht!
# ==== Effekttexte ====
eff.survival|Debuff-Widerstand {0}%, erlittener Feuer-/Giftschaden -{1}%
eff.survivalElite|, Blutungsimmunität
        eff.elemental|Erlittener Elementarschaden -{0}%
        eff.elementalElite|, Elite -5% extra
eff.repair|Max. Haltbarkeitsverlust -{0}%
eff.barter|Verkaufspreis +{0}%
eff.throwing|Wurfweite +{0}%, Explosionsschaden +{1}%
eff.looting|Item-Erkennungszeit -{0}%
eff.dash|Abklingzeit -{0}%, Aktionszeit -{1}%, Ausdauerkosten -{2}%
eff.melee|Nahkampf-Angriffstempo +{0}%
eff.hideout|Händler-Nachschub-Abklingzeit -{0}%
eff.hideoutElite|, Abbauzeit -{0}%
eff.crafting|Zusätzliche Ausgabechance {0}%
eff.healSpeed|Heilgeschwindigkeit +{0}%
eff.value|{0} {1}
# ==== Werte ====
stat.MaxWeight|Max. Gewicht
stat.InventoryCapacity|Inventarplätze
stat.StaminaDrainRate|Ausdauerverbrauch
stat.StaminaRecoverRate|Ausdauererholung
stat.MaxHealth|Max. Gesundheit
stat.HealGain|Heileffizienz
stat.GunDamageMultiplier|Schusswaffenschaden
stat.GunCritRateGain|Schusswaffen-Kritchance
stat.RecoilControl|Rückstoßkontrolle
stat.RecoilScaleV|Vertikaler Rückstoß
stat.RecoilScaleH|Horizontaler Rückstoß
stat.GunDistanceMultiplier|Schusswaffenreichweite
stat.GunScatterMultiplier|Kugelstreuung
stat.GunCritDamageGain|Kopfschussschaden
stat.MeleeDamageMultiplier|Nahkampfschaden
stat.MeleeCritRateGain|Nahkampf-Kritchance
stat.WalkSpeed|Bewegungsgeschwindigkeit
stat.RunSpeed|Bewegungsgeschwindigkeit
stat.WalkSoundRange|Gehgeräusch
stat.RunSoundRange|Laufgeräusch
stat.SoundRange|Geräuschradius
stat.BodyArmor|Körperpanzerung
stat.HeadArmor|Kopfpanzerung
stat.DurabilityCost|Haltbarkeitsverbrauch
stat.BuffChance|Debuff-Chance
stat.ElementFactor_Fire|Erlittener Feuerschaden
stat.ElementFactor_Poison|Erlittener Giftschaden
stat.ElementFactor_Physics|Erlittener physischer Schaden
        stat.ElementFactor_Electricity|Erlittener Elektroschaden
        stat.ElementFactor_Ice|Erlittener Eisschaden
        stat.ElementFactor_Ghost|Erlittener Geistschaden
        stat.ElementFactor_Space|Erlittener Raumschaden
stat.BleedChance|Blutungschance
stat.ReloadSpeedGain|Nachladegeschwindigkeit
stat.EnergyCost|Hungerverbrauch
stat.WaterCost|Durstverbrauch
stat.NightVisionAbility|Nachtsicht
stat.ViewDistance|Sichtweite
stat.SenseRange|Erkennungsradius
stat.HearingAbility|Gehör
# ==== 24 Fertigkeiten ====
skill.strength.name|Stärke
        skill.strength.trigger|Bewegen mit über 60% Gewicht (ab 80% schneller)
skill.strength.elite|Inventar +5 Plätze, Bewegungsgeschwindigkeit +10%
skill.endurance.name|Ausdauer
skill.endurance.trigger|Beim Rennen
skill.endurance.elite|Ausdauererholung +30%
skill.vitality.name|Vitalität
skill.vitality.trigger|Wenn du Schaden erleidest (10 Pkt. pro Schaden)
skill.vitality.elite|Max. Gesundheit +5
skill.health.name|Heilung
skill.health.trigger|Wenn Heilmittel deine Gesundheit auffüllen (10 Pkt. pro HP)
skill.health.elite|Heilgeschwindigkeit +10% (zusätzlich)
skill.metabolism.name|Stoffwechsel
skill.metabolism.trigger|Wenn Essen oder Wasser Hunger/Durst füllt (20 Pkt. pro Punkt)
skill.metabolism.elite|Kein Schaden bei Hunger/Durst 0
skill.dash.name|Rollen
skill.dash.trigger|Wenn du rollst (4 Pkt. pro Rolle)
skill.dash.elite|Abklingzeit, Aktionszeit und Ausdauerkosten je -10% mehr
skill.nightvision.name|Nachtsicht
skill.nightvision.trigger|Bei Aktivität in der Nacht (2 Pkt./s)
skill.nightvision.elite|Nachtsichtfähigkeit +0.2 (hebt den Nachtmalus vollständig auf)
skill.assault.name|Schütze
skill.assault.trigger|Nur wenn ein Schuss trifft
skill.assault.elite|Schusswaffen-Kritchance +10%
skill.recoil.name|Rückstoßkontrolle
skill.recoil.trigger|Wenn du schießt (2 Pkt. pro Schuss, 4 Pkt. beim Zielen)
skill.recoil.elite|Vertikaler und horizontaler Rückstoß -15%
skill.marksmanship.name|Präzisionsschuss
skill.marksmanship.trigger|Treffer aus 20 m oder mehr (150 Pkt., +200 bei Kopfschuss)
skill.marksmanship.elite|Kopfschussschaden +10%
skill.melee.name|Nahkampf
skill.melee.trigger|Wenn ein Nahkampfangriff trifft (30 Pkt., +60 bei Nahkampf-Kill)
skill.melee.elite|Bewegungsgeschwindigkeit +10% (zusätzlich)
skill.throwing.name|Wurfwaffen
skill.throwing.trigger|Wenn du Sprengstoff wirfst (100 Pkt., 2 Pkt. pro Explosionsschaden)
skill.throwing.elite|Reichweite +20% (zusätzlich), 30% Chance auf Sofortzündung
skill.perception.name|Wahrnehmung
skill.perception.trigger|Wenn du ein Geräusch bemerkst (4 Pkt., max. 1x pro Sekunde)
skill.perception.elite|Sicht und Erkennung +10%, Gehör +20% (zusätzlich)
skill.covert.name|Tarnbewegung
skill.covert.trigger|Beim Gehen (Rennen ausgenommen, 1 Pkt./s)
skill.covert.elite|Geh- und Laufgeräusch -25% mehr (insgesamt -45% / -35%)
skill.armor.name|Panzerung
skill.armor.trigger|Wenn du mit Panzerung Schaden erleidest (12 Pkt. pro Schaden)
skill.armor.elite|Erlittener physischer Schaden -10%
        skill.elemental.name|Elementanpassung
        skill.elemental.trigger|Wenn du Elementarschaden erleidest (Feuer/Gift/Elektro/Eis/Geist/Raum) (20 Pkt. pro Schaden)
        skill.elemental.elite|Elementarschaden -5% mehr (insgesamt -15% je Element)
skill.survival.name|Überleben
skill.survival.trigger|Bei einem Statuseffekt (Blutung/Gift/Brand) (300 Pkt., 12 Pkt. pro Tick)
skill.survival.elite|Blutungsimmunität (30% Debuff-Widerstand ist der Basiseffekt)
skill.reload.name|Nachladen
skill.reload.trigger|Wenn du das Nachladen beendest (40 Pkt.)
skill.reload.elite|Nachladegeschwindigkeit +10% (zusätzlich)
skill.repair.name|Reparatur
        skill.repair.trigger|Wenn du Ausrüstung reparierst (5 Pkt. pro Haltbarkeit)
skill.repair.elite|Kein Verlust der max. Haltbarkeit
skill.barter.name|Feilschen
skill.barter.trigger|Beim Kaufen oder Verkaufen (5 Pkt. pro 1.000 Handel)
skill.barter.elite|Schwarzmarkt-Abklingzeit -50%
skill.looting.name|Plündern
skill.looting.trigger|Beim Durchsuchen von Kisten oder Leichen (20 Pkt. pro Fund, 10 Pkt. pro Aufnahme)
        skill.looting.elite|Beim Öffnen: 50% Chance, alle Items sofort zu erkennen
skill.hideout.name|Versteck
skill.hideout.trigger|Aufenthalt im Stützpunkt (0.5 Pkt./s) + 500 Pkt. pro Bau oder Ausbau
skill.hideout.elite|Bitcoin-Miner-Produktionszeit -20%
skill.fishing.name|Angeln
skill.fishing.trigger|Wenn du einen Fisch fängst (250 Pkt. pro Fisch)
skill.fishing.elite|Angelkunst und Glück je +20% (zusätzlich)
skill.crafting.name|Herstellen
skill.crafting.trigger|Herstellen (nach Materialwert) + jedes neue Rezept
skill.crafting.elite|Zusätzliche Ausgabechance +20% (insgesamt 50%)

";
    }
}
