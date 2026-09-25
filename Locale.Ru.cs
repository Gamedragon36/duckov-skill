namespace Dskill
{
    /// <summary>Русский (Russian)</summary>
    internal static class LocRu
    {
        public const string Table = @"
# ==== UI ====
        ui.title|★ Duckov Skill   —   перетащите заголовок, чтобы переместить окно
ui.series|Ветка: {0}
ui.total|Всего Lv.{0} / {1}
ui.growth|Рост: {0}
        ui.perLevel|За уровень: {0}
ui.xpStage|Скорость опыта: ступень {0}/5 (x{1})
ui.xpStageHint|1 = быстрее всего, 5 = по умолчанию
ui.tabs|[F7] смена ветки    [{0} / ESC] закрыть
ui.footer|Опыт сохраняется в слоте вместе с сохранением игры.
cat.body|Тело
cat.combat|Бой
cat.utility|Практика
# ==== Наследие ====
grade.6|Легенда
grade.5|Герой
grade.4|Мастер
grade.3|Опытный
grade.2|Новичок
grade.1|Начальный
grade.0|Нет
meta.off|Наследие: отключено
meta.none|Наследие: записей из других сохранений пока нет
meta.line|Наследие {0} (других сохранений: {1} / Lv.{2}) -> опыт +{3}%
meta.notice|[Наследие] Опыт +{0}% из других сохранений ({1})
# ==== Уведомления ====
notify.levelup|[Навык] {0} Lv.{1}
notify.elite|  ★Достигнут элитный уровень!
# ==== Тексты эффектов ====
eff.survival|Сопротивление дебаффам {0}%, получаемый урон огнём/ядом -{1}%
eff.survivalElite|, иммунитет к кровотечению
        eff.elemental|Получаемый урон от стихий -{0}%
        eff.elementalElite|, элита -5% дополнительно
eff.repair|Снижение макс. прочности -{0}%
eff.barter|Цена продажи +{0}%
eff.throwing|Дальность броска +{0}%, урон взрыва +{1}%
eff.looting|Время поиска предметов -{0}%
eff.dash|Перезарядка -{0}%, время действия -{1}%, расход выносливости -{2}%
eff.melee|Скорость атаки в ближнем бою +{0}%
eff.hideout|Перезарядка пополнения торговца -{0}%
eff.hideoutElite|, время добычи -{0}%
eff.crafting|Шанс доп. продукции {0}%
eff.healSpeed|Скорость лечения +{0}%
eff.value|{0} {1}
# ==== Названия параметров ====
stat.MaxWeight|Макс. вес
stat.InventoryCapacity|Ячейки инвентаря
stat.StaminaDrainRate|Расход выносливости
stat.StaminaRecoverRate|Восстановление выносливости
stat.MaxHealth|Макс. здоровье
stat.HealGain|Эффективность лечения
stat.GunDamageMultiplier|Урон оружия
stat.GunCritRateGain|Крит. шанс оружия
stat.RecoilControl|Контроль отдачи
stat.RecoilScaleV|Вертикальная отдача
stat.RecoilScaleH|Горизонтальная отдача
stat.GunDistanceMultiplier|Дальность оружия
stat.GunScatterMultiplier|Разброс пуль
stat.GunCritDamageGain|Урон в голову
stat.MeleeDamageMultiplier|Урон в ближнем бою
stat.MeleeCritRateGain|Крит. шанс ближнего боя
stat.WalkSpeed|Скорость передвижения
stat.RunSpeed|Скорость передвижения
stat.WalkSoundRange|Звук ходьбы
stat.RunSoundRange|Звук бега
stat.SoundRange|Радиус звука
stat.BodyArmor|Броня корпуса
stat.HeadArmor|Броня головы
stat.DurabilityCost|Расход прочности
stat.BuffChance|Шанс дебаффа
stat.ElementFactor_Fire|Получаемый урон огнём
stat.ElementFactor_Poison|Получаемый урон ядом
stat.ElementFactor_Physics|Получаемый физический урон
        stat.ElementFactor_Electricity|Получаемый электрический урон
        stat.ElementFactor_Ice|Получаемый урон льдом
        stat.ElementFactor_Ghost|Получаемый урон призрака
        stat.ElementFactor_Space|Получаемый космический урон
stat.BleedChance|Шанс кровотечения
stat.ReloadSpeedGain|Скорость перезарядки
stat.EnergyCost|Расход сытости
stat.WaterCost|Расход воды
stat.NightVisionAbility|Ночное зрение
stat.ViewDistance|Дальность обзора
stat.SenseRange|Радиус обнаружения
stat.HearingAbility|Слух
# ==== 24 навыка ====
skill.strength.name|Сила
        skill.strength.trigger|Движение при весе более 60% (свыше 80% — быстрее)
skill.strength.elite|Инвентарь +5 ячеек, скорость передвижения +10%
skill.endurance.name|Выносливость
skill.endurance.trigger|Во время бега
skill.endurance.elite|Восстановление выносливости +30%
skill.vitality.name|Живучесть
skill.vitality.trigger|При получении урона (10 очков за единицу урона)
skill.vitality.elite|Макс. здоровье +5
skill.health.name|Лечение
skill.health.trigger|При восстановлении здоровья аптечкой (10 очков за единицу)
skill.health.elite|Скорость лечения +10% (дополнительно)
skill.metabolism.name|Метаболизм
skill.metabolism.trigger|При восстановлении сытости/воды (10 очков за единицу)
skill.metabolism.elite|Нет урона при нулевой сытости и воде
skill.dash.name|Кувырок
skill.dash.trigger|При использовании кувырка (4 очка за раз)
skill.dash.elite|Перезарядка, время действия и расход выносливости ещё -10%
skill.nightvision.name|Ночное зрение
skill.nightvision.trigger|При активности ночью (2 очка/с)
skill.nightvision.elite|Ночное зрение +0.2 (полностью снимает ночной штраф)
skill.assault.name|Стрельба
skill.assault.trigger|Только при попадании
skill.assault.elite|Крит. шанс оружия +10%
skill.recoil.name|Контроль отдачи
skill.recoil.trigger|При выстреле (2 очка за выстрел, 4 очка при прицеливании)
skill.recoil.elite|Вертикальная и горизонтальная отдача -15%
skill.marksmanship.name|Точная стрельба
skill.marksmanship.trigger|Попадание с 20 м и далее (150 очков, +200 за выстрел в голову)
skill.marksmanship.elite|Урон в голову +10%
skill.melee.name|Ближний бой
skill.melee.trigger|При попадании в ближнем бою (30 очков, +60 за убийство)
skill.melee.elite|Скорость передвижения +10% (дополнительно)
skill.throwing.name|Метательное
skill.throwing.trigger|При броске взрывчатки (100 очков, 2 очка за единицу урона взрыва)
skill.throwing.elite|Дальность +20% (дополнительно), 30% шанс мгновенного взрыва
skill.perception.name|Восприятие
skill.perception.trigger|При обнаружении звука рядом (4 очка, не чаще 1 раза в секунду)
skill.perception.elite|Обзор и обнаружение +10%, слух +20% (дополнительно)
skill.covert.name|Скрытное движение
skill.covert.trigger|При ходьбе (кроме бега, 1 очко/с)
skill.covert.elite|Звук ходьбы и бега ещё -25% (итого -45% / -35%)
skill.armor.name|Защита
skill.armor.trigger|При получении урона в броне (12 очков за единицу урона)
skill.armor.elite|Получаемый физический урон -10%
        skill.elemental.name|Адаптация к стихиям
        skill.elemental.trigger|При получении урона от стихий (огонь/яд/электричество/лёд/призрак/космос) (20 очков за урон)
        skill.elemental.elite|Урон от стихий -5% дополнительно (итого -15% для каждой стихии)
skill.survival.name|Выживание
skill.survival.trigger|При получении статуса (кровотечение/яд/огонь) (300 очков, 12 очков за тик)
skill.survival.elite|Иммунитет к кровотечению (30% сопротивление — базовый эффект)
skill.reload.name|Перезарядка
skill.reload.trigger|При завершении перезарядки (40 очков)
skill.reload.elite|Скорость перезарядки +10% (дополнительно)
skill.repair.name|Ремонт
        skill.repair.trigger|При ремонте снаряжения (5 очков за единицу прочности)
skill.repair.elite|Макс. прочность больше не снижается
skill.barter.name|Торговля
skill.barter.trigger|При покупке или продаже (5 очков за 1 000 оборота)
skill.barter.elite|Перезарядка чёрного рынка -50%
skill.looting.name|Обыск
skill.looting.trigger|При поиске в ящиках и телах (20 очков за находку, 10 очков за подбор)
        skill.looting.elite|При открытии: 50% шанс мгновенно обнаружить все предметы
skill.hideout.name|Убежище
skill.hideout.trigger|Пребывание на базе (0.5 очка/с) + 500 очков за постройку или улучшение
skill.hideout.elite|Время добычи биткоин-майнера -20%
skill.fishing.name|Рыбалка
skill.fishing.trigger|При поимке рыбы (250 очков за рыбу)
skill.fishing.elite|Навык рыбалки и удача по +20% (дополнительно)
skill.crafting.name|Изготовление
skill.crafting.trigger|Изготовление (по стоимости материалов) + каждый новый рецепт
skill.crafting.elite|Шанс доп. продукции +20% (итого 50%)

";
    }
}
