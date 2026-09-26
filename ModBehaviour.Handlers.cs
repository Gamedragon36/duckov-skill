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

        /// <summary>근접 명중 간격 실측용 (공격속도 효과가 실제로 반영되는지 확인)</summary>
        private float _lastMeleeHitTime = -10f;

        private void HandleHurt(Health health, DamageInfo info)
        {
            if (health == null || _skills == null || _main == null)
            {
                return;
            }
            float damage = info.finalDamage;
            if (damage <= 0f)
            {
                return;
            }

            bool hurtMe = health.TryGetCharacter() == _main;
            bool fromMe = info.fromCharacter != null && info.fromCharacter == _main;

            // ---- 내가 맞은 것 ----
            if (hurtMe)
            {
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
            }

            // ---- 내가 준 피해 (투척·근접·사격·정밀 사격) ----
            //  0.0.9 수정: 예전에는 이 함수가 '내가 맞은 경우'만 통과시키고 바로 return 해서
            //   아래(내가 준 피해) 코드가 **한 번도 실행되지 않았다**(로그 '근접 공격 감지' 0회).
            //   → 근접 명중·사격 명중·정밀 사격 경험치가 오르지 않던 원인.
            if (fromMe && !hurtMe)
            {
                // 지속 피해(출혈 등)는 '명중'으로 세지 않는다(경험치 중복 방지)
                if (info.isFromBuffOrEffect)
                {
                    return;
                }

                if (info.isExplosion)
                {
                    // 투척술: 내가 던진 폭발물로 준 피해
                    _skills.AddXp("throwing", damage * Rates.ThrowingPerExplosionDamage);
                }
                else if (IsMeleeDamage(info))
                {
                    // 근접 전투: 명중할 때마다
                    float meleeNow = Time.time;
                    if (_lastMeleeHitTime > 0f)
                    {
                        // 실측 로그: 이전 근접 명중과의 간격 — 공격속도(CA_Attack 시간 감소)가 반영되는지 확인용
                        Debug.Log("[Dskill] 근접 공격 감지: 이전 명중과 " + (meleeNow - _lastMeleeHitTime).ToString("0.##") + "초 간격");
                    }
                    _lastMeleeHitTime = meleeNow;
                    _skills.AddXp("melee", Rates.MeleePerHit);
                }
                else if (IsGunDamage(info))
                {
                    _skills.AddXp("assault", Rates.AssaultPerHit);

                    // 거리는 '내 캐릭터'와 '맞은 대상' 사이로 잰다.
                    //  0.0.9 수정: 예전에는 info.damagePoint 를 썼는데 총알 피해에서는 값이 비어 있어(0)
                    //   사실상 '원점~플레이어' 거리가 나왔고, 그 값이 20m 를 넘어 **가까운 거리에서도 정밀 사격이 올랐다**.
                    float distance = MeasureHitDistance(health, info);
                    if (distance >= Rates.MarksmanshipMinDistance)
                    {
                        float gain = Rates.MarksmanshipPerHit;
                        bool crit = info.crit != 0;
                        if (crit)
                        {
                            gain += Rates.MarksmanshipPerCrit;
                        }
                        _skills.AddXp("marksmanship", gain);
                        Debug.Log("[Dskill] 정밀 사격 +" + gain.ToString("0.#") + " (" + distance.ToString("0.#") +
                                  "m" + (crit ? ", 치명타" : "") + " / 무기ID " + info.fromWeaponItemID + ")");
                    }
                    // 기준 미만은 로그를 남기지 않는다 — 명중마다 찍혀 로그가 폭증한다(2026-09-26 감사에서 제거)
                }
            }
        }

        /// <summary>이 피해가 총기(사격)로 준 것인지 확인한다.
        ///  게임이 fromWeaponItemID 를 채우지 않는 경우가 있어,
        ///  근접·폭발·상태이상이 아니면서 총을 들고 있으면 사격으로 인정한다
        ///  (정밀 사격이 동작하지 않던 원인 보완 · 2026-09-26).</summary>
        private bool IsGunDamage(DamageInfo info)
        {
            if (_gun == null || _gun.Item == null)
            {
                return false;
            }
            if (info.fromWeaponItemID == _gun.Item.TypeID)
            {
                return true;
            }
            return !IsMeleeDamage(info) && !info.isExplosion;
        }

        /// <summary>내 캐릭터와 '맞은 대상' 사이의 거리(m).
        ///  총알 피해의 damagePoint 는 비어 있는 경우가 있고, 체력 컴포넌트 위치도 0 인 대상이 있어
        ///  **대상 캐릭터 위치 → 체력 컴포넌트 위치 → 피해 지점** 순으로 믿을 수 있는 값을 고른다.
        ///  (위치를 알 수 없으면 0 을 돌려 정밀 사격을 주지 않는다 — 가까운 거리 오지급 방지)</summary>
        private float MeasureHitDistance(Health health, DamageInfo info)
        {
            if (_main == null || health == null)
            {
                return 0f;
            }
            Vector3 myPosition = _main.transform.position;

            CharacterMainControl victim = health.TryGetCharacter();
            if (victim != null && victim != _main)
            {
                return Vector3.Distance(myPosition, victim.transform.position);
            }

            Vector3 target = health.transform.position;
            if (target != Vector3.zero)
            {
                return Vector3.Distance(myPosition, target);
            }

            if (info.damagePoint != Vector3.zero)
            {
                return Vector3.Distance(myPosition, info.damagePoint);
            }
            return 0f;
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
            else if (IsGunDamage(info))
            {
                // 검증용: 사격 처치 거리를 남긴다(정밀 사격 기준 20m 와 비교해 확인)
                Debug.Log("[Dskill] 사격 처치: " + MeasureHitDistance(health, info).ToString("0.#") +
                          "m (정밀 사격 기준 " + Rates.MarksmanshipMinDistance.ToString("0") + "m)");
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

