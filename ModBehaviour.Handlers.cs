using System;
using System.Collections.Generic;
using Duckov.Buffs;
using Duckov.UI;
using ItemStatsSystem;
using Saves;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dskill
{
    /// <summary>
    /// 스킬 경험치를 주는 실제 게임 이벤트 처리 + 화면 표시.
    /// (뼈대와 주기 처리는 ModBehaviour.cs 에 있습니다)
    /// </summary>
    public partial class ModBehaviour
    {
        // ------------------------------------------------------------------
        // 피해 / 사망
        // ------------------------------------------------------------------

        /// <summary>이 피해에 '속성'이 실려 있는지 확인한다.
        ///  게임은 속성이 없으면 물리(physics) 1.0 을 넣으므로(Health.cs),
        ///  물리가 아닌 속성이 하나라도 있으면 속성 피해로 봅니다.</summary>
        private static bool HasElementDamage(DamageInfo info)
        {
            List<ElementFactor> factors = info.elementFactors;
            if (factors == null)
            {
                return false;
            }
            for (int i = 0; i < factors.Count; i++)
            {
                ElementFactor factor = factors[i];
                if (factor.elementType != ElementTypes.physics && factor.factor > 0f)
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleHurt(Health health, DamageInfo info)
        {
            if (health == null || _skills == null || _main == null)
            {
                return;
            }
            // 내가 맞은 것이 아니면 무시
            if (health.TryGetCharacter() != _main)
            {
                return;
            }

            float damage = info.finalDamage;
            if (damage <= 0f)
            {
                return;
            }

            // 생명력: 맞은 만큼
            _skills.AddXp("vitality", damage * Rates.VitalityPerDamage);

            // 방어: 방어구를 착용 중일 때
            if (health.BodyArmor > 0f || health.HeadArmor > 0f)
            {
                _skills.AddXp("armor", damage * Rates.ArmorPerDamage);
            }

            // 상태이상(출혈·중독 등)으로 인한 지속 피해
            if (info.isFromBuffOrEffect)
            {
                _skills.AddXp("survival", damage * Rates.SurvivalPerTickDamage);
            }

            // 속성적응: 속성(화염·독·전기·얼음·유령·우주) 피해를 받았을 때 (물리는 '방어' 담당)
            if (HasElementDamage(info))
            {
                _skills.AddXp("elemental", damage * Rates.ElementPerDamage);
            }

            // 폭발 피해 (투척술): 내가 던진 폭발물로 준 피해
            if (info.isExplosion && info.fromCharacter != null && info.fromCharacter == _main)
            {
                _skills.AddXp("throwing", damage * Rates.ThrowingPerExplosionDamage);
            }

            // 내가 준 피해 (사격 / 정밀 사격 / 근접)
            CharacterMainControl from = info.fromCharacter;
            if (from != null && from == _main && IsMeleeDamage(info))
            {
                // 근접 전투: 명중할 때마다 (이전 버전에서 누락되어 있던 경험치)
                _skills.AddXp("melee", Rates.MeleePerHit);
            }
            else if (from != null && from == _main)
            {
                if (_gun != null && _gun.Item != null && info.fromWeaponItemID == _gun.Item.TypeID)
                {
                    _skills.AddXp("assault", Rates.AssaultPerHit);

                    float distance = Vector3.Distance(_main.transform.position, info.damagePoint);
                    if (distance >= Rates.MarksmanshipMinDistance)
                    {
                        float gain = Rates.MarksmanshipPerHit;
                        if (info.crit != 0)
                        {
                            gain += Rates.MarksmanshipPerCrit;
                        }
                        _skills.AddXp("marksmanship", gain);
                    }
                }
            }
        }

        /// <summary>이 피해가 근접 무기로 준 것인지 확인</summary>
        private bool IsMeleeDamage(DamageInfo info)
        {
            if (_melee == null || _melee.Item == null)
            {
                return false;
            }
            return info.fromWeaponItemID == _melee.Item.TypeID;
        }

        private void HandleDead(Health health, DamageInfo info)
        {
            if (_skills == null || info.fromCharacter == null || info.fromCharacter != _main)
            {
                return;
            }
            if (IsMeleeDamage(info))
            {
                _skills.AddXp("melee", Rates.MeleePerKill);
            }
        }

        // ------------------------------------------------------------------
        // 사격 / 재장전
        // ------------------------------------------------------------------

        private void HandleShoot()
        {
            if (_skills == null)
            {
                return;
            }
            // 0.0.6: 사격술은 '총알이 적중했을 때만' 오른다(발사 자체로는 오르지 않음).
            //        발사로는 반동 제어만 오른다.
            bool ads = _gun != null && _gun.IsInAds;
            _skills.AddXp("recoil", ads ? Rates.RecoilPerAdsShot : Rates.RecoilPerShot);
        }

        private void HandleLoaded()
        {
            if (_skills == null)
            {
                return;
            }
            _skills.AddXp("reload", Rates.ReloadPerComplete);
        }

        // ---- 회복 (치료 속도 = 아이템 사용 시간 감소) ----
        private readonly HashSet<int> _healingTypeIds = new HashSet<int>();                      // 실제로 체력을 채운 아이템 종류(TypeID)
        private readonly Dictionary<int, float> _originalUseTime = new Dictionary<int, float>(); // TypeID -> 원래 사용 시간
        private Item _lastUsedItem;
        private static readonly System.Reflection.FieldInfo _useTimeField =
            typeof(UsageUtilities).GetField("useTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        /// <summary>회복 아이템의 '사용 시간'을 줄인다(치료 속도).
        ///  게임에는 사용 속도 스탯이 없어서 아이템의 비공개 필드(UsageUtilities.useTime)를 바꾼다.
        ///  아이템 인스턴스에만 적용되고(프리팹 원본 아님), 실패하면 조용히 넘어간다.</summary>
        private void ApplyHealSpeed(Item item)
        {
            if (item == null || _skills == null || _useTimeField == null)
            {
                return;
            }
            if (!_healingTypeIds.Contains(item.TypeID))
            {
                return;   // 아직 '회복 아이템'으로 확인되지 않은 종류는 건드리지 않는다
            }

            float reduction = _skills.HealSpeedBonus(_skills.GetLevel("health"));
            if (reduction <= 0f)
            {
                return;
            }

            try
            {
                UsageUtilities usage = item.UsageUtilities;
                if (usage == null)
                {
                    return;
                }
                float original;
                if (!_originalUseTime.TryGetValue(item.TypeID, out original))
                {
                    original = usage.UseTime;
                    _originalUseTime[item.TypeID] = original;
                }
                float target = Mathf.Max(0.05f, original * (1f - reduction));
                if (Mathf.Abs(usage.UseTime - target) > 0.001f)
                {
                    _useTimeField.SetValue(usage, target);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 치료 속도 적용 실패(무시): " + e.Message);
            }
        }

        private void HandleStartUseItem(Item item)
        {
            // 아이템을 사용한 직후 몇 초 동안의 체력 증가를 '회복' 경험치로 인정한다.
            _medicalUseUntil = Time.time + 6f;
            _lastUsedItem = item;
            ApplyHealSpeed(item);   // 회복 아이템으로 확인된 종류면 사용 시간을 줄인다(치료 속도)
            // 음식/물을 사용한 직후의 포만감·수분 증가만 '신진대사' 경험치로 인정한다.
            // (기지에서 잠만 자도 오르는 것을 막기 위함)
            _foodUseUntil = Time.time + 15f;

            // 폭발물을 사용하면 잠시 동안 수류탄 확인을 촘촘히 한다(최적화)
            if (item != null && item.Tags != null && item.Tags.Contains("Explosive"))
            {
                _grenadeWatchUntil = Time.time + 6f;
            }
        }

        // ------------------------------------------------------------------
        // 상태이상 (생존술 / 신진대사 엘리트)
        // ------------------------------------------------------------------

        private void HandleAddBuff(CharacterBuffManager manager, Buff buff)
        {
            if (_skills == null || buff == null || manager == null)
            {
                return;
            }

            try
            {
                Buff.BuffExclusiveTags tag = buff.ExclusiveTag;

                // 생존술: 확률로 디버프를 무효화 (디버프 확률 -30% 효과를 코드로 구현)
                if (IsDebuffTag(tag))
                {
                    float resist = _skills.SurvivalDebuffResist(_skills.GetLevel("survival"));
                    if (resist > 0f && UnityEngine.Random.value < resist)
                    {
                        manager.RemoveBuff(buff, false);
                        return;
                    }
                }

                switch (tag)
                {
                    case Buff.BuffExclusiveTags.Bleeding:
                        // 생존술 엘리트: 출혈에 걸리지 않는다
                        if (_skills.IsElite("survival"))
                        {
                            manager.RemoveBuff(buff, false);
                            return;
                        }
                        _skills.AddXp("survival", Rates.SurvivalPerDebuff);
                        break;

                    case Buff.BuffExclusiveTags.Poison:
                    case Buff.BuffExclusiveTags.Burning:
                    case Buff.BuffExclusiveTags.Electric:
                    case Buff.BuffExclusiveTags.Nauseous:
                    case Buff.BuffExclusiveTags.Space:
                        _skills.AddXp("survival", Rates.SurvivalPerDebuff);
                        break;

                    case Buff.BuffExclusiveTags.Starve:
                    case Buff.BuffExclusiveTags.Thirsty:
                        // 신진대사 엘리트: 배고픔/수분이 0이어도 피해를 받지 않는다
                        if (_skills.IsElite("metabolism"))
                        {
                            manager.RemoveBuff(buff, false);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 상태이상 처리 오류: " + e.Message);
            }
        }

        /// <summary>플레이어에게 해로운 상태이상인지 확인</summary>
        private static bool IsDebuffTag(Buff.BuffExclusiveTags tag)
        {
            switch (tag)
            {
                case Buff.BuffExclusiveTags.Bleeding:
                case Buff.BuffExclusiveTags.Poison:
                case Buff.BuffExclusiveTags.Burning:
                case Buff.BuffExclusiveTags.Electric:
                case Buff.BuffExclusiveTags.Nauseous:
                case Buff.BuffExclusiveTags.Space:
                case Buff.BuffExclusiveTags.Stun:
                case Buff.BuffExclusiveTags.Pain:
                    return true;
                default:
                    return false;
            }
        }

        // ------------------------------------------------------------------
        // 거래 (흥정)
        // ------------------------------------------------------------------

        private void HandleItemSold(Duckov.Economy.StockShop shop, Item item, int sellPrice)
        {
            if (_skills == null)
            {
                return;
            }

            _skills.AddXp("barter", sellPrice / 1000f * Rates.BarterPer1000);

            // 흥정 레벨만큼 판매 금액을 더 준다 (최대 +10%)
            float bonus = _skills.BarterBonus(_skills.GetLevel("barter"));
            if (bonus > 0f && sellPrice > 0)
            {
                try
                {
                    Duckov.Economy.EconomyManager.Add((long)Mathf.RoundToInt(sellPrice * bonus));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[Dskill] 판매 보너스 지급 실패: " + e.Message);
                }
            }
        }

        private void HandleItemPurchased(Duckov.Economy.StockShop shop, Item item)
        {
            if (_skills == null)
            {
                return;
            }

            int price = 0;
            try
            {
                if (shop != null && item != null)
                {
                    price = shop.ConvertPrice(item);
                }
            }
            catch (Exception)
            {
                price = 0;
            }
            _skills.AddXp("barter", price / 1000f * Rates.BarterPer1000);
        }

        /// <summary>흥정 엘리트: 암시장 갱신 쿨타임 -50%</summary>
        private void HandleBlackMarketRefresh(Duckov.BlackMarkets.BlackMarket.OnRequestRefreshTimeFactorEventContext context)
        {
            if (_skills == null || context == null)
            {
                return;
            }
            if (_skills.IsElite("barter"))
            {
                context.Add(-0.5f);
            }
        }

        // ------------------------------------------------------------------
        // 레벨업 / 저장
        // ------------------------------------------------------------------

        private void HandleLevelUp(string id, int level)
        {
            SkillDef def = SkillDefs.Find(id);
            if (def == null || _skills == null)
            {
                return;
            }

            // 새 레벨의 보너스를 바로 적용한다
            if (_mainItem != null)
            {
                _skills.ApplyTo(_mainItem);
            }

            if (_config == null || !_config.NotifyLevelUp)
            {
                return;
            }

            try
            {
                string text = Locale.F("notify.levelup", "[스킬] {0} Lv.{1}", Locale.SkillName(def), level);
                if (level >= _skills.MaxLevel)
                {
                    text += Locale.T("notify.elite", " ★엘리트 달성!");
                }
                NotificationText.Push(text);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 알림 표시 실패: " + e.Message);
            }
        }

        private void HandleSetFile()
        {
            if (_skills == null)
            {
                return;
            }
            // 다른 세이브 슬롯을 열면 그 슬롯의 스킬 데이터를 읽고, 계승 보너스도 다시 계산한다
            if (_meta != null)
            {
                _meta.Rescan(true);
            }
            _skills.Load();
            _firstTick = true;
        }

        private void HandleCollectSaveData()
        {
            if (_skills != null)
            {
                _skills.Save();
            }
        }

        // ------------------------------------------------------------------
        // 하이드아웃 (건물 건설)
        // 농작물 수확은 게임에 정원이 실제로 배치되지 않아 제외했습니다(QA 결과).
        // ------------------------------------------------------------------

        private void HandleBuildingBuilt(int id)
        {
            if (_skills != null)
            {
                _skills.AddXp("hideout", Rates.HideoutPerBuilding);
            }
        }

        // ------------------------------------------------------------------
        // 화면 표시 (IMGUI)
        // ------------------------------------------------------------------

        /// <summary>한글이 깨지지 않도록 운영체제 폰트를 불러온다.</summary>
        private void EnsureFont()
        {
            if (_font != null)
            {
                return;
            }
            _font = Font.CreateDynamicFontFromOSFont(
                new[]
                {
                    // 한국어
                    "Malgun Gothic", "맑은 고딕", "Gulim", "돋움",
                    // 중국어(간체 / 번체)
                    "Microsoft YaHei", "微软雅黑", "SimSun", "SimHei", "Microsoft JhengHei", "MingLiU",
                    // 일본어
                    "Yu Gothic UI", "MS Gothic", "Meiryo", "MS PGothic",
                    // 공용 대체 폰트
                    "Arial Unicode MS", "Arial"
                }, 15);
            if (_font == null)
            {
                Debug.LogWarning("[Dskill] 한글 폰트를 찾지 못했습니다.");
            }
        }

        private void OnGUI()
        {
            if (!_panelVisible || _skills == null || _config == null)
            {
                return;
            }

            EnsureFont();

            // 그리는 동안만 폰트를 바꾸고 끝나면 원래대로 되돌린다
            Font previousFont = GUI.skin.font;
            if (_font != null)
            {
                GUI.skin.font = _font;
            }

            try
            {
                SkillWindow.Draw(_skills, _config, _meta);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 스킬 창 오류: " + e.Message);
            }
            finally
            {
                GUI.skin.font = previousFont;
            }
        }
    }
}

