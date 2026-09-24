namespace Dskill
{
    /// <summary>繁體中文 (Traditional Chinese)</summary>
    internal static class LocZhHant
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ 塔科夫技能系統   —   可用滑鼠拖曳標題列移動視窗
ui.series|類別：{0}
ui.total|總計 Lv.{0} / {1}
ui.growth|成長：{0}
ui.xpStage|經驗倍率：{0}/5 檔（x{1}）
ui.xpStageHint|1 = 最快，5 = 預設
ui.tabs|[F7] 切換類別    [{0} / ESC] 關閉
ui.footer|儲存遊戲時經驗值會一併存入存檔欄位。
cat.body|體質
cat.combat|戰鬥
cat.utility|實用
# ==== 繼承（Roguelike） ====
grade.6|傳說
grade.5|英雄
grade.4|精通
grade.3|熟練
grade.2|初心
grade.1|入門
grade.0|無
meta.off|繼承：已關閉
meta.none|繼承：尚無其他存檔記錄
meta.line|繼承 {0}（其他存檔 {1} 個 / Lv.{2}）→ 經驗 +{3}%
meta.notice|[繼承] 其他存檔的記錄使經驗 +{0}%（{1}）
# ==== 通知 ====
notify.levelup|[技能] {0} Lv.{1}
notify.elite|  ★達成菁英！
# ==== 效果文字 ====
eff.survival|異常狀態抗性 {0}%，受到火焰/中毒傷害 -{1}%
eff.survivalElite|，免疫流血
eff.repair|最大耐久度損耗 -{0}%
eff.barter|售價 +{0}%
eff.throwing|投擲距離 +{0}%，爆炸傷害 +{1}%
eff.looting|物品搜索時間 -{0}%
eff.dash|冷卻 -{0}%，動作時間 -{1}%，耐力消耗 -{2}%
eff.hideout|商人補貨冷卻 -{0}%
eff.hideoutElite|，挖礦時間 -{0}%
eff.crafting|額外產出機率 {0}%
eff.healSpeed|治療速度 +{0}%
eff.value|{0} {1}
# ==== 屬性名稱 ====
stat.MaxWeight|最大負重
stat.InventoryCapacity|背包格數
stat.StaminaDrainRate|耐力消耗
stat.StaminaRecoverRate|耐力恢復
stat.MaxHealth|最大生命值
stat.HealGain|治療效果
stat.GunDamageMultiplier|槍械傷害
stat.GunCritRateGain|槍械爆擊率
stat.RecoilControl|後座力控制
stat.RecoilScaleV|垂直後座力
stat.RecoilScaleH|水平後座力
stat.GunDistanceMultiplier|槍械射程
stat.GunScatterMultiplier|子彈散布
stat.GunCritDamageGain|爆頭傷害
stat.MeleeDamageMultiplier|近戰傷害
stat.MeleeCritRateGain|近戰爆擊率
stat.WalkSpeed|移動速度
stat.RunSpeed|移動速度
stat.WalkSoundRange|走路聲音
stat.RunSoundRange|奔跑聲音
stat.SoundRange|聲音範圍
stat.BodyArmor|身體護甲
stat.HeadArmor|頭部護甲
stat.DurabilityCost|耐久度消耗
stat.BuffChance|負面狀態機率
stat.ElementFactor_Fire|受到火焰傷害
stat.ElementFactor_Poison|受到中毒傷害
stat.ElementFactor_Physics|受到物理傷害
stat.BleedChance|流血機率
stat.ReloadSpeedGain|裝彈速度
stat.EnergyCost|飢餓消耗
stat.WaterCost|水分消耗
stat.NightVisionAbility|夜視能力
stat.ViewDistance|視野距離
stat.SenseRange|感知範圍
stat.HearingAbility|聽力
# ==== 技能 23 種 ====
skill.strength.name|力量
skill.strength.trigger|負重 70% 以上時移動（越重越快）
skill.strength.elite|背包空間 +5 格，移動速度 +10%
skill.endurance.name|耐力
skill.endurance.trigger|奔跑時
skill.endurance.elite|耐力恢復速度 +30%
skill.vitality.name|生命
skill.vitality.trigger|受到傷害時（每點傷害 10 點）
skill.vitality.elite|最大生命值 +5
skill.health.name|治療
skill.health.trigger|用治療物品恢復生命時（每點恢復量 10 點）
skill.health.elite|治療速度 +10%（額外）
skill.metabolism.name|新陳代謝
skill.metabolism.trigger|用食物或水補充飢餓/水分時（每點 20 點）
skill.metabolism.elite|飢餓、水分為 0 時也不會受到傷害
skill.dash.name|翻滾
skill.dash.trigger|使用翻滾時（每次 4 點）
skill.dash.elite|冷卻、動作時間與耐力消耗各再 -10%
skill.nightvision.name|夜視
skill.nightvision.trigger|夜晚活動時（每秒 2 點）
skill.nightvision.elite|夜視能力 +0.2（完全抵消夜間懲罰）
skill.assault.name|射擊術
skill.assault.trigger|僅當子彈命中時
skill.assault.elite|槍械爆擊率 +10%
skill.recoil.name|後座力控制
skill.recoil.trigger|開槍時（每發 2 點，瞄準射擊 4 點）
skill.recoil.elite|垂直、水平後座力 -15%
skill.marksmanship.name|精準射擊
skill.marksmanship.trigger|在 8 公尺以上距離命中（每次 45 點，爆頭 +70 點）
skill.marksmanship.elite|爆頭傷害 +10%
skill.melee.name|近戰
skill.melee.trigger|近戰攻擊命中時（每次 30 點，近戰擊殺 +60 點）
skill.melee.elite|移動速度 +10%（額外）
skill.throwing.name|投擲
skill.throwing.trigger|投擲爆炸物時（每次 100 點，每點爆炸傷害 2 點）
skill.throwing.elite|距離 +20%（額外），30% 機率立即爆炸
skill.perception.name|感知
skill.perception.trigger|察覺到周圍聲響時（每次 4 點，每秒最多 1 次）
skill.perception.elite|視野與感知 +10%，聽力 +20%（額外）
skill.covert.name|潛行移動
skill.covert.trigger|走路移動時（不含奔跑，每秒 1 點）
skill.covert.elite|走路與奔跑聲音再 -25%（合計 -45% / -35%）
skill.armor.name|防禦
skill.armor.trigger|穿著護甲受到傷害時（每點傷害 12 點）
skill.armor.elite|受到的物理傷害 -10%
skill.survival.name|生存術
skill.survival.trigger|陷入異常狀態（流血、中毒、燃燒）時（每次 300 點，每點持續傷害 12 點）
skill.survival.elite|不會流血（30% 負面狀態抗性為預設效果）
skill.reload.name|裝彈
skill.reload.trigger|完成裝彈時（每次 40 點）
skill.reload.elite|裝彈速度 +10%（額外）
skill.repair.name|維修
skill.repair.trigger|維修裝備時（每點修復耐久度 30 點）
skill.repair.elite|最大耐久度不再下降
skill.barter.name|討價還價
skill.barter.trigger|買賣物品時（每 1,000 交易額 5 點）
skill.barter.elite|黑市刷新冷卻 -50%
skill.looting.name|搜刮
skill.looting.trigger|在箱子或屍體中發現物品時（每件 20 點，拾取 1 件 10 點）
skill.looting.elite|50% 機率立即發現
skill.hideout.name|藏身處
skill.hideout.trigger|待在基地（每秒 0.5 點）+ 新建或升級建築 500 點
skill.hideout.elite|比特幣礦機產出時間 -20%
skill.fishing.name|釣魚
skill.fishing.trigger|釣到魚時（每條 250 點）
skill.fishing.elite|釣魚能力與運氣各 +20%（額外）
skill.crafting.name|製作
skill.crafting.trigger|完成製作（與材料價值成正比）+ 解鎖新配方
skill.crafting.elite|額外產出機率 +20%（合計 50%）
# ==== config.ini 註解 ====
cfg.header|塔科夫式技能系統 - 設定
cfg.headerNote1|修改數值後重新啟動遊戲即可生效。
cfg.headerNote2|# 後面的內容只是說明，可以刪除。
cfg.maxLevel|技能最高等級（滿級）
cfg.xpBase|升到 1 級所需經驗
cfg.xpStep|每級額外需要的經驗
cfg.xpMultiplier|全域經驗倍率（0.5=一半，2.0=兩倍）
cfg.hotkey|開啟技能視窗的按鍵（例如 F6、F7、F8）
cfg.notifyLevelUp|升級時顯示通知（true/false）
cfg.metaNote1|繼承（Roguelike）：其他存檔中培養的技能等級總和
cfg.metaNote2|會提升當前存檔的經驗取得速度。
cfg.metaEnabled|啟用該功能（true/false）
cfg.metaPerLevel|其他存檔每 1 技能等級，經驗 +0.5%
cfg.metaCap|最大加成（%）
cfg.resetNote1|危險：設為 true 並啟動遊戲一次，所有技能經驗將被清零。
cfg.resetNote2|清零完成後該值會自動變回 false。
cfg.resetSkills|true = 下次啟動時清零所有技能經驗
cfg.skillXpNote|各技能經驗倍率（0.5=一半速度，2.0=兩倍速度）
cfg.language|介面語言：auto（跟隨遊戲內選擇的語言），或 en、ko、zh、zh-hant、ja、de、ru、es、fr、pt-br
cfg.xpStage|經驗倍率檔位 1~5（1=最快，5=預設）
cfg.uploadNow|作者專用：設為 true 並啟動遊戲可上傳一次到工作坊
cfg.craftUnlockXp|解鎖新配方的經驗（批次解鎖只算一次）
cfg.craftXpPerValue|製作經驗 = 材料價值 × 此值（0.08 = 8%）
cfg.craftXpCap|單次製作經驗上限（0 = 不限制）
cfg.hideoutSkills|可以在藏身處獲得經驗的技能（用逗號分隔）— 其他技能不會在藏身處升級

";
    }
}
