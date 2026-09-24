using System;
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

            // 폭발 피해 (투척술): 내가 던진 폭발물로 준 피해
            if (info.isExplosion && info.fromCharacter != null && info.fromCharacter == _main)
            {
                _skills.AddXp("throwing", damage * Rates.ThrowingPerExplosionDamage);
            }

            // 내가 준 피해 (사격 / 정밀 사격)
            CharacterMainControl from = info.fromCharacter;
            if (from != null && from == _main && !IsMeleeDamage(info))
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
            _skills.AddXp("assault", Rates.AssaultPerShot);
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

        private void HandleStartUseItem(Item item)
        {
            // 아이템을 사용한 직후 몇 초 동안의 체력 증가를 '회복' 경험치로 인정한다.
            _medicalUseUntil = Time.time + 6f;
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
                string text = "[스킬] " + def.NameKo + " Lv." + level;
                if (level >= _skills.MaxLevel)
                {
                    text += " ★엘리트 달성!";
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
                new[] { "Malgun Gothic", "맑은 고딕", "Gulim", "Arial Unicode MS", "Arial" }, 15);
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

