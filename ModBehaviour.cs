using System;
using System.Collections.Generic;
using Duckov.Buffs;
using ItemStatsSystem;
using Saves;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dskill
{
    /// <summary>
    /// 타르코프식 스킬 시스템 (v0.2.0)
    /// - 게임에서 특정 행동을 하면 스킬 경험치가 쌓인다.
    /// - 레벨이 오르면 캐릭터 스탯 보너스가 자동으로 붙는다.
    /// - F6 키로 스킬 창을 열 수 있다.
    /// (이 파일은 뼈대와 주기 처리 / 실제 경험치 처리와 화면은 ModBehaviour.Handlers.cs)
    /// </summary>
    public partial class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public const string Version = "0.0.6";

        /// <summary>창작마당 업로드 시 함께 기록되는 변경 메모 (릴리즈마다 갱신)</summary>
        private const string ChangeNote = "repair XP fix (equipped+stash), throwing/looting unlock, heal speed, dash tuning";

        private Config _config;
        private MetaProgress _meta;
        private SkillSystem _skills;
        private bool _metaNoticeShown;

        private Key _toggleKey = Key.F6;
        private string _toggleKeyName = "F6";
        private bool _panelVisible;
        private Font _font;

        // 캐릭터 / 무기 참조 (레벨이 바뀌면 갱신)
        private CharacterMainControl _main;
        private Item _mainItem;
        private ItemAgent_Gun _gun;
        private ItemAgent_MeleeWeapon _melee;
        private CharacterBuffManager _buffManager;

        // 주기 처리용
        private float _tickTimer;
        private float _saveTimer;
        private Vector3 _lastPosition;
        private float _lastHealth;
        private float _lastEnergy;
        private float _lastWater;
        private float _levelInitTime;
        private float _medicalUseUntil;
        private float _foodUseUntil;
        private bool _firstTick = true;

        /// <summary>수리 경험치용: 아이템별 직전 내구도</summary>
        private readonly Dictionary<Item, float> _lastDurability = new Dictionary<Item, float>();
        private float _repairStorageNextScan;   // 창고(PlayerStorage) 수리 감지 스로틀
        private readonly List<Item> _itemBuffer = new List<Item>();

        // ------------------------------------------------------------------
        // 시작 / 종료
        // ------------------------------------------------------------------

        protected override void OnAfterSetup()
        {
            try
            {
                _config = Config.Load();
                Locale.Setup(_config.Language);           // 표시 언어 결정 (auto = OS 언어)
                _meta = new MetaProgress(_config);
                _meta.Rescan(true);                       // 다른 세이브 기록으로 계승 보너스 계산
                _skills = new SkillSystem(_config, _meta);
                ParseHotkey(_config.Hotkey);

                LevelManager.OnLevelInitialized += HandleLevelInitialized;
                Health.OnHurt += HandleHurt;
                Health.OnDead += HandleDead;
                CharacterMainControl.OnMainCharacterStartUseItem += HandleStartUseItem;
                SavesSystem.OnSetFile += HandleSetFile;
                SavesSystem.OnCollectSaveData += HandleCollectSaveData;
                Duckov.Economy.StockShop.OnItemSoldByPlayer += HandleItemSold;
                Duckov.Economy.StockShop.OnItemPurchased += HandleItemPurchased;
                Duckov.BlackMarkets.BlackMarket.onRequestRefreshTime += HandleBlackMarketRefresh;
                Duckov.Buildings.BuildingManager.OnBuildingBuilt += HandleBuildingBuilt;
                AIMainBrain.OnPlayerHearSound += HandlePlayerHearSound;
                CraftingManager.OnItemCrafted += HandleItemCrafted;
                CraftingManager.OnFormulaUnlocked += HandleFormulaUnlocked;
                _skills.OnLevelUp += HandleLevelUp;

                _skills.Load();

                // config.ini 의 reset_skills = true 요청: 스킬을 모두 0으로 초기화하고 플래그를 되돌린다
                if (_config.ResetSkills)
                {
                    _skills.ResetAll();
                    _config.ClearResetFlag();
                    Debug.Log("[Dskill] 요청에 따라 스킬 초기화를 실행했고 reset_skills 를 false 로 되돌렸습니다.");
                }

                // config.ini 의 upload_now = true 요청: 창작마당 업로드를 1회 실행한다(제작자용)
                if (_config.UploadNow)
                {
                    _config.ClearUploadFlag();
                    StartCoroutine(UploadWorkshopAfterDelay(20f));
                }

                Debug.Log("[Dskill] 모드 로드 완료 v" + Version + " (스킬 창 키: " + _toggleKeyName +
                          " / 언어: " + Locale.CurrentCode + (Locale.AutoDetected ? " auto" : " 지정") + ")");
            }
            catch (Exception e)
            {
                Debug.LogError("[Dskill] 초기화 실패: " + e);
            }
        }

        /// <summary>게임이 준비될 때까지 잠시 기다렸다가 창작마당 업로드를 요청한다.</summary>
        private System.Collections.IEnumerator UploadWorkshopAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            TryUploadToWorkshop();
        }

        /// <summary>
        /// 창작마당 업로드를 게임 자체 기능으로 1회 실행한다(제작자용).
        /// 게임의 Duckov.Modding.SteamWorkshopManager.UploadWorkshopItem 을 리플렉션으로 호출한다.
        /// 실패해도 게임에 영향을 주지 않는다.
        /// </summary>
        private void TryUploadToWorkshop()
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(typeof(ModBehaviour).Assembly.Location);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("[Dskill] 업로드 중단: 모드 폴더 경로를 찾지 못했습니다.");
                    return;
                }

                Type managerType = null;
                foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    managerType = asm.GetType("Duckov.Modding.SteamWorkshopManager", false);
                    if (managerType != null)
                    {
                        break;
                    }
                }
                if (managerType == null)
                {
                    Debug.LogWarning("[Dskill] 업로드 중단: SteamWorkshopManager 를 찾지 못했습니다.");
                    return;
                }

                System.Reflection.PropertyInfo instanceProperty =
                    managerType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                object manager = instanceProperty != null ? instanceProperty.GetValue(null, null) : null;
                if (manager == null)
                {
                    Debug.LogWarning("[Dskill] 업로드 중단: SteamWorkshopManager 인스턴스가 없습니다(스팀 미연결).");
                    return;
                }

                System.Reflection.MethodInfo method = managerType.GetMethod("UploadWorkshopItem",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (method == null)
                {
                    Debug.LogWarning("[Dskill] 업로드 중단: UploadWorkshopItem 메서드를 찾지 못했습니다.");
                    return;
                }

                Debug.Log("[Dskill] 창작마당 업로드를 요청합니다: " + path);

                // 설정 파일은 배포본에 넣지 않는다.
                // (구독자가 제작자 설정을 물려받지 않고, 자기 환경에 맞는 기본값으로 새로 시작하도록)
                string configPath = System.IO.Path.Combine(path, "config.ini");
                // 백업은 모드 폴더 '밖'에 둔다 → 업로드 내용에 섞이지 않게 함
                string parentDir = System.IO.Path.GetDirectoryName(path);
                string backupPath = System.IO.Path.Combine(
                    string.IsNullOrEmpty(parentDir) ? System.IO.Path.GetTempPath() : parentDir,
                    "Dskill-config.ini.author");
                bool moved = false;
                try
                {
                    if (System.IO.File.Exists(backupPath))
                    {
                        System.IO.File.Delete(backupPath);
                    }
                    if (System.IO.File.Exists(configPath))
                    {
                        System.IO.File.Move(configPath, backupPath);
                        moved = true;
                        Debug.Log("[Dskill] 업로드 동안 설정 파일을 잠시 제외합니다: " + configPath);
                    }
                }
                catch (Exception moveError)
                {
                    Debug.LogWarning("[Dskill] 설정 파일 제외 실패(그대로 업로드됩니다): " + moveError.Message);
                }

                method.Invoke(manager, new object[] { path, "Duckov Skill " + Version + " - " + ChangeNote });

                if (moved)
                {
                    StartCoroutine(RestoreConfigAfterUpload(configPath, backupPath));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 업로드 요청 실패: " + e.Message);
            }
        }

        /// <summary>업로드가 끝날 때까지 기다렸다가 설정 파일을 원래 자리로 되돌린다.</summary>
        private System.Collections.IEnumerator RestoreConfigAfterUpload(string configPath, string backupPath)
        {
            yield return new WaitForSeconds(45f);   // 업로드(약 5~10초) + 여유
            try
            {
                if (System.IO.File.Exists(backupPath) && !System.IO.File.Exists(configPath))
                {
                    System.IO.File.Move(backupPath, configPath);
                    Debug.Log("[Dskill] 업로드 후 설정 파일을 되돌렸습니다.");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 설정 파일 복구 실패: " + e.Message);
            }
        }

        protected override void OnBeforeDeactivate()
        {
            try
            {
                LevelManager.OnLevelInitialized -= HandleLevelInitialized;
                Health.OnHurt -= HandleHurt;
                Health.OnDead -= HandleDead;
                CharacterMainControl.OnMainCharacterStartUseItem -= HandleStartUseItem;
                SavesSystem.OnSetFile -= HandleSetFile;
                SavesSystem.OnCollectSaveData -= HandleCollectSaveData;
                Duckov.Economy.StockShop.OnItemSoldByPlayer -= HandleItemSold;
                Duckov.Economy.StockShop.OnItemPurchased -= HandleItemPurchased;
                Duckov.BlackMarkets.BlackMarket.onRequestRefreshTime -= HandleBlackMarketRefresh;
                Duckov.Buildings.BuildingManager.OnBuildingBuilt -= HandleBuildingBuilt;
                AIMainBrain.OnPlayerHearSound -= HandlePlayerHearSound;
                CraftingManager.OnItemCrafted -= HandleItemCrafted;
                CraftingManager.OnFormulaUnlocked -= HandleFormulaUnlocked;

                if (_gun != null)
                {
                    _gun.OnShootEvent -= HandleShoot;
                    _gun.OnLoadedEvent -= HandleLoaded;
                    _gun = null;
                }
                UnsubscribeBuffs();
                RestoreDash();
                RestoreHideoutEffects();

                if (_skills != null)
                {
                    _skills.OnLevelUp -= HandleLevelUp;
                    _skills.Save();
                    _skills.Detach();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 종료 처리 중 오류: " + e.Message);
            }
            Debug.Log("[Dskill] 모드 비활성화");
        }

        private void ParseHotkey(string text)
        {
            Key parsed;
            if (!string.IsNullOrEmpty(text) && Enum.TryParse(text.Trim(), true, out parsed))
            {
                _toggleKey = parsed;
                _toggleKeyName = text.Trim().ToUpperInvariant();
            }
        }

        private void HandleLevelInitialized()
        {
            _levelInitTime = Time.time;
            _firstTick = true;
            _lastDurability.Clear();
            _medicalUseUntil = 0f;
            _foodUseUntil = 0f;

            // 레벨이 바뀌면 이전 씬의 오브젝트 추적 기록은 의미가 없으므로 비운다
            // (개수 초과로 중간에 비울 때 이미 처리한 대상이 다시 처리되는 문제 방지)
            _handledGrenades.Clear();
            _inspectingSeen.Clear();
            _knownItems.Clear();
            _pendingInspect.Clear();

            // 레벨(레이드·기지)에 들어갈 때 계승 보너스를 확인한다(60초 스로틀로 파일 읽기 최소화)
            if (_meta != null)
            {
                _meta.Rescan();

                if (!_metaNoticeShown)
                {
                    string notice = _meta.StartupNotice();
                    if (!string.IsNullOrEmpty(notice))
                    {
                        _metaNoticeShown = true;
                        try
                        {
                            Duckov.UI.NotificationText.Push(notice);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("[Dskill] 계승 알림 실패: " + e.Message);
                        }
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        // 주기 처리
        // ------------------------------------------------------------------

        private void Update()
        {
            HandleInput();
            DetectDash();

            _tickTimer += Time.unscaledDeltaTime;
            if (_tickTimer >= 0.5f)
            {
                float elapsed = _tickTimer;
                _tickTimer = 0f;
                try
                {
                    Tick(elapsed);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[Dskill] 주기 처리 오류: " + e.Message);
                }
            }
        }

        private void HandleInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }
            if (keyboard[_toggleKey].wasPressedThisFrame)
            {
                _panelVisible = !_panelVisible;

                // 창을 열 때 계승 정보를 최신으로 맞춘다(내부 스로틀로 과도한 파일 읽기 방지)
                if (_panelVisible && _meta != null)
                {
                    _meta.Rescan();
                }
            }
            if (_panelVisible && keyboard.escapeKey.wasPressedThisFrame)
            {
                _panelVisible = false;
            }
        }

        private void Tick(float elapsed)
        {
            if (_skills == null)
            {
                return;
            }

            RefreshReferences();
            if (_mainItem == null)
            {
                return;
            }

            // ---- 이동 관련 (근력 / 지구력 / 은신 이동) ----
            bool moved = false;
            if (_main != null)
            {
                Vector3 position = _main.transform.position;
                if (!_firstTick)
                {
                    moved = (position - _lastPosition).sqrMagnitude > 0.0025f;   // 5cm 이상 이동
                }
                _lastPosition = position;
            }

            if (moved && _main != null)
            {
                if (_main.Running)
                {
                    _skills.AddXp("endurance", elapsed * Rates.EndurancePerSecond);
                }
                else
                {
                    _skills.AddXp("covert", elapsed * Rates.CovertPerSecond);
                }

                float maxWeight = _main.MaxWeight;
                if (maxWeight > 0.01f)
                {
                    float ratio = _mainItem.TotalWeight / maxWeight;
                    if (ratio >= 0.7f)
                    {
                        float perSecond = ratio >= 0.9f ? Rates.StrengthHeavyPerSecond : Rates.StrengthPerSecond;
                        _skills.AddXp("strength", elapsed * perSecond);
                    }
                }
            }

            // ---- 회복 (회복 아이템을 쓴 뒤 체력이 늘어난 만큼) ----
            if (_main != null && _main.Health != null)
            {
                float health = _main.Health.CurrentHealth;
                bool windowOpen = Time.time < _medicalUseUntil;
                if (!_firstTick && health > _lastHealth && windowOpen && Time.time - _levelInitTime > 3f)
                {
                    _skills.AddXp("health", (health - _lastHealth) * Rates.HealthPerHeal);
                    // 이 종류는 '회복 아이템'으로 학습 → 다음부터 사용 시간 감소(치료 속도) 적용
                    if (_lastUsedItem != null)
                    {
                        _healingTypeIds.Add(_lastUsedItem.TypeID);
                    }
                }
                _lastHealth = health;
            }

            // ---- 신진대사 (음식/물을 쓴 직후 포만감·수분이 늘어난 만큼) ----
            if (_main != null)
            {
                float energy = _main.CurrentEnergy;
                float water = _main.CurrentWater;
                bool foodWindow = Time.time < _foodUseUntil;
                if (!_firstTick && foodWindow)
                {
                    if (energy - _lastEnergy > Rates.MetabolismMinDelta)
                    {
                        _skills.AddXp("metabolism", (energy - _lastEnergy) * Rates.MetabolismPerPoint);
                    }
                    if (water - _lastWater > Rates.MetabolismMinDelta)
                    {
                        _skills.AddXp("metabolism", (water - _lastWater) * Rates.MetabolismPerPoint);
                    }
                }
                _lastEnergy = energy;
                _lastWater = water;
            }

            CheckRepair();
            ScanGrenades(elapsed);
            ScanLoot(elapsed);
            CheckPickups();
            ApplyDashSettings();
            DetectHideoutTime(elapsed);
            ApplyHideoutEffects(elapsed);
            DetectNightTime(elapsed);
            _skills.EnsureApplied(_mainItem);

            _firstTick = false;

            // 안전장치용 주기 저장 (게임 저장과 별개)
            _saveTimer += elapsed;
            if (_saveTimer >= 120f)
            {
                _saveTimer = 0f;
                _skills.Save();
            }
        }

        // ------------------------------------------------------------------
        // 참조 갱신 / 수리 감지
        // ------------------------------------------------------------------

        /// <summary>메인 캐릭터 / 아이템 / 총 / 버프 관리자를 최신 상태로 맞춘다.</summary>
        private void RefreshReferences()
        {
            CharacterMainControl main = CharacterMainControl.Main;
            if (main != _main)
            {
                UnsubscribeBuffs();
                _main = main;
                _mainItem = null;
                _gun = null;
                _melee = null;
                _firstTick = true;
                _lastDurability.Clear();
            }
            if (_main == null)
            {
                return;
            }

            Item item = _main.CharacterItem;
            if (item != _mainItem)
            {
                _mainItem = item;
                _firstTick = true;
                _lastDurability.Clear();
                if (_skills != null && item != null)
                {
                    _skills.EnsureApplied(item);
                }
            }

            ItemAgent_Gun gun = _main.GetGun();
            if (gun != _gun)
            {
                if (_gun != null)
                {
                    _gun.OnShootEvent -= HandleShoot;
                    _gun.OnLoadedEvent -= HandleLoaded;
                }
                _gun = gun;
                if (_gun != null)
                {
                    _gun.OnShootEvent += HandleShoot;
                    _gun.OnLoadedEvent += HandleLoaded;
                }
            }

            _melee = _main.GetMeleeWeapon();
            SubscribeBuffs();
        }

        private void SubscribeBuffs()
        {
            CharacterBuffManager manager = _main != null ? _main.GetBuffManager() : null;
            if (manager == _buffManager)
            {
                return;
            }
            UnsubscribeBuffs();
            _buffManager = manager;
            if (_buffManager != null)
            {
                _buffManager.onAddBuff += HandleAddBuff;
            }
        }

        private void UnsubscribeBuffs()
        {
            if (_buffManager != null)
            {
                _buffManager.onAddBuff -= HandleAddBuff;
                _buffManager = null;
            }
        }

        /// <summary>수리 경험치: 들고 있는 장비의 내구도가 늘어난 만큼 점수를 준다.</summary>
        private void CheckRepair()
        {
            if (_mainItem == null || _skills == null)
            {
                return;
            }

            _itemBuffer.Clear();
            CollectItems(_mainItem, _itemBuffer);

            // 0.0.6: 기지에서는 '창고(PlayerStorage)'의 장비를 수리하므로 창고도 확인한다.
            //        (캐릭터 인벤만 보면 기지 수리를 전혀 감지하지 못했다 — 수리 경험치 0 원인)
            //        창고는 최대 16,384칸까지 커질 수 있어 2초에 한 번만 훑는다.
            if (Time.time >= _repairStorageNextScan)
            {
                _repairStorageNextScan = Time.time + 2f;
                try
                {
                    Inventory storage = PlayerStorage.Inventory;
                    if (storage != null && storage.Content != null)
                    {
                        foreach (Item stored in storage.Content)
                        {
                            CollectItems(stored, _itemBuffer);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[Dskill] 창고 아이템 확인 실패(수리 경험치 누락 가능): " + e.Message);
                }
            }

            float gained = 0f;
            foreach (Item item in _itemBuffer)
            {
                if (item == null || !item.UseDurability)
                {
                    continue;
                }
                float current = item.Durability;
                float previous;
                if (_lastDurability.TryGetValue(item, out previous) && current > previous)
                {
                    gained += current - previous;
                }
                _lastDurability[item] = current;
            }

            if (gained > 0.5f)
            {
                _skills.AddXp("repair", gained * Rates.RepairPerDurability);
            }

            // 더 이상 가지고 있지 않은 아이템 정보 정리
            if (_lastDurability.Count > _itemBuffer.Count + 32)
            {
                List<Item> keys = new List<Item>(_lastDurability.Keys);
                foreach (Item key in keys)
                {
                    if (key == null || !_itemBuffer.Contains(key))
                    {
                        _lastDurability.Remove(key);
                    }
                }
            }
        }

        /// <summary>아이템과 그 안에 든 아이템을 모두 모은다.</summary>
        private static void CollectItems(Item root, List<Item> result)
        {
            if (root == null || result.Contains(root))
            {
                return;
            }
            result.Add(root);

            // 0.0.6: **장착 슬롯(무기·방어구 등)도 포함**한다.
            //  (장비를 직접 수리하는 경우, 슬롯은 Inventory 가 아니라 Slots 에 있어서 예전에는 감지되지 않았다)
            try
            {
                var slots = root.Slots;
                if (slots != null)
                {
                    foreach (var slot in slots)
                    {
                        if (slot != null && slot.Content != null)
                        {
                            CollectItems(slot.Content, result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Dskill] 슬롯 아이템 확인 실패(무시): " + e.Message);
            }

            Inventory inventory = root.Inventory;
            if (inventory == null)
            {
                return;
            }
            List<Item> children = inventory.Content;
            if (children == null)
            {
                return;
            }
            foreach (Item child in children)
            {
                CollectItems(child, result);
            }
        }
    }
}
