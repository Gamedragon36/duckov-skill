namespace Dskill
{
    /// <summary>Español (Latinoamérica)</summary>
    internal static class LocEs
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ Sistema de habilidades estilo Tarkov   —   arrastra la barra de título para mover
ui.series|Rama: {0}
ui.total|Total Lv.{0} / {1}
ui.growth|Crecimiento: {0}
ui.tabs|[F7] cambiar rama    [{0} / ESC] cerrar
ui.footer|La experiencia se guarda en la ranura al guardar la partida.
cat.body|Cuerpo
cat.combat|Combate
cat.utility|Práctico
# ==== Herencia ====
grade.6|Legendario
grade.5|Heroico
grade.4|Experto
grade.3|Entrenado
grade.2|Novato
grade.1|Principiante
grade.0|Ninguno
meta.off|Herencia: desactivada
meta.none|Herencia: aún no hay registros de otras partidas
meta.line|Herencia {0} (otras partidas: {1} / Lv.{2}) -> EXP +{3}%
meta.notice|[Herencia] EXP +{0}% gracias a otras partidas ({1})
# ==== Avisos ====
notify.levelup|[Habilidad] {0} Lv.{1}
notify.elite|  ★¡Nivel de élite alcanzado!
# ==== Textos de efecto ====
eff.survival|Resistencia a debuffs {0}%, daño recibido de fuego/veneno -{1}%
eff.survivalElite|, inmunidad al sangrado
eff.repair|Pérdida de durabilidad máx. -{0}%
eff.barter|Precio de venta +{0}%
eff.throwing|Distancia de lanzamiento +{0}%, daño de explosión +{1}%
eff.looting|Tiempo de detección de objetos -{0}%
eff.dash|Reutilización -{0}%, costo de aguante -{1}%
eff.hideout|Reabastecimiento del comerciante -{0}%
eff.hideoutElite|, tiempo de minado -{0}%
eff.crafting|Probabilidad de producción extra {0}%
eff.value|{0} {1}
# ==== Estadísticas ====
stat.MaxWeight|Peso máximo
stat.InventoryCapacity|Espacios de inventario
stat.StaminaDrainRate|Consumo de aguante
stat.StaminaRecoverRate|Recuperación de aguante
stat.MaxHealth|Salud máxima
stat.HealGain|Eficiencia de curación
stat.GunDamageMultiplier|Daño de arma
stat.GunCritRateGain|Prob. crítica de arma
stat.RecoilControl|Control del retroceso
stat.RecoilScaleV|Retroceso vertical
stat.RecoilScaleH|Retroceso horizontal
stat.GunDistanceMultiplier|Alcance del arma
stat.GunScatterMultiplier|Dispersión de balas
stat.GunCritDamageGain|Daño de disparo a la cabeza
stat.MeleeDamageMultiplier|Daño cuerpo a cuerpo
stat.MeleeCritRateGain|Prob. crítica cuerpo a cuerpo
stat.WalkSpeed|Velocidad de movimiento
stat.RunSpeed|Velocidad de movimiento
stat.WalkSoundRange|Ruido al caminar
stat.RunSoundRange|Ruido al correr
stat.SoundRange|Radio de sonido
stat.BodyArmor|Armadura corporal
stat.HeadArmor|Armadura de cabeza
stat.DurabilityCost|Consumo de durabilidad
stat.BuffChance|Probabilidad de debuff
stat.ElementFactor_Fire|Daño de fuego recibido
stat.ElementFactor_Poison|Daño de veneno recibido
stat.ElementFactor_Physics|Daño físico recibido
stat.BleedChance|Probabilidad de sangrado
stat.ReloadSpeedGain|Velocidad de recarga
stat.EnergyCost|Consumo de hambre
stat.WaterCost|Consumo de agua
stat.NightVisionAbility|Visión nocturna
stat.ViewDistance|Distancia de visión
stat.SenseRange|Radio de detección
stat.HearingAbility|Oído
# ==== 23 habilidades ====
skill.strength.name|Fuerza
skill.strength.trigger|Moverse con más del 70% de peso (1 pto/s, 2 ptos/s con más del 90%)
skill.strength.elite|Inventario +5 espacios, velocidad de movimiento +10%
skill.endurance.name|Aguante
skill.endurance.trigger|Mientras corres (1 pto/s)
skill.endurance.elite|Recuperación de aguante +30%
skill.vitality.name|Vitalidad
skill.vitality.trigger|Al recibir daño (10 ptos por punto de daño)
skill.vitality.elite|Salud máxima +5
skill.health.name|Curación
skill.health.trigger|Al recuperar salud con objetos curativos (20 ptos por punto)
skill.health.elite|Eficiencia de curación +10%
skill.metabolism.name|Metabolismo
skill.metabolism.trigger|Al llenar hambre/sed con comida o agua (20 ptos por punto)
skill.metabolism.elite|Sin daño aunque hambre y sed estén en 0
skill.dash.name|Rodar
skill.dash.trigger|Al rodar (8 ptos por vez)
skill.dash.elite|Reutilización y costo de aguante -20% más cada uno
skill.nightvision.name|Visión nocturna
skill.nightvision.trigger|Al estar activo de noche (2 ptos/s)
skill.nightvision.elite|Visión nocturna +0.2 (anula la penalización nocturna)
skill.assault.name|Tiro
skill.assault.trigger|Al disparar (4 ptos por disparo, +4 al acertar)
skill.assault.elite|Prob. crítica de arma +10%
skill.recoil.name|Control del retroceso
skill.recoil.trigger|Al disparar (3 ptos por disparo, 6 ptos apuntando)
skill.recoil.elite|Retroceso vertical y horizontal -15%
skill.marksmanship.name|Puntería
skill.marksmanship.trigger|Acertar desde 20 m o más (45 ptos, +70 por disparo a la cabeza)
skill.marksmanship.elite|Daño de disparo a la cabeza +10%
skill.melee.name|Cuerpo a cuerpo
skill.melee.trigger|Al acertar un golpe cuerpo a cuerpo (30 ptos, +60 por muerte)
skill.melee.elite|Velocidad de movimiento +10% (extra)
skill.throwing.name|Lanzables
skill.throwing.trigger|Al lanzar un explosivo (200 ptos, 2 ptos por punto de daño)
skill.throwing.elite|Alcance +20% (extra), 30% de probabilidad de detonar al instante
skill.perception.name|Percepción
skill.perception.trigger|Al notar un sonido cercano (4 ptos, máximo 1 vez por segundo)
skill.perception.elite|Visión y detección +10%, oído +20% (extra)
skill.covert.name|Movimiento sigiloso
skill.covert.trigger|Al caminar (sin correr, 1 pto/s)
skill.covert.elite|Ruido al caminar y correr -25% más (-45% / -35% en total)
skill.armor.name|Defensa
skill.armor.trigger|Al recibir daño con armadura (12 ptos por punto de daño)
skill.armor.elite|Daño físico recibido -10%
skill.survival.name|Supervivencia
skill.survival.trigger|Al sufrir un estado (sangrado/veneno/quemadura) (300 ptos, 12 ptos por tick)
skill.survival.elite|Inmunidad al sangrado (30% de resistencia es el efecto base)
skill.reload.name|Recarga
skill.reload.trigger|Al terminar de recargar (40 ptos)
skill.reload.elite|Velocidad de recarga +10% (extra)
skill.repair.name|Reparación
skill.repair.trigger|Al reparar equipo (30 ptos por punto de durabilidad)
skill.repair.elite|Sin pérdida de durabilidad máxima
skill.barter.name|Regateo
skill.barter.trigger|Al comprar o vender (10 ptos por cada 1.000)
skill.barter.elite|Reabastecimiento del mercado negro -50%
skill.looting.name|Saqueo
skill.looting.trigger|Al buscar en cajas o cuerpos (40 ptos por hallazgo, 15 ptos por recogida)
skill.looting.elite|50% de probabilidad de detección instantánea
skill.hideout.name|Refugio
skill.hideout.trigger|Estar en la base (1 pto/s) + 500 ptos por construir o mejorar
skill.hideout.elite|Tiempo de producción del minero de bitcoins -20%
skill.fishing.name|Pesca
skill.fishing.trigger|Al pescar un pez (250 ptos por pez)
skill.fishing.elite|Habilidad de pesca y suerte +20% cada una (extra)
skill.crafting.name|Fabricación
skill.crafting.trigger|200 ptos por fabricación + 400 ptos por nueva receta
skill.crafting.elite|Probabilidad de producción extra +20% (50% en total)

";
    }
}
