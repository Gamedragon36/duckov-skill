namespace Dskill
{
    /// <summary>Português (Brasil)</summary>
    internal static class LocPtBr
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ Sistema de habilidades estilo Tarkov   —   arraste a barra de título para mover
ui.series|Ramo: {0}
ui.total|Total Lv.{0} / {1}
ui.growth|Crescimento: {0}
ui.xpStage|Taxa de EXP: nível {0}/5 (x{1})
ui.xpStageHint|1 = mais rápido, 5 = padrão
ui.tabs|[F7] trocar de ramo    [{0} / ESC] fechar
ui.footer|A experiência é salva no espaço ao salvar o jogo.
cat.body|Corpo
cat.combat|Combate
cat.utility|Prático
# ==== Herança ====
grade.6|Lendário
grade.5|Heroico
grade.4|Especialista
grade.3|Treinado
grade.2|Novato
grade.1|Iniciante
grade.0|Nenhum
meta.off|Herança: desativada
meta.none|Herança: ainda sem registro de outros saves
meta.line|Herança {0} (outros saves: {1} / Lv.{2}) -> XP +{3}%
meta.notice|[Herança] XP +{0}% graças aos outros saves ({1})
# ==== Avisos ====
notify.levelup|[Habilidade] {0} Lv.{1}
notify.elite|  ★Nível de elite alcançado!
# ==== Textos de efeito ====
eff.survival|Resistência a debuffs {0}%, dano recebido de fogo/veneno -{1}%
eff.survivalElite|, imunidade a sangramento
eff.repair|Perda de durabilidade máx. -{0}%
eff.barter|Preço de venda +{0}%
eff.throwing|Distância de arremesso +{0}%, dano de explosão +{1}%
eff.looting|Tempo de detecção de itens -{0}%
eff.dash|Recarga -{0}%, custo de stamina -{1}%
eff.hideout|Reabastecimento do comerciante -{0}%
eff.hideoutElite|, tempo de mineração -{0}%
eff.crafting|Chance de produção extra {0}%
eff.value|{0} {1}
# ==== Atributos ====
stat.MaxWeight|Peso máximo
stat.InventoryCapacity|Espaços do inventário
stat.StaminaDrainRate|Consumo de stamina
stat.StaminaRecoverRate|Recuperação de stamina
stat.MaxHealth|Saúde máxima
stat.HealGain|Eficiência de cura
stat.GunDamageMultiplier|Dano de arma
stat.GunCritRateGain|Chance de crítico da arma
stat.RecoilControl|Controle de recuo
stat.RecoilScaleV|Recuo vertical
stat.RecoilScaleH|Recuo horizontal
stat.GunDistanceMultiplier|Alcance da arma
stat.GunScatterMultiplier|Dispersão das balas
stat.GunCritDamageGain|Dano na cabeça
stat.MeleeDamageMultiplier|Dano corpo a corpo
stat.MeleeCritRateGain|Crítico corpo a corpo
stat.WalkSpeed|Velocidade de movimento
stat.RunSpeed|Velocidade de movimento
stat.WalkSoundRange|Barulho ao andar
stat.RunSoundRange|Barulho ao correr
stat.SoundRange|Raio de som
stat.BodyArmor|Armadura corporal
stat.HeadArmor|Armadura de cabeça
stat.DurabilityCost|Consumo de durabilidade
stat.BuffChance|Chance de debuff
stat.ElementFactor_Fire|Dano de fogo recebido
stat.ElementFactor_Poison|Dano de veneno recebido
stat.ElementFactor_Physics|Dano físico recebido
stat.BleedChance|Chance de sangramento
stat.ReloadSpeedGain|Velocidade de recarga
stat.EnergyCost|Consumo de fome
stat.WaterCost|Consumo de água
stat.NightVisionAbility|Visão noturna
stat.ViewDistance|Distância de visão
stat.SenseRange|Raio de detecção
stat.HearingAbility|Audição
# ==== 23 habilidades ====
skill.strength.name|Força
skill.strength.trigger|Mover-se com mais de 70% de peso (1 pt/s, 2 pts/s acima de 90%)
skill.strength.elite|Inventário +5 espaços, velocidade de movimento +10%
skill.endurance.name|Resistência
skill.endurance.trigger|Enquanto corre (1 pt/s)
skill.endurance.elite|Recuperação de stamina +30%
skill.vitality.name|Vitalidade
skill.vitality.trigger|Ao receber dano (10 pts por dano)
skill.vitality.elite|Saúde máxima +5
skill.health.name|Cura
skill.health.trigger|Ao recuperar saúde com itens de cura (20 pts por ponto)
skill.health.elite|Eficiência de cura +10%
skill.metabolism.name|Metabolismo
skill.metabolism.trigger|Ao encher fome/sede com comida ou água (20 pts por ponto)
skill.metabolism.elite|Sem dano mesmo com fome/sede em 0
skill.dash.name|Rolamento
skill.dash.trigger|Ao usar o rolamento (8 pts por vez)
skill.dash.elite|Recarga e custo de stamina -20% a mais cada
skill.nightvision.name|Visão noturna
skill.nightvision.trigger|Ativo durante a noite (2 pts/s)
skill.nightvision.elite|Visão noturna +0.2 (anula a penalidade noturna)
skill.assault.name|Tiro
skill.assault.trigger|Ao disparar (4 pts por tiro, +4 ao acertar)
skill.assault.elite|Chance de crítico da arma +10%
skill.recoil.name|Controle de recuo
skill.recoil.trigger|Ao disparar (3 pts por tiro, 6 pts ao mirar)
skill.recoil.elite|Recuo vertical e horizontal -15%
skill.marksmanship.name|Precisão
skill.marksmanship.trigger|Acertar a 20 m ou mais (45 pts, +70 na cabeça)
skill.marksmanship.elite|Dano na cabeça +10%
skill.melee.name|Corpo a corpo
skill.melee.trigger|Quando um ataque corpo a corpo acerta (30 pts, +60 por abate)
skill.melee.elite|Velocidade de movimento +10% (extra)
skill.throwing.name|Arremessos
skill.throwing.trigger|Ao arremessar um explosivo (200 pts, 2 pts por dano de explosão)
skill.throwing.elite|Alcance +20% (extra), 30% de chance de explodir na hora
skill.perception.name|Percepção
skill.perception.trigger|Ao notar um som por perto (4 pts, no máximo 1 vez por segundo)
skill.perception.elite|Visão e detecção +10%, audição +20% (extra)
skill.covert.name|Movimento furtivo
skill.covert.trigger|Ao andar (corrida excluída, 1 pt/s)
skill.covert.elite|Barulho ao andar e correr -25% a mais (-45% / -35% no total)
skill.armor.name|Defesa
skill.armor.trigger|Ao receber dano usando armadura (12 pts por dano)
skill.armor.elite|Dano físico recebido -10%
skill.survival.name|Sobrevivência
skill.survival.trigger|Ao sofrer um estado (sangramento/veneno/queimadura) (300 pts, 12 pts por tick)
skill.survival.elite|Imunidade a sangramento (30% de resistência é o efeito base)
skill.reload.name|Recarga
skill.reload.trigger|Ao concluir a recarga (40 pts)
skill.reload.elite|Velocidade de recarga +10% (extra)
skill.repair.name|Reparo
skill.repair.trigger|Ao reparar equipamento (30 pts por ponto de durabilidade)
skill.repair.elite|Sem perda de durabilidade máxima
skill.barter.name|Pechincha
skill.barter.trigger|Ao comprar ou vender (10 pts por 1.000 negociados)
skill.barter.elite|Reabastecimento do mercado negro -50%
skill.looting.name|Saque
skill.looting.trigger|Ao procurar em caixas ou corpos (40 pts por achado, 15 pts por coleta)
skill.looting.elite|50% de chance de detecção imediata
skill.hideout.name|Esconderijo
skill.hideout.trigger|Ficar na base (1 pt/s) + 500 pts por construção ou melhoria
skill.hideout.elite|Tempo de produção do minerador de bitcoin -20%
skill.fishing.name|Pesca
skill.fishing.trigger|Ao pescar um peixe (250 pts por peixe)
skill.fishing.elite|Habilidade de pesca e sorte +20% cada (extra)
skill.crafting.name|Fabricação
skill.crafting.trigger|200 pts por fabricação + 400 pts por nova receita
skill.crafting.elite|Chance de produção extra +20% (50% no total)

";
    }
}
