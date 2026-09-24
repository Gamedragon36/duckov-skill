namespace Dskill
{
    /// <summary>简体中文 (Simplified Chinese)</summary>
    internal static class LocZhHans
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ 塔科夫技能系统   —   可用鼠标拖动标题栏移动窗口
ui.series|类别：{0}
ui.total|总计 Lv.{0} / {1}
ui.growth|成长：{0}
ui.tabs|[F7] 切换类别    [{0} / ESC] 关闭
ui.footer|保存游戏时经验值会一并存入存档栏位。
cat.body|体质
cat.combat|战斗
cat.utility|实用
# ==== 继承（Roguelike） ====
grade.6|传说
grade.5|英雄
grade.4|精通
grade.3|熟练
grade.2|初心
grade.1|入门
grade.0|无
meta.off|继承：已关闭
meta.none|继承：暂无其他存档记录
meta.line|继承 {0}（其他存档 {1} 个 / Lv.{2}）→ 经验 +{3}%
meta.notice|[继承] 其他存档的记录使经验 +{0}%（{1}）
# ==== 通知 ====
notify.levelup|[技能] {0} Lv.{1}
notify.elite|  ★达成精英！
# ==== 效果文本 ====
eff.survival|异常状态抗性 {0}%，受到火焰/中毒伤害 -{1}%
eff.survivalElite|，免疫流血
eff.repair|最大耐久度损耗 -{0}%
eff.barter|售价 +{0}%
eff.throwing|投掷距离 +{0}%，爆炸伤害 +{1}%
eff.looting|物品搜索时间 -{0}%
eff.dash|冷却 -{0}%，耐力消耗 -{1}%
eff.hideout|商人补货冷却 -{0}%
eff.hideoutElite|，挖矿时间 -{0}%
eff.crafting|额外产出概率 {0}%
eff.value|{0} {1}
# ==== 属性名称 ====
stat.MaxWeight|最大负重
stat.InventoryCapacity|背包格数
stat.StaminaDrainRate|耐力消耗
stat.StaminaRecoverRate|耐力恢复
stat.MaxHealth|最大生命值
stat.HealGain|治疗效果
stat.GunDamageMultiplier|枪械伤害
stat.GunCritRateGain|枪械暴击率
stat.RecoilControl|后坐力控制
stat.RecoilScaleV|垂直后坐力
stat.RecoilScaleH|水平后坐力
stat.GunDistanceMultiplier|枪械射程
stat.GunScatterMultiplier|子弹散布
stat.GunCritDamageGain|爆头伤害
stat.MeleeDamageMultiplier|近战伤害
stat.MeleeCritRateGain|近战暴击率
stat.WalkSpeed|移动速度
stat.RunSpeed|移动速度
stat.WalkSoundRange|走路声音
stat.RunSoundRange|奔跑声音
stat.SoundRange|声音范围
stat.BodyArmor|身体护甲
stat.HeadArmor|头部护甲
stat.DurabilityCost|耐久度消耗
stat.BuffChance|负面状态概率
stat.ElementFactor_Fire|受到火焰伤害
stat.ElementFactor_Poison|受到中毒伤害
stat.ElementFactor_Physics|受到物理伤害
stat.BleedChance|流血概率
stat.ReloadSpeedGain|装弹速度
stat.EnergyCost|饥饿消耗
stat.WaterCost|水分消耗
stat.NightVisionAbility|夜视能力
stat.ViewDistance|视野距离
stat.SenseRange|感知范围
stat.HearingAbility|听力
# ==== 技能 23 种 ====
skill.strength.name|力量
skill.strength.trigger|负重 70% 以上时移动（每秒 1 点，90% 以上为 2 点）
skill.strength.elite|背包空间 +5 格，移动速度 +10%
skill.endurance.name|耐力
skill.endurance.trigger|奔跑时（每秒 1 点）
skill.endurance.elite|耐力恢复速度 +30%
skill.vitality.name|生命
skill.vitality.trigger|受到伤害时（每点伤害 10 点）
skill.vitality.elite|最大生命值 +5
skill.health.name|治疗
skill.health.trigger|用治疗物品恢复生命时（每点恢复量 20 点）
skill.health.elite|治疗效率 +10%
skill.metabolism.name|新陈代谢
skill.metabolism.trigger|用食物或水补充饥饿/水分时（每点 20 点）
skill.metabolism.elite|饥饿、水分为 0 时也不会受到伤害
skill.dash.name|翻滚
skill.dash.trigger|使用翻滚时（每次 8 点）
skill.dash.elite|冷却与耐力消耗各再 -20%
skill.nightvision.name|夜视
skill.nightvision.trigger|夜晚活动时（每秒 2 点）
skill.nightvision.elite|夜视能力 +0.2（完全抵消夜间惩罚）
skill.assault.name|射击术
skill.assault.trigger|开枪时（每发 4 点，命中再 +4 点）
skill.assault.elite|枪械暴击率 +10%
skill.recoil.name|后坐力控制
skill.recoil.trigger|开枪时（每发 3 点，瞄准射击 6 点）
skill.recoil.elite|垂直、水平后坐力 -15%
skill.marksmanship.name|精准射击
skill.marksmanship.trigger|在 20 米以上距离命中（每次 45 点，爆头 +70 点）
skill.marksmanship.elite|爆头伤害 +10%
skill.melee.name|近战
skill.melee.trigger|近战攻击命中时（每次 30 点，近战击杀 +60 点）
skill.melee.elite|移动速度 +10%（额外）
skill.throwing.name|投掷
skill.throwing.trigger|投掷爆炸物时（每次 200 点，每点爆炸伤害 2 点）
skill.throwing.elite|距离 +20%（额外），30% 概率立即爆炸
skill.perception.name|感知
skill.perception.trigger|察觉到周围声响时（每次 4 点，每秒最多 1 次）
skill.perception.elite|视野与感知 +10%，听力 +20%（额外）
skill.covert.name|潜行移动
skill.covert.trigger|走路移动时（不含奔跑，每秒 1 点）
skill.covert.elite|走路与奔跑声音再 -25%（合计 -45% / -35%）
skill.armor.name|防御
skill.armor.trigger|穿着护甲受到伤害时（每点伤害 12 点）
skill.armor.elite|受到的物理伤害 -10%
skill.survival.name|生存术
skill.survival.trigger|陷入异常状态（流血、中毒、燃烧）时（每次 300 点，每点持续伤害 12 点）
skill.survival.elite|不会流血（30% 负面状态抗性为默认效果）
skill.reload.name|装弹
skill.reload.trigger|完成装弹时（每次 40 点）
skill.reload.elite|装弹速度 +10%（额外）
skill.repair.name|维修
skill.repair.trigger|维修装备时（每点修复耐久度 30 点）
skill.repair.elite|最大耐久度不再下降
skill.barter.name|讨价还价
skill.barter.trigger|买卖物品时（每 1,000 交易额 10 点）
skill.barter.elite|黑市刷新冷却 -50%
skill.looting.name|搜刮
skill.looting.trigger|在箱子或尸体中发现物品时（每件 40 点，拾取 1 件 15 点）
skill.looting.elite|50% 概率立即发现
skill.hideout.name|藏身处
skill.hideout.trigger|待在基地（每秒 1 点）+ 新建或升级建筑 500 点
skill.hideout.elite|比特币矿机产出时间 -20%
skill.fishing.name|钓鱼
skill.fishing.trigger|钓到鱼时（每条 250 点）
skill.fishing.elite|钓鱼能力与运气各 +20%（额外）
skill.crafting.name|制作
skill.crafting.trigger|完成制作 1 次 200 点 + 解锁新配方 1 次 400 点
skill.crafting.elite|额外产出概率 +20%（合计 50%）
# ==== config.ini 注释 ====
cfg.header|塔科夫式技能系统 - 设置
cfg.headerNote1|修改数值后重新启动游戏即可生效。
cfg.headerNote2|# 后面的内容只是说明，可以删除。
cfg.maxLevel|技能最高等级（满级）
cfg.xpBase|升到 1 级所需经验
cfg.xpStep|每级额外需要的经验
cfg.xpMultiplier|全局经验倍率（0.5=一半，2.0=两倍）
cfg.hotkey|打开技能窗口的按键（例如 F6、F7、F8）
cfg.notifyLevelUp|升级时显示通知（true/false）
cfg.metaNote1|继承（Roguelike）：其他存档中培养的技能等级总和
cfg.metaNote2|会提升当前存档的经验获取速度。
cfg.metaEnabled|启用该功能（true/false）
cfg.metaPerLevel|其他存档每 1 技能等级，经验 +0.5%
cfg.metaCap|最大加成（%）
cfg.resetNote1|危险：设为 true 并启动游戏一次，所有技能经验将被清零。
cfg.resetNote2|清零完成后该值会自动变回 false。
cfg.resetSkills|true = 下次启动时清零所有技能经验
cfg.skillXpNote|各技能经验倍率（0.5=一半速度，2.0=两倍速度）
cfg.language|界面语言：auto、en、ko、zh、zh-hant、ja、de、ru、es、fr、pt-br

";
    }
}
