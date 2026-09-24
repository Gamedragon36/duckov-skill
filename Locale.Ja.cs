namespace Dskill
{
    /// <summary>日本語 (Japanese)</summary>
    internal static class LocJa
    {
        public const string Table = @"
# ==== UI ====
ui.title|★ タルコフ式スキルシステム   —   タイトルバーをドラッグして移動
ui.series|系統：{0}
ui.total|合計 Lv.{0} / {1}
ui.growth|成長条件：{0}
ui.xpStage|経験値レート：{0}/5（x{1}）
ui.xpStageHint|1 = 最速、5 = 既定
ui.tabs|[F7] 系統切替    [{0} / ESC] 閉じる
ui.footer|経験値はセーブ時にセーブスロットへ保存されます。
cat.body|身体
cat.combat|戦闘
cat.utility|実用
# ==== 継承（ローグライク） ====
grade.6|伝説
grade.5|英雄
grade.4|熟練
grade.3|練達
grade.2|初心
grade.1|入門
grade.0|なし
meta.off|継承：使用しない
meta.none|継承：他のセーブの記録なし
meta.line|継承 {0}（他のセーブ {1} 個 / Lv.{2}）→ 経験値 +{3}%
meta.notice|[継承] 他のセーブの記録により経験値 +{0}%（{1}）
# ==== 通知 ====
notify.levelup|[スキル] {0} Lv.{1}
notify.elite|  ★エリート達成！
# ==== 効果テキスト ====
eff.survival|状態異常耐性 {0}%、受ける火炎/毒ダメージ -{1}%
eff.survivalElite|、出血免疫
eff.repair|最大耐久度の減少 -{0}%
eff.barter|売却額 +{0}%
eff.throwing|投擲距離 +{0}%、爆発ダメージ +{1}%
eff.looting|アイテム探知時間 -{0}%
eff.dash|クールタイム -{0}%、動作時間 -{1}%、スタミナ消費 -{2}%
eff.hideout|商人の補充クールタイム -{0}%
eff.hideoutElite|、採掘時間 -{0}%
eff.crafting|追加生産確率 {0}%
eff.healSpeed|治療速度 +{0}%
eff.value|{0} {1}
# ==== ステータス名 ====
stat.MaxWeight|最大重量
stat.InventoryCapacity|バッグ枠
stat.StaminaDrainRate|スタミナ消費
stat.StaminaRecoverRate|スタミナ回復
stat.MaxHealth|最大体力
stat.HealGain|治療効率
stat.GunDamageMultiplier|銃ダメージ
stat.GunCritRateGain|銃クリティカル率
stat.RecoilControl|反動制御
stat.RecoilScaleV|縦反動
stat.RecoilScaleH|横反動
stat.GunDistanceMultiplier|銃の射程
stat.GunScatterMultiplier|弾の拡散
stat.GunCritDamageGain|ヘッドショットダメージ
stat.MeleeDamageMultiplier|近接ダメージ
stat.MeleeCritRateGain|近接クリティカル率
stat.WalkSpeed|移動速度
stat.RunSpeed|移動速度
stat.WalkSoundRange|歩きの音
stat.RunSoundRange|走りの音
stat.SoundRange|音の範囲
stat.BodyArmor|身体アーマー
stat.HeadArmor|頭アーマー
stat.DurabilityCost|耐久度消費
stat.BuffChance|デバフ確率
stat.ElementFactor_Fire|受ける火炎ダメージ
stat.ElementFactor_Poison|受ける毒ダメージ
stat.ElementFactor_Physics|受ける物理ダメージ
stat.BleedChance|出血確率
stat.ReloadSpeedGain|リロード速度
stat.EnergyCost|空腹消費
stat.WaterCost|水分消費
stat.NightVisionAbility|暗視能力
stat.ViewDistance|視界距離
stat.SenseRange|感知範囲
stat.HearingAbility|聴力
# ==== スキル 23 種 ====
skill.strength.name|筋力
skill.strength.trigger|重量 70% 以上で移動（重いほど速く）
skill.strength.elite|バッグ容量 +5 枠、移動速度 +10%
skill.endurance.name|持久力
skill.endurance.trigger|走っている間
skill.endurance.elite|スタミナ回復速度 +30%
skill.vitality.name|生命力
skill.vitality.trigger|ダメージを受けたとき（ダメージ 1 につき 10 点）
skill.vitality.elite|最大体力 +5
skill.health.name|回復
skill.health.trigger|回復アイテムで体力を回復したとき（回復量 1 につき 10 点）
skill.health.elite|治療速度 +10%（追加）
skill.metabolism.name|代謝
skill.metabolism.trigger|食べ物・水で空腹/水分を回復したとき（1 につき 20 点）
skill.metabolism.elite|空腹・水分が 0 でもダメージを受けない
skill.dash.name|ローリング
skill.dash.trigger|ローリングを使ったとき（1 回 4 点）
skill.dash.elite|クールタイム・動作時間・スタミナ消費がさらに -10%
skill.nightvision.name|暗視
skill.nightvision.trigger|夜間に活動したとき（毎秒 2 点）
skill.nightvision.elite|暗視能力 +0.2（夜間ペナルティを完全に無効化）
skill.assault.name|射撃術
skill.assault.trigger|弾が命中したときのみ
skill.assault.elite|銃クリティカル率 +10%
skill.recoil.name|反動制御
skill.recoil.trigger|銃を発砲したとき（1 発 2 点、照準射撃は 4 点）
skill.recoil.elite|縦・横反動 -15%
skill.marksmanship.name|精密射撃
skill.marksmanship.trigger|8m 以上の距離で命中（1 回 45 点、ヘッドショットは +70 点）
skill.marksmanship.elite|ヘッドショットダメージ +10%
skill.melee.name|近接戦闘
skill.melee.trigger|近接攻撃が命中したとき（1 回 30 点、近接撃破 +60 点）
skill.melee.elite|移動速度 +10%（追加）
skill.throwing.name|投擲術
skill.throwing.trigger|爆発物を投げたとき（1 回 100 点、爆発ダメージ 1 につき 2 点）
skill.throwing.elite|距離 +20%（追加）、30% の確率で即時爆発
skill.perception.name|認知・偵察
skill.perception.trigger|周囲の音を感知したとき（1 回 4 点、1 秒に最大 1 回）
skill.perception.elite|視界・感知 +10%、聴力 +20%（追加）
skill.covert.name|隠密移動
skill.covert.trigger|歩いて移動したとき（走りは除く、毎秒 1 点）
skill.covert.elite|歩き・走りの音がさらに -25%（合計 -45% / -35%）
skill.armor.name|防御
skill.armor.trigger|アーマーを着けてダメージを受けたとき（ダメージ 1 につき 12 点）
skill.armor.elite|受ける物理ダメージ -10%
skill.survival.name|サバイバル
skill.survival.trigger|状態異常（出血・中毒・燃焼）になったとき（1 回 300 点、継続ダメージ 1 につき 12 点）
skill.survival.elite|出血しない（30% のデバフ耐性は基本効果）
skill.reload.name|リロード
skill.reload.trigger|リロードを完了したとき（1 回 40 点）
skill.reload.elite|リロード速度 +10%（追加）
skill.repair.name|修理
skill.repair.trigger|装備を修理したとき（修理した耐久度 1 につき 30 点）
skill.repair.elite|最大耐久度が減らなくなる
skill.barter.name|値切り
skill.barter.trigger|アイテムを売買したとき（取引額 1,000 につき 5 点）
skill.barter.elite|闇市場の補充クールタイム -50%
skill.looting.name|漁り
skill.looting.trigger|箱や死体からアイテムを見つけたとき（1 個 20 点、拾得 1 個 10 点）
skill.looting.elite|50% の確率で即座に探知
skill.hideout.name|ハイドアウト
skill.hideout.trigger|拠点に滞在（毎秒 0.5 点）＋ 建物の新築・アップグレード 500 点
skill.hideout.elite|ビットコインマイナーの生産時間 -20%
skill.fishing.name|釣り
skill.fishing.trigger|魚を釣り上げたとき（1 匹 250 点）
skill.fishing.elite|釣り能力・運がそれぞれ +20%（追加）
skill.crafting.name|製作
skill.crafting.trigger|製作完了（材料の価値に比例）＋ 新レシピ解放
skill.crafting.elite|追加生産確率 +20%（合計 50%）

";
    }
}
