﻿using System;
using System.Collections.Generic;
using System.Linq;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders;
using static STRINGS.UI.CODEX;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER.BUTTONS;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using Microsoft.SqlServer.Server;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER;
using Klei.AI;
using System.Threading.Tasks;
using static STRINGS.DUPLICANTS.PERSONALITIES;
using Klei.CustomSettings;
using ProcGen;
using System.Text.RegularExpressions;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER.ASTEROIDTRAITS.CONTENT.TRAITCONTAINER.SCROLLAREA.CONTENT.LISTVIEWENTRYPREFAB;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;
using static ClusterTraitGenerationManager.CGM_MainScreen_UnityScreen;
using static CustomGameSettings;
using Database;

namespace ClusterTraitGenerationManager
{
    public class CGM_MainScreen_UnityScreen : KModalScreen
    {
        ////GridLayouter galleryGridLayouter;
        //GridLayoutSizeAdjustment galleryGridLayouter;

        private Dictionary<StarmapItemCategory, CategoryItem> categoryToggles = new Dictionary<StarmapItemCategory, CategoryItem>();
        private Dictionary<StarmapItem, GalleryItem> planetoidGridButtons = new Dictionary<StarmapItem, GalleryItem>();

        ///Gallery
        GameObject PlanetoidEntryPrefab;
        private GameObject galleryGridContainer;

        GameObject StoryTraitEntryPrefab;
        private GameObject StoryTraitGridContainer;

        GameObject CustomGameSettingsContainer;

        GameObject VanillaStarmapItemContainer;
        private GameObject VanillaStarmapItemPrefab;

        ///GalleryContents
        GameObject CustomGameSettingsContent;
        GameObject VanillaStarmapItemContent;
        GameObject StoryTraitGridContent;
        GameObject ClusterItemsContent;


        ///Categories
        GameObject PlanetoidCategoryPrefab;
        public GameObject categoryListContent;


        GameObject VanillaStarmapButton;

        GameObject StoryTraitButton;
        GameObject GameSettingsButton;

        private LocText galleryHeaderLabel;
        private LocText categoryHeaderLabel;
        private LocText selectionHeaderLabel;

        private bool init = false;

        private StarmapItem _selectedPlanet = null;// new StarmapItem("none", StarmapItemCategory.Starter,null);
        StarmapItem SelectedPlanet
        {
            get { return _selectedPlanet; }
            set
            {
                _selectedPlanet = value;
                this.RefreshView();
            }
        }




        ///<GameSettings>

        private FCycle ImmuneSystem;
        private FCycle CalorieBurn;
        private FCycle Morale;
        private FCycle Durability;
        private FCycle MeteorShowers;
        private FCycle Radiation;
        private FCycle Stress;
        private FToggle2 StressBreaks;
        private FToggle2 CarePackages;
        private FToggle2 SandboxMode;
        private FToggle2 FastWorkersMode;
        private FToggle2 SaveToCloud;
        private FToggle2 Teleporters;

        /// </GameSettings>
        StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;


        Dictionary<string, GameObject> SeasonTypes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> ShowerTypes = new Dictionary<string, GameObject>();


        Dictionary<string, GameObject> PlanetBiomes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();



        private GameObject Details_StoryTraitContainer;
        private Image StoryTraitImage;
        private LocText StoryTraitDesc;
        public FToggle2 StoryTraitToggle;


        private GameObject Details_VanillaPOIContainer;
        private GameObject VanillaPOIResourceContainer;
        private GameObject VanillaPOIResourcePrefab;
        private LocText VanillaPOI_SizeAmountDesc;
        private LocText VanillaPOI_ReplenishmentAmountDesc;
        private LocText VanillaPOI_ArtifactDesc;
        private FButton VanillaPOI_RemovePOIBtn;
        List<GameObject> VanillaPOI_Resources = new List<GameObject>();



        private FSlider ClusterSize;

        public FInputField2 SeedInput_Main;
        public FButton SeedCycleButton_Main;
        public FToggle2 SeedRerollsTraitsToggle_Main;



        private LocText StarmapItemEnabledText;
        private FToggle2 StarmapItemEnabled;

        private FSlider NumberToGenerate;
        private FSlider NumberOfRandomClassics;
        private GameObject NumberOfRandomClassicsGO;


        private UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider MinMaxDistanceSlider;
        private LocText SpawnDistanceText;
        private FSlider BufferDistance;

        private GameObject AsteroidSize;
        private LocText AsteroidSizeLabel;
        private ToolTip AsteroidSizeTooltip;

        private FInputField2 PlanetSizeWidth;
        private FInputField2 PlanetSizeHeight;

        private FCycle PlanetSizeCycle;
        private FCycle PlanetRazioCycle;

        private GameObject MeteorSelector;
        private GameObject ActiveMeteorsContainer;
        private GameObject MeteorPrefab;
        private GameObject ActiveSeasonsContainer;
        private GameObject SeasonPrefab;
        private FButton AddSeasonButton;


        private GameObject AsteroidTraits;
        private GameObject ActiveTraitsContainer;
        private GameObject TraitPrefab;
        private FButton AddTraitButton;
        private FButton RandomTraitBlacklistOpener;
        private FButton RandomTraitDeleteButton;


        private GameObject PlanetBiomesGO;
        private GameObject ActiveBiomesContainer;
        private GameObject BiomePrefab;


        private static FButton ResetButton;
        private static FButton ResetAllButton;
        private FButton ReturnButton;
        private FButton PresetsButton;
        //private FButton SettingsButton;
        private FButton GenerateClusterButton;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.canBackoutWithRightClick = true;
            base.ConsumeMouseScroll = true;
            this.SetHasFocus(true);


#if DEBUG
            //UIUtils.ListAllChildrenPath(this.transform);
#endif

            ///Categories
            PlanetoidCategoryPrefab = transform.Find("Categories/Content/Item").gameObject;
            categoryListContent = transform.Find("Categories/Content").gameObject;
            categoryHeaderLabel = transform.Find("Categories/Header/Label").GetComponent<LocText>();

            StoryTraitButton = transform.Find("Categories/FooterContent/StoryTraits").gameObject;
            GameSettingsButton = transform.Find("Categories/FooterContent/GameSettings").gameObject;
            
            ///Gallery
            StoryTraitGridContainer = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer").gameObject;
            StoryTraitEntryPrefab = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer/Item").gameObject;

            VanillaStarmapItemContainer = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer").gameObject;
            VanillaStarmapItemPrefab = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/Item").gameObject;

            CustomGameSettingsContainer = transform.Find("ItemSelection/CustomGameSettingsContent/CustomGameSettingsContainer").gameObject;

            galleryGridContainer = transform.Find("ItemSelection/StarItemContent/StarItemContainer").gameObject;
            PlanetoidEntryPrefab = transform.Find("ItemSelection/StarItemContent/StarItemContainer/Item").gameObject;
            galleryHeaderLabel = transform.Find("ItemSelection/Header/Label").GetComponent<LocText>();
            ///GalleryContainers

            CustomGameSettingsContent = transform.Find("ItemSelection/CustomGameSettingsContent").gameObject;
            VanillaStarmapItemContent = transform.Find("ItemSelection/VanillaStarmapContent").gameObject;
            StoryTraitGridContent = transform.Find("ItemSelection/StoryTraitsContent").gameObject;
            ClusterItemsContent = transform.Find("ItemSelection/StarItemContent").gameObject;



            ///Details
            selectionHeaderLabel = transform.Find("Details/Header/Label").GetComponent<LocText>();

            //galleryGridLayouter = galleryGridContainer.AddOrGet<GridLayoutSizeAdjustment>();
            //galleryGridLayouter.SetValues(100, 140);
            //galleryGridLayouter = new GridLayouter
            //{
            //    minCellSize = 80f,
            //    maxCellSize = 160f,
            //    targetGridLayouts = new List<GridLayoutGroup>() { galleryGridContainer.GetComponent<GridLayoutGroup>() }
            //};
            Init();


            OnResize();
        }

        public void DoAndRefreshView(System.Action action)
        {
            action.Invoke();
            this.RefreshGallery();
            this.RefreshDetails();
        }
        public override float GetSortKey() => 20f;

        public override void OnActivate() => this.OnShow(true);

        public static bool AllowedToClose()
        {
            return (
                (TraitSelectorScreen.Instance != null ? !TraitSelectorScreen.Instance.IsCurrentlyActive : true)
                    && (SeasonSelectorScreen.Instance != null ? !SeasonSelectorScreen.Instance.IsCurrentlyActive : true)
                    && (CustomSettingsController.Instance != null ? !CustomSettingsController.Instance.IsCurrentlyActive : true)
                    );
        }

        bool overrideToWholeNumbers = false;
        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.Controller.GetKeyDown(KKeyCode.LeftShift))
            {
                overrideToWholeNumbers = !overrideToWholeNumbers;
                e.Consumed = true;
                RefreshDetails();
            }

            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                if (AllowedToClose())
                    Show(show: false);
            }

            base.OnKeyDown(e);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public bool IsCurrentlyActive = false;
        public override void OnShow(bool show)
        {
            //SgtLogger.l("SHOWING: " + show);
            //this.isActive = show;
            base.OnShow(show);
            if (!show)
                return;

            if (!init)
            {
                Init();
                init = true;
            }
            IsCurrentlyActive = show;

            //// this.galleryGridLayouter.RequestGridResize();

            OnResize();
            //RefreshWithDelay(() => OnResize(true),300);
            ScreenResize.Instance.OnResize += () => OnResize();
            RefreshView();
            DoWithDelay(() => this.SelectCategory(StarmapItemCategory.Starter), 25);
        }
        static async Task DoWithDelay(System.Action task, int ms)
        {
            await Task.Delay(ms);
            task.Invoke();
        }
        public void OnResize()
        {
            var rectMain = this.rectTransform();
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
            //if(galleryGridLayouter!=null)    
            //    this.galleryGridLayouter.RequestGridResize();
        }

        public void RefreshView()
        {
            this.RefreshCategories();
            this.RefreshGallery();
            this.RefreshDetails();
        }
        private void RefreshCategories()
        {
            foreach (var categoryToggle in this.categoryToggles)
            {
                Sprite PlanetSprite = null;
                switch (categoryToggle.Key)
                {
                    case StarmapItemCategory.Starter:
                        PlanetSprite = CustomCluster.StarterPlanet != null ? CustomCluster.StarterPlanet.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.Warp:
                        PlanetSprite = CustomCluster.WarpPlanet != null ? CustomCluster.WarpPlanet.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.Outer:
                        PlanetSprite = CustomCluster.OuterPlanets.Count > 0 ? CustomCluster.OuterPlanets.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.POI:
                        PlanetSprite = CustomCluster.POIs.Count > 0 ? CustomCluster.POIs.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                }
                categoryToggle.Value.Refresh(SelectedCategory, PlanetSprite);
            }
        }

        #region gamesettings
        private void AddMissingCustomGameSetting(SettingConfig type)
        {
            SgtLogger.warning(type.GetType().ToString() + " value not found, defaulting");
            CustomGameSettings.Instance.QualitySettings[type.id] = type;
        }
        private void LoadGameSettings()
        {
            var instance = CustomGameSettings.Instance;
            bool isNoSweat = instance.customGameMode == CustomGameMode.Nosweat;

            ///ImmuneSystem
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.ImmuneSystem.id))
            {
                ImmuneSystem.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.ImmuneSystem);
                ImmuneSystem.Value = isNoSweat ? CustomGameSettingConfigs.ImmuneSystem.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.ImmuneSystem.GetDefaultLevelId();
            }

            ///CalorieBurn
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CalorieBurn.id))
            {
                CalorieBurn.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.CalorieBurn);
                CalorieBurn.Value = isNoSweat ? CustomGameSettingConfigs.CalorieBurn.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.CalorieBurn.GetDefaultLevelId();
            }

            ///Morale
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Morale.id))
            {
                Morale.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Morale);
                Morale.Value = isNoSweat ? CustomGameSettingConfigs.Morale.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Morale.GetDefaultLevelId();
            }

            ///Durability (suits)
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Durability.id))
            {
                Durability.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Durability).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Durability);
                Durability.Value = isNoSweat ? CustomGameSettingConfigs.Durability.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Durability.GetDefaultLevelId();
            }

            ///MeteorShowers
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.MeteorShowers.id))
            {
                MeteorShowers.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.MeteorShowers).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.MeteorShowers);
                MeteorShowers.Value = isNoSweat ? CustomGameSettingConfigs.MeteorShowers.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.MeteorShowers.GetDefaultLevelId();
            }

            ///Radiation
            if (DlcManager.IsExpansion1Active())
            {
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Radiation.id))
                {
                    Radiation.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Radiation).id;
                }
                else
                {
                    AddMissingCustomGameSetting(CustomGameSettingConfigs.Radiation);
                    Radiation.Value = isNoSweat ? CustomGameSettingConfigs.Radiation.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Radiation.GetDefaultLevelId();
                }
            }

            ///Stress
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Stress.id))
            {
                Stress.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Stress);
                Stress.Value = isNoSweat ? CustomGameSettingConfigs.Stress.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Stress.GetDefaultLevelId();
            }

            ///StressBreaks
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.StressBreaks.id))
            {
                StressBreaks.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.StressBreaks).id == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.StressBreaks);
                StressBreaks.On = isNoSweat
                    ? CustomGameSettingConfigs.StressBreaks.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.StressBreaks.GetDefaultLevelId() == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id;
            }

            ///Sandbox
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.SandboxMode.id))
            {
                SandboxMode.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SandboxMode).id == (CustomGameSettingConfigs.SandboxMode as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.SandboxMode);
                SandboxMode.On = isNoSweat
                    ? CustomGameSettingConfigs.SandboxMode.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.SandboxMode as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.SandboxMode.GetDefaultLevelId() == (CustomGameSettingConfigs.SandboxMode as ToggleSettingConfig).on_level.id;
            }

            ///CarePackages
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
            {
                CarePackages.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CarePackages).id == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.CarePackages);
                CarePackages.On = isNoSweat
                    ? CustomGameSettingConfigs.CarePackages.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.CarePackages.GetDefaultLevelId() == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id;
            }

            ///Fast Workers
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.FastWorkersMode.id))
            {
                FastWorkersMode.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.FastWorkersMode).id == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.FastWorkersMode);
                FastWorkersMode.On = isNoSweat
                    ? CustomGameSettingConfigs.FastWorkersMode.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.FastWorkersMode.GetDefaultLevelId() == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id;
            }

            ///Save to Cloud
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.SaveToCloud.id))
            {
                SaveToCloud.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id == (CustomGameSettingConfigs.SaveToCloud as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.SaveToCloud);
                SaveToCloud.On = isNoSweat
                    ? CustomGameSettingConfigs.SaveToCloud.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.SaveToCloud as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.SaveToCloud.GetDefaultLevelId() == (CustomGameSettingConfigs.SaveToCloud as ToggleSettingConfig).on_level.id;
            }

            ///Teleporters
            ///
            if (DlcManager.IsExpansion1Active())
            {
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Teleporters.id))
                {
                    Teleporters.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Teleporters).id == (CustomGameSettingConfigs.Teleporters as ToggleSettingConfig).on_level.id;
                }
                else
                {
                    AddMissingCustomGameSetting(CustomGameSettingConfigs.Teleporters);
                    Teleporters.On = isNoSweat
                        ? CustomGameSettingConfigs.Teleporters.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.Teleporters as ToggleSettingConfig).on_level.id
                        : CustomGameSettingConfigs.Teleporters.GetDefaultLevelId() == (CustomGameSettingConfigs.Teleporters as ToggleSettingConfig).on_level.id;
                }
            }

        }

        private void GetNewRandomSeed() => this.SetSeed(UnityEngine.Random.Range(0, int.MaxValue).ToString());
        void SetSeed(string seedString)
        {
            string ExistingSeedString = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id;
            if (ExistingSeedString == seedString)
            {
                SgtLogger.l("new seed was the same as old seed, skipping setter");
                return;
            }

            int seed = int.Parse(seedString);
            seed = Mathf.Min(seed, int.MaxValue);
            SeedInput_Main.Text = seedString;

            CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.WorldgenSeed, seed.ToString());
            RefreshView();
        }

        #endregion

        bool _lastCategoryWasStarmapItem = false;

        public void RefreshDetails()
        {
            SeedInput_Main.Text = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id;
            SeedRerollsTraitsToggle_Main.On = CGSMClusterManager.RerollTraitsWithSeedChange;

            bool CategoryIsStarmapitem = SelectedCategory > 0;
            if(CategoryIsStarmapitem != _lastCategoryWasStarmapItem)
            {
                _lastCategoryWasStarmapItem = CategoryIsStarmapitem;
                StarmapItemEnabled.gameObject.SetActive(CategoryIsStarmapitem);
                NumberToGenerate.transform.parent.gameObject.SetActive(CategoryIsStarmapitem);
                MinMaxDistanceSlider.transform.parent.gameObject.SetActive(CategoryIsStarmapitem);
                BufferDistance.transform.parent.gameObject.SetActive(CategoryIsStarmapitem);
                AsteroidSize.SetActive(CategoryIsStarmapitem);
                MeteorSelector.SetActive(CategoryIsStarmapitem);
                AsteroidTraits.SetActive(CategoryIsStarmapitem);
                PlanetBiomesGO.SetActive(CategoryIsStarmapitem);
                ActiveBiomesContainer.SetActive(CategoryIsStarmapitem);
            }
            Details_StoryTraitContainer.SetActive(SelectedCategory == StarmapItemCategory.StoryTraits);
            Details_VanillaPOIContainer.SetActive(SelectedCategory == StarmapItemCategory.VanillaStarmap);

            if (CategoryIsStarmapitem && SelectedPlanet != null)
            {
                bool IsPartOfCluster = CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current);
                bool isRandom = current.id.Contains(CGSMClusterManager.RandomKey);
                bool canGenerateMultiple = current.MaxNumberOfInstances > 1;

                selectionHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, SelectedPlanet.DisplayName), SelectedCategory));
                
                StarmapItemEnabledText.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STARMAPITEMENABLED.LABEL, SelectedCategory));
                StarmapItemEnabled.SetOn(IsPartOfCluster);

                NumberToGenerate.transform.parent.gameObject.SetActive(canGenerateMultiple);///Amount, only on poi / random planets
                if (canGenerateMultiple)
                {
                    NumberToGenerate.SetWholeNumbers(!current.IsPOI || overrideToWholeNumbers);
                    NumberToGenerate.SetMinMaxCurrent(0, current.MaxNumberOfInstances, current.InstancesToSpawn);
                    NumberToGenerate.SetInteractable(IsPartOfCluster);
                    current.SetSpawnNumber(NumberToGenerate.Value);
                }

                if (RandomOuterPlanetsStarmapItem != null)
                {
                    NumberOfRandomClassicsGO.SetActive(current.IsRandom && !current.IsPOI);
                    NumberOfRandomClassics.SetMinMaxCurrent(0, CGSMClusterManager.RandomOuterPlanetsStarmapItem.MaxNumberOfInstances, CGSMClusterManager.MaxClassicOuterPlanets);
                    NumberOfRandomClassics.SetInteractable(IsPartOfCluster);
                }

                MinMaxDistanceSlider.SetLimits(0, CustomCluster.Rings);
                MinMaxDistanceSlider.SetValues(current.minRing, current.maxRing, 0, CustomCluster.Rings, true);
                MinMaxDistanceSlider.SetInteractable(IsPartOfCluster);
                SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)current.minRing, (int)current.maxRing));

                BufferDistance.SetMinMaxCurrent(0, CustomCluster.Rings, SelectedPlanet.buffer);
                BufferDistance.transform.parent.gameObject.SetActive(!current.IsPOI);
                BufferDistance.SetInteractable(IsPartOfCluster);

                ClusterSize.SetMinMaxCurrent(ringMin, ringMax, CustomCluster.Rings);

                RandomTraitDeleteButton.SetInteractable(!isRandom);
                AddTraitButton.SetInteractable(IsPartOfCluster && !isRandom);
                AddSeasonButton.SetInteractable(IsPartOfCluster && !isRandom);

                AsteroidSize.SetActive(!current.IsPOI && !isRandom);
                MeteorSelector.SetActive(!current.IsPOI && !isRandom);
                AsteroidTraits.SetActive(!current.IsPOI);
                PlanetBiomesGO.SetActive(!current.IsPOI && !isRandom);
                ActiveBiomesContainer.SetActive(!current.IsPOI && !isRandom);

                UpdateSizeLabels(current);
                PlanetSizeCycle.Value = current.CurrentSizePreset.ToString();
                PlanetRazioCycle.Value = current.CurrentRatioPreset.ToString();

                if (current.IsPOI) return;
                RefreshMeteorLists();
                RefreshTraitList();
                RefreshPlanetBiomes();
            }
        }

        public void SelectItem(StarmapItem planet)
        {
            if (planet != SelectedPlanet)
            {
                overrideToWholeNumbers = false;
            }

            SelectedPlanet = planet;
            this.RefreshView();
        }

        private void RefreshGallery()
        {

            CustomGameSettingsContent.SetActive(SelectedCategory == StarmapItemCategory.GameSettings);
            VanillaStarmapItemContent.SetActive(SelectedCategory == StarmapItemCategory.VanillaStarmap);
            StoryTraitGridContent.SetActive(SelectedCategory == StarmapItemCategory.StoryTraits);
            ClusterItemsContent.SetActive(SelectedCategory > 0);

            if (SelectedCategory > 0)
            {
                var activePlanets = CGSMClusterManager.GetActivePlanetsStarmapitems();

                foreach (var galleryGridButton in planetoidGridButtons)
                {
                    var logicComponent = galleryGridButton.Value;
                    logicComponent.Refresh(galleryGridButton.Key, false, false);
                    galleryGridButton.Value.gameObject.SetActive(galleryGridButton.Key.category == this.SelectedCategory);
                }
                foreach (var activePlanet in activePlanets)
                {
                    bool selected = SelectedPlanet == null ? false : SelectedPlanet == activePlanet;
                    planetoidGridButtons[activePlanet].Refresh(activePlanet, true, selected);
                    planetoidGridButtons[activePlanet].gameObject.SetActive(activePlanet.category == this.SelectedCategory);
                }
                this.galleryHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION.HEADER.LABEL, SelectedCategory));
            }
            else if (SelectedCategory == StarmapItemCategory.GameSettings)
            {
                this.galleryHeaderLabel.SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.CUSTOMIZE);
                LoadGameSettings();
            }
            else if (SelectedCategory == StarmapItemCategory.StoryTraits)
            {
                this.galleryHeaderLabel.SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER);
            }
            else if (SelectedCategory == StarmapItemCategory.VanillaStarmap)
            {
                this.galleryHeaderLabel.SetText(global::STRINGS.UI.CLUSTERMAP.TITLE);
            }

        }


        public void SelectCategory(StarmapItemCategory category)
        {
            this.SelectedCategory = category;
            //this.categoryHeaderLabel.SetText(STRINGS.UI.CUSTOMCLUSTERUI.NAMECATEGORIES);
            this.SelectDefaultCategoryItem();
            this.RefreshView();
        }
        private void SelectDefaultCategoryItem()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory && CustomCluster.HasStarmapItem(galleryGridButton.Key.id, out var i))
                {
                    this.SelectItem(galleryGridButton.Key);
                    return;
                }
            }
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory)
                {
                    this.SelectItem(galleryGridButton.Key);
                    return;
                }
            }
            this.SelectItem(null);
        }

        #region initialisation

        public void Init()
        {
            if (init) return;

            this.InitializeVanillaStarmapInfo();
            this.InitializeVanillaStarmap();
            this.InitializeStoryTraits();
            this.InitializeGameSettings();
            this.PopulateGalleryAndCategories();
            this.InitializeItemSettings();
            init = true;
        }

        public void InitializeItemSettings()
        {
            //UIUtils.ListAllChildrenPath(transform);
            MinMaxDistanceSlider = transform.Find("Details/Content/ScrollRectContainer/MinMaxDistance/Slider").FindOrAddComponent<UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider>();
            MinMaxDistanceSlider.SliderBounds = MinMaxDistanceSlider.transform.Find("Handle Slide Area").rectTransform();
            MinMaxDistanceSlider.MinHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/HandleMin").rectTransform();
            MinMaxDistanceSlider.MaxHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/Handle").rectTransform();
            MinMaxDistanceSlider.MiddleGraphic = MinMaxDistanceSlider.transform.Find("Fill Area/Fill").rectTransform();
            MinMaxDistanceSlider.wholeNumbers = true;
            MinMaxDistanceSlider.onValueChanged.AddListener(
                (min, max) =>
                {
                    if (SelectedPlanet != null && CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        current.SetInnerRing(Mathf.RoundToInt(min));
                        current.SetOuterRing(Mathf.RoundToInt(max));
                        SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)min, (int)max));
                    }
                }
                );

            //MinMaxDistanceSlider.SetLimits(0, CustomCluster.Rings);
            //MinMaxDistanceSlider.SetValues(0, 0.001f, 0, CustomCluster.Rings, true);

            SpawnDistanceText = MinMaxDistanceSlider.transform.parent.Find("Descriptor/Output").GetComponent<LocText>();
            UIUtils.AddSimpleTooltipToObject(MinMaxDistanceSlider.transform.parent.Find("Descriptor"), (MINMAXDISTANCE.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);

            StarmapItemEnabledText = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled/Label").GetComponent<LocText>();
            StarmapItemEnabled = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled").FindOrAddComponent<FToggle2>();
            StarmapItemEnabled.SetCheckmark("Background/Checkmark");
            StarmapItemEnabled.OnClick += () =>
            {
                if (SelectedPlanet != null)
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    this.RefreshCategories();
                    this.RefreshGallery();
                    this.RefreshDetails();
                }
            };
            UIUtils.AddSimpleTooltipToObject(StarmapItemEnabled.transform, STARMAPITEMENABLED.TOOLTIP, onBottom: true, alignCenter: true);


            NumberToGenerate = transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Slider").FindOrAddComponent<FSlider>();

            NumberToGenerate.SetWholeNumbers(true);
            NumberToGenerate.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Descriptor/Output").GetComponent<LocText>());
            NumberToGenerate.OnChange += (value) =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetSpawnNumber(value);
                    this.RefreshGallery();

                    if (SelectedPlanet == CGSMClusterManager.RandomOuterPlanetsStarmapItem)
                    {
                        MaxClassicOuterPlanets = Mathf.Min(MaxClassicOuterPlanets, Mathf.RoundToInt(SelectedPlanet.InstancesToSpawn) + 2);
                    }

                    this.RefreshDetails();
                }
            };
            UIUtils.AddSimpleTooltipToObject(NumberToGenerate.transform.parent.Find("Descriptor"), (AMOUNTSLIDER.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);
            NumberOfRandomClassicsGO = transform.Find("Details/Content/ScrollRectContainer/AmountOfClassicPlanets").gameObject;

            NumberOfRandomClassicsGO.SetActive(true);
            NumberOfRandomClassics = NumberOfRandomClassicsGO.transform.Find("Slider").FindOrAddComponent<FSlider>();
            NumberOfRandomClassicsGO.SetActive(false);

            NumberOfRandomClassics.SetWholeNumbers(true);
            NumberOfRandomClassics.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/AmountOfClassicPlanets/Descriptor/Output").GetComponent<LocText>());
            NumberOfRandomClassics.OnChange += (value) =>
            {
                MaxClassicOuterPlanets = Mathf.RoundToInt(value);
            };
            if (CGSMClusterManager.RandomOuterPlanetsStarmapItem != null)
                NumberOfRandomClassics.SetMinMaxCurrent(0, CGSMClusterManager.RandomOuterPlanetsStarmapItem.InstancesToSpawn, CGSMClusterManager.RandomOuterPlanetsStarmapItem.InstancesToSpawn);

            UIUtils.AddSimpleTooltipToObject(NumberOfRandomClassics.transform.parent.Find("Descriptor"), (AMOUNTOFCLASSICPLANETS.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);

            BufferDistance = transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Slider").FindOrAddComponent<FSlider>();
            BufferDistance.SetWholeNumbers(true);
            BufferDistance.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Descriptor/Output").GetComponent<LocText>());
            BufferDistance.OnChange += (value) =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetBuffer((int)value);
                }
            };
            UIUtils.AddSimpleTooltipToObject(BufferDistance.transform.parent.Find("Descriptor"), BUFFERSLIDER.DESCRIPTOR.TOOLTIP);

            ClusterSize = transform.Find("Details/Footer/ClusterSizeSlider/Slider").FindOrAddComponent<FSlider>();
            ClusterSize.SetWholeNumbers(true);
            ClusterSize.AttachOutputField(transform.Find("Details/Footer/ClusterSizeSlider/Descriptor/Output").GetComponent<LocText>());
            ClusterSize.OnChange += (value) =>
            {
                CustomCluster.SetRings((int)value);
                this.RefreshGallery();
                this.RefreshDetails();
            };
            UIUtils.AddSimpleTooltipToObject(ClusterSize.transform.parent.Find("Descriptor"), CLUSTERSIZESLIDER.DESCRIPTOR.TOOLTIP);


            AsteroidSize = transform.Find("Details/Content/ScrollRectContainer/AsteroidSize").gameObject;
            AsteroidSizeLabel = AsteroidSize.transform.Find("Descriptor/Label").GetComponent<LocText>();
            AsteroidSizeTooltip = UIUtils.AddSimpleTooltipToObject(AsteroidSizeLabel.transform.parent, ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);

            PlanetSizeWidth = AsteroidSize.transform.Find("Content/Info/WidthLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeWidth.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, false));

            PlanetSizeHeight = AsteroidSize.transform.Find("Content/Info/HeightLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeHeight.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, true));

            PlanetSizeCycle = AsteroidSize.transform.Find("Content/Cycles/SizeCycle").gameObject.AddOrGet<FCycle>();
            PlanetSizeCycle.Initialize(
                PlanetSizeCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetSizeCycle.Options = new List<FCycle.Option>()
            {
                new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3TOOLTIP),

                new FCycle.Option(WorldSizePresets.Normal.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE0, ASTEROIDSIZE.SIZESELECTOR.SIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlyLarger.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE1, ASTEROIDSIZE.SIZESELECTOR.SIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Large.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE2, ASTEROIDSIZE.SIZESELECTOR.SIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.Huge.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE3, ASTEROIDSIZE.SIZESELECTOR.SIZE3TOOLTIP),
                new FCycle.Option(WorldSizePresets.Massive.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE4, ASTEROIDSIZE.SIZESELECTOR.SIZE4TOOLTIP),
                new FCycle.Option(WorldSizePresets.Enormous.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE5, ASTEROIDSIZE.SIZESELECTOR.SIZE5TOOLTIP),
            };

            PlanetSizeCycle.OnChange += () =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        WorldSizePresets setTo = Enum.TryParse<WorldSizePresets>(PlanetSizeCycle.Value, out var result) ? result : WorldSizePresets.Normal;
                        current.SetPlanetSizeToPreset(setTo);
                        UpdateSizeLabels(current);
                    }
                }
            };

            PlanetRazioCycle = AsteroidSize.transform.Find("Content/Cycles/RazioCycle").gameObject.AddOrGet<FCycle>();
            PlanetRazioCycle.Initialize(
                PlanetRazioCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetRazioCycle.Options = new List<FCycle.Option>()
            {
                new FCycle.Option(WorldRatioPresets.LotWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE3, ASTEROIDSIZE.RATIOSELECTOR.WIDE3TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Wider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE2, ASTEROIDSIZE.RATIOSELECTOR.WIDE2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE1, ASTEROIDSIZE.RATIOSELECTOR.WIDE1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Normal.ToString(), ASTEROIDSIZE.RATIOSELECTOR.NORMAL, ASTEROIDSIZE.RATIOSELECTOR.NORMALTOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Taller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.LotTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3TOOLTIP),
            };
            PlanetRazioCycle.Value = WorldRatioPresets.Normal.ToString();

            PlanetRazioCycle.OnChange += () =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        WorldRatioPresets setTo = Enum.TryParse<WorldRatioPresets>(PlanetRazioCycle.Value, out var result) ? result : WorldRatioPresets.Normal;
                        current.SetPlanetRatioToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            MeteorSelector = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle").gameObject;
            ActiveMeteorsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content").gameObject;
            MeteorPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content/ListViewEntryPrefab").gameObject;

            ActiveSeasonsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content").gameObject;
            SeasonPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/ListViewEntryPrefab").gameObject;

            ActiveBiomesContainer = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Content/TraitContainer/ScrollArea/Content").gameObject;
            BiomePrefab = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Content/TraitContainer/ScrollArea/Content/Item").gameObject;
            AddSeasonButton = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/AddSeasonButton").FindOrAddComponent<FButton>();
            UIUtils.AddSimpleTooltipToObject(AddSeasonButton.transform, METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);
            AddSeasonButton.OnClick += () =>
            {
                SeasonSelectorScreen.InitializeView(SelectedPlanet, () => RefreshMeteorLists());
            };

            UIUtils.AddSimpleTooltipToObject(MeteorSelector.transform.Find("Descriptor"), METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);


            AsteroidTraits = transform.Find("Details/Content/ScrollRectContainer/AsteroidTraits").gameObject;
            ActiveTraitsContainer = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content").gameObject;
            TraitPrefab = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;
            TraitPrefab.SetActive(false);

            AddTraitButton = AsteroidTraits.transform.Find("Content/AddSeasonButton").FindOrAddComponent<FButton>();

            AddTraitButton.OnClick += () =>
            {
                TraitSelectorScreen.InitializeView(SelectedPlanet, () => RefreshTraitList());
            };

            var buttons = transform.Find("Details/Footer/Buttons");

            ReturnButton = buttons.Find("ReturnButton").FindOrAddComponent<FButton>();
            ReturnButton.OnClick += () => Show(false);

            GenerateClusterButton = buttons.Find("GenerateClusterButton").FindOrAddComponent<FButton>();
            GenerateClusterButton.OnClick += () => CGSMClusterManager.InitializeGeneration();

            ResetButton = buttons.Find("ResetSelectionButton").FindOrAddComponent<FButton>();
            ResetButton.OnClick += () =>
            {
                CGSMClusterManager.ResetPlanetFromPreset(SelectedPlanet.id);
                RefreshView();
            };

            ResetAllButton = buttons.Find("ResetClusterButton").FindOrAddComponent<FButton>();
            ResetAllButton.OnClick += () =>
            {
                CGSMClusterManager.ResetToLastPreset();
                RefreshView();
            };

            PresetsButton = buttons.Find("PresetButton").FindOrAddComponent<FButton>();
            PresetsButton.OnClick += () =>
            {
                CGSMClusterManager.OpenPresetWindow(() => RefreshView());
            };

            //SettingsButton = buttons.Find("SettingsButton").FindOrAddComponent<FButton>();
            //SettingsButton.OnClick += () =>
            //{
            //    CustomSettingsController.ShowWindow(() => RefreshView());
            //};

            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, RESETCLUSTERBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(ResetButton.transform, RESETSELECTIONBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(GenerateClusterButton.transform, GENERATECLUSTERBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(ReturnButton.transform, RETURNBUTTON.TOOLTIP, true, onBottom: true);
            //UIUtils.AddSimpleTooltipToObject(SettingsButton.transform, SETTINGSBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(PresetsButton.transform, PRESETBUTTON.TOOLTIP, true, onBottom: true);

            PlanetBiomesGO = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes").gameObject;

            SeedInput_Main = transform.Find("Details/Footer/Seed/SeedBar/Input").FindOrAddComponent<FInputField2>();
            SeedInput_Main.inputField.onEndEdit.AddListener(SetSeed);

            SeedCycleButton_Main = transform.Find("Details/Footer/Seed/SeedBar/DeleteButton").FindOrAddComponent<FButton>();
            SeedCycleButton_Main.OnClick += () => GetNewRandomSeed();

            var SeedLabel = transform.Find("Details/Footer/Seed/Label").gameObject.AddOrGet<LocText>();
            SeedLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.NAME;
            UIUtils.AddSimpleTooltipToObject(SeedLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.TOOLTIP, alignCenter: true, onBottom: true);


            SeedRerollsTraitsToggle_Main = transform.Find("Details/Footer/Seed/SeedAfffectingTraits").FindOrAddComponent<FToggle2>();
            SeedRerollsTraitsToggle_Main.SetCheckmark("Background/Checkmark");
            SeedRerollsTraitsToggle_Main.On = CGSMClusterManager.RerollTraitsWithSeedChange;
            SeedRerollsTraitsToggle_Main.OnClick += () =>
            {
                CGSMClusterManager.RerollTraitsWithSeedChange = SeedRerollsTraitsToggle_Main.On;
            };

            var seedRerollLabel = transform.Find("Details/Footer/Seed/SeedAfffectingTraits/Label").gameObject.AddOrGet<LocText>();
            seedRerollLabel.text = STRINGS.UI.SEEDLOCK.NAME_SHORT;
            UIUtils.AddSimpleTooltipToObject(seedRerollLabel.transform, STRINGS.UI.SEEDLOCK.TOOLTIP, alignCenter: true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Descriptor/infoIcon"), STRINGS.UI.INFOTOOLTIPS.INFO_ONLY, alignCenter: true, onBottom: true);


            InitializeTraitContainer();
            InitializeMeteorShowerContainers();
            //UIUtils.AddSimpleTooltipToObject(SeedLabel.transform, global::STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES, alignCenter: true, onBottom: true);
            InitializePlanetBiomesContainers();
        }

        public void InitializeVanillaStarmapInfo()
        {

            Details_VanillaPOIContainer = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources").gameObject;// .FindOrAddComponent<FToggle2>();
            Details_VanillaPOIContainer.SetActive(true);
            VanillaPOIResourceContainer = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Content/ResourceContainer/ScrollArea/Content").gameObject;
            VanillaPOIResourcePrefab = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Content/ResourceContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;
            VanillaPOI_SizeAmountDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Size/Amount").FindOrAddComponent<LocText>();
            VanillaPOI_ReplenishmentAmountDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Replenisment/Amount").FindOrAddComponent<LocText>();
            VanillaPOI_ArtifactDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/VanillaPOI_Artifact/Amount").FindOrAddComponent<LocText>();
            VanillaPOI_RemovePOIBtn = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/VanillaPOI_Remove/DeletePOI").FindOrAddComponent<FButton>();
            Details_VanillaPOIContainer.SetActive(false);
        }
        public void InitializeStoryTraits()
        {

            StoryTraitGridContainer = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer").gameObject;
            StoryTraitEntryPrefab = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer/Item").gameObject;

            Details_StoryTraitContainer = transform.Find("Details/Content/ScrollRectContainer/StoryTrait").gameObject;// .FindOrAddComponent<FToggle2>();
            Details_StoryTraitContainer.SetActive(true);
            StoryTraitImage = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/HeaderImage").FindOrAddComponent<Image>();
            StoryTraitDesc = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/Description").FindOrAddComponent<LocText>();
            StoryTraitToggle = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/StoryTraitEnabled").FindOrAddComponent<FToggle2>();
            Details_StoryTraitContainer.SetActive(false);
        }
        public void InitializeVanillaStarmap()
        {
            VanillaStarmapItemContainer = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer").gameObject;
            VanillaStarmapItemPrefab = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/VanillaStarmapEntryPrefab").gameObject;
        }


        private void SetCustomGameSettings(SettingConfig ConfigToSet, object valueId)
        {
            string valueToSet = valueId.ToString();
            if (valueId is bool)
            {
                var toggle = ConfigToSet as ToggleSettingConfig;
                valueToSet = ((bool)valueId) ? toggle.on_level.id : toggle.off_level.id;
            }
            SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentQualitySetting(ConfigToSet).id + " to " + valueToSet.ToString());
            CustomGameSettings.Instance.SetQualitySetting(ConfigToSet, valueToSet);
        }

        public void InitializeGameSettings()
        {
            var transform = CustomGameSettingsContainer.transform;
            transform.gameObject.SetActive(true);
            GameObject SwitchPrefab = CustomGameSettingsContainer.transform.Find("SwitchPrefab").gameObject;
            GameObject TogglePrefab = CustomGameSettingsContainer.transform.Find("TogglePrefab").gameObject;

            TogglePrefab.SetActive(false);
            SwitchPrefab.SetActive(false);
            // UIUtils.AddSimpleTooltipToObject(transform.Find("Content/Warning"), STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.CHANGEWARNINGTOOLTIP);

            StressBreaks = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();

            var StressBreaksLabel = StressBreaks.transform.Find("Label").gameObject.AddOrGet<LocText>();
            StressBreaksLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.NAME;
            UIUtils.AddSimpleTooltipToObject(StressBreaksLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.TOOLTIP, alignCenter: true, onBottom: true);

            StressBreaks.SetCheckmark("Background/Checkmark");
            StressBreaks.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.StressBreaks, StressBreaks.On);
            };

            CarePackages = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();

            var CarePackagesLabel = CarePackages.transform.Find("Label").gameObject.AddOrGet<LocText>();
            CarePackagesLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.NAME;
            UIUtils.AddSimpleTooltipToObject(CarePackagesLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.TOOLTIP, alignCenter: true, onBottom: true);

            CarePackages.SetCheckmark("Background/Checkmark");
            CarePackages.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.CarePackages, CarePackages.On);
            };


            SandboxMode = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();

            var SandboxModeLabel = SandboxMode.transform.Find("Label").gameObject.AddOrGet<LocText>();
            SandboxModeLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.NAME;
            UIUtils.AddSimpleTooltipToObject(SandboxModeLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.TOOLTIP, alignCenter: true, onBottom: true);

            SandboxMode.SetCheckmark("Background/Checkmark");
            SandboxMode.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.SandboxMode, SandboxMode.On);
            };



            FastWorkersMode = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();

            var FastWorkersModeLabel = FastWorkersMode.transform.Find("Label").gameObject.AddOrGet<LocText>();
            FastWorkersModeLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.NAME;
            UIUtils.AddSimpleTooltipToObject(FastWorkersModeLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.TOOLTIP, alignCenter: true, onBottom: true);

            FastWorkersMode.SetCheckmark("Background/Checkmark");
            FastWorkersMode.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.FastWorkersMode, FastWorkersMode.On);
            };

            SaveToCloud = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();

            var SaveToCloudLabel = SaveToCloud.transform.Find("Label").gameObject.AddOrGet<LocText>();
            SaveToCloudLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.NAME;
            UIUtils.AddSimpleTooltipToObject(SaveToCloudLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP, alignCenter: true, onBottom: true);

            SaveToCloud.SetCheckmark("Background/Checkmark");
            SaveToCloud.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.SaveToCloud, SaveToCloud.On);
            };


            Teleporters = Util.KInstantiateUI(TogglePrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FToggle2>();
            if (DlcManager.IsExpansion1Active())
            {
                var TeleportersLabel = Teleporters.transform.Find("Label").gameObject.AddOrGet<LocText>();
                TeleportersLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.TELEPORTERS.NAME;
                UIUtils.AddSimpleTooltipToObject(TeleportersLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.TELEPORTERS.TOOLTIP, alignCenter: true, onBottom: true);

                Teleporters.SetCheckmark("Background/Checkmark");
                Teleporters.OnClick += () =>
                {
                    SetCustomGameSettings(CustomGameSettingConfigs.Teleporters, Teleporters.On);
                };
            }
            else
            {
                Teleporters.gameObject.SetActive(false);
            }

            ///Immune System Strength
            ImmuneSystem = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).transform.gameObject.AddOrGet<FCycle>();
            var ImmuneLabel = ImmuneSystem.transform.Find("Label").gameObject.AddOrGet<LocText>();
            ImmuneLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.NAME;
            UIUtils.AddSimpleTooltipToObject(ImmuneLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.TOOLTIP, alignCenter: true, onBottom: true);
            ImmuneSystem.Initialize(
                ImmuneSystem.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                ImmuneSystem.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                ImmuneSystem.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                ImmuneSystem.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            ImmuneSystem.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.ImmuneSystem.GetLevels())
            {
                ImmuneSystem.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            ImmuneSystem.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.ImmuneSystem, ImmuneSystem.Value);
            };

            ///Calorie Usage
            CalorieBurn = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();

            var CalorieBurnLabel = CalorieBurn.transform.Find("Label").gameObject.AddOrGet<LocText>();
            CalorieBurnLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.NAME;
            UIUtils.AddSimpleTooltipToObject(CalorieBurnLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.TOOLTIP, alignCenter: true, onBottom: true);

            CalorieBurn.Initialize(
                CalorieBurn.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                CalorieBurn.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                CalorieBurn.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                CalorieBurn.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            CalorieBurn.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.CalorieBurn.GetLevels())
            {
                CalorieBurn.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            CalorieBurn.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.CalorieBurn, CalorieBurn.Value);
            };

            ///Morale Requirements
            Morale = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();

            var MoraleLabel = Morale.transform.Find("Label").gameObject.AddOrGet<LocText>();
            MoraleLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.NAME;
            UIUtils.AddSimpleTooltipToObject(MoraleLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.TOOLTIP, alignCenter: true, onBottom: true);

            Morale.Initialize(
                Morale.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Morale.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Morale.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Morale.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Morale.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Morale.GetLevels())
            {
                Morale.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Morale.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Morale, Morale.Value);
            };

            ///Suit Durability Settings
            Durability = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();

            var DurabilityLabel = Durability.transform.Find("Label").gameObject.AddOrGet<LocText>();
            DurabilityLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.DURABILITY.NAME;
            UIUtils.AddSimpleTooltipToObject(DurabilityLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.DURABILITY.TOOLTIP, alignCenter: true, onBottom: true);

            Durability.Initialize(
                Durability.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Durability.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Durability.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Durability.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Durability.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Durability.GetLevels())
            {
                Durability.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Durability.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Durability, Durability.Value);
            };

            ///Meteors
            MeteorShowers = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();

            var MeteorLabel = MeteorShowers.transform.Find("Label").gameObject.AddOrGet<LocText>();
            MeteorLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.METEORSHOWERS.NAME;
            UIUtils.AddSimpleTooltipToObject(MeteorLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.METEORSHOWERS.TOOLTIP, alignCenter: true, onBottom: true);
            MeteorShowers.Initialize(
                MeteorShowers.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                MeteorShowers.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                MeteorShowers.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                MeteorShowers.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            MeteorShowers.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.MeteorShowers.GetLevels())
            {
                MeteorShowers.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            MeteorShowers.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.MeteorShowers, MeteorShowers.Value);
            };

            ///Radiation
            Radiation = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();
            if (DlcManager.IsExpansion1Active())
            {
                Radiation.Initialize(
                    Radiation.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                    Radiation.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                    Radiation.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                    Radiation.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());


                var RadiationLabel = Radiation.transform.Find("Label").gameObject.AddOrGet<LocText>();
                RadiationLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.RADIATION.NAME;
                UIUtils.AddSimpleTooltipToObject(RadiationLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.RADIATION.TOOLTIP, alignCenter: true, onBottom: true);

                Radiation.Options = new List<FCycle.Option>();
                foreach (var config in CustomGameSettingConfigs.Radiation.GetLevels())
                {
                    Radiation.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
                }
                Radiation.OnChange += () =>
                {
                    SetCustomGameSettings(CustomGameSettingConfigs.Radiation, Radiation.Value);
                };
            }
            else
            {
                Radiation.gameObject.SetActive(false);
            }

            ///Stress
            Stress = Util.KInstantiateUI(SwitchPrefab, CustomGameSettingsContainer, true).gameObject.AddOrGet<FCycle>();

            var STRESSLabel = Stress.transform.Find("Label").gameObject.AddOrGet<LocText>();
            STRESSLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.NAME;
            UIUtils.AddSimpleTooltipToObject(STRESSLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.TOOLTIP, alignCenter: true, onBottom: true);

            Stress.Initialize(
                Stress.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Stress.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Stress.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Stress.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Stress.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Stress.GetLevels())
            {
                Stress.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Stress.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Stress, Stress.Value);
            };

            this.ImmuneSystem.transform.SetAsLastSibling();
            this.CalorieBurn.transform.SetAsLastSibling();
            this.Morale.transform.SetAsLastSibling();
            this.Durability.transform.SetAsLastSibling();
            this.MeteorShowers.transform.SetAsLastSibling();
            this.Radiation.transform.SetAsLastSibling();
            this.Stress.transform.SetAsLastSibling();
            this.StressBreaks.transform.SetAsLastSibling();
            this.CarePackages.transform.SetAsLastSibling();
            this.SandboxMode.transform.SetAsLastSibling();
            this.FastWorkersMode.transform.SetAsLastSibling();
            this.SaveToCloud.transform.SetAsLastSibling();
            this.Teleporters.transform.SetAsLastSibling();

        }
        public void PopulateGalleryAndCategories()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
                UnityEngine.Object.Destroy(galleryGridButton.Value.gameObject);
            planetoidGridButtons.Clear();

            foreach (var item in this.categoryToggles)
                UnityEngine.Object.Destroy(item.Value.gameObject);

            categoryToggles.Clear();


            foreach (var Planet in PlanetoidDict())
            {
                this.AddItemToGallery(Planet.Value);
            }
            foreach (StarmapItemCategory category in (StarmapItemCategory[])Enum.GetValues(typeof(StarmapItemCategory)))
            {
                if (category < 0)
                    continue;

                AddCategoryItem(category);
            };
            var StoryTraitsBtn = StoryTraitButton.AddOrGet<CategoryItem>();
            StoryTraitsBtn.Initialize(StarmapItemCategory.StoryTraits, null);
            StoryTraitsBtn.ActiveToggle.OnClick += (() => this.SelectCategory(StarmapItemCategory.StoryTraits));
            this.categoryToggles.Add(StarmapItemCategory.StoryTraits, StoryTraitsBtn);

            var GameSettingsBtn = GameSettingsButton.AddOrGet<CategoryItem>();
            GameSettingsBtn.Initialize(StarmapItemCategory.GameSettings, null);
            GameSettingsBtn.ActiveToggle.OnClick += (() => this.SelectCategory(StarmapItemCategory.GameSettings));
            this.categoryToggles.Add(StarmapItemCategory.GameSettings, GameSettingsBtn);

            //if (galleryGridLayouter != null)
            //    this.galleryGridLayouter.RequestGridResize();


            CustomGameSettingsContainer.SetActive(true);
            VanillaStarmapItemContent.SetActive(true);
            StoryTraitGridContent.SetActive(true);
            ClusterItemsContent.SetActive(true);
        }
        void InitializePlanetBiomesContainers()
        {
            UIUtils.TryChangeText(transform, "Details/Content/ScrollRectContainer/AsteroidBiomes/Descriptor/Label", global::STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES + ":");


            Regex rx = new Regex(@"subworlds[\\\/](.*)[\\\/]");
            foreach (var biomeTypePath in SettingsCache.subworlds.Keys)
            {
                var match = rx.Match(biomeTypePath);
                string biomeName = string.Empty;
                if (!match.Success || match.Groups.Count < 2)
                {
                    continue;
                }
                biomeName = match.Groups[1].ToString();
                if (!PlanetBiomes.ContainsKey(biomeName))
                {
                    var biomeHolder = Util.KInstantiateUI(BiomePrefab, ActiveBiomesContainer, true);
                    //SgtLogger.l(biomeName, "BIOMETYPE");

                    string name = Strings.Get("STRINGS.SUBWORLDS." + biomeName.ToUpperInvariant() + ".NAME");
                    string description = Strings.Get("STRINGS.SUBWORLDS." + biomeName.ToUpperInvariant() + ".DESC");

                    Sprite biomeSprite = GameUtil.GetBiomeSprite(biomeName);
                    var icon = biomeHolder.transform.Find("Image").GetComponent<Image>();
                    icon.sprite = biomeSprite;

                    UIUtils.AddSimpleTooltipToObject(biomeHolder.transform, description, true, 250, true);
                    var LocTextName = biomeHolder.transform.Find("Label").GetComponent<LocText>();
                    LocTextName.fontSizeMax = 18f;
                    LocTextName.fontSizeMin = LocTextName.fontSize - 7f;
                    LocTextName.enableAutoSizing = true;
                    UIUtils.TryChangeText(biomeHolder.transform, "Label", name);


                    PlanetBiomes[biomeName] = biomeHolder;

                }
            }

            RefreshPlanetBiomes();
        }

        void InitializeMeteorShowerContainers()
        {
            ///SeasonContainer
            foreach (var gameplaySeason in Db.Get().GameplaySeasons.resources)
            {
                if (!(gameplaySeason is MeteorShowerSeason) || gameplaySeason.Id.Contains("Fullerene") || gameplaySeason.Id.Contains("TemporalTear") || gameplaySeason.dlcId != DlcManager.EXPANSION1_ID)
                    continue;
                var meteorSeason = gameplaySeason as MeteorShowerSeason;

                var seasonInstanceHolder = Util.KInstantiateUI(SeasonPrefab, ActiveSeasonsContainer, true);


                string name = meteorSeason.Name.Replace("MeteorShowers", string.Empty);

                string description = meteorSeason.events.Count == 0 ? METEORSEASONCYCLE.CONTENT.SEASONTYPENOMETEORSTOOLTIP : METEORSEASONCYCLE.CONTENT.SEASONTYPETOOLTIP;

                foreach (var meteorShower in meteorSeason.events)
                {
                    description += "\n • ";
                    description += Assets.GetPrefab((meteorShower as MeteorShowerEvent).clusterMapMeteorShowerID).GetProperName();// Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform, description);
                var LocTextName = seasonInstanceHolder.transform.Find("Label").GetComponent<LocText>();
                LocTextName.fontSizeMax = LocTextName.fontSize;
                LocTextName.fontSizeMin = LocTextName.fontSize - 7f;
                LocTextName.enableAutoSizing = true;
                UIUtils.TryChangeText(seasonInstanceHolder.transform, "Label", name);
                UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform.Find("Label"), description);


                var RemoveButton = seasonInstanceHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                var SwitchButton = seasonInstanceHolder.transform.Find("SwitchButton").gameObject.FindOrAddComponent<FButton>();
                UIUtils.AddSimpleTooltipToObject(SwitchButton.transform, METEORSEASONCYCLE.SWITCHTOOTHERSEASONTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(RemoveButton.transform, METEORSEASONCYCLE.REMOVESEASONTOOLTIP);


                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.RemoveMeteorSeason(meteorSeason.Id); //SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
                    }
                    RefreshMeteorLists();
                };
                SwitchButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        SeasonSelectorScreen.InitializeView(SelectedPlanet, () => RefreshMeteorLists(), meteorSeason.Id);
                    }
                };
                SeasonTypes[gameplaySeason.Id] = seasonInstanceHolder;
            }

            ///Shower Container
            foreach (var gameplayEvent in Db.Get().GameplayEvents.resources)
            {
                if (!(gameplayEvent is MeteorShowerEvent) || gameplayEvent.Id.Contains("Fullerene"))
                    continue;
                var meteorEvent = gameplayEvent as MeteorShowerEvent;
                string ClusterEventID = meteorEvent.clusterMapMeteorShowerID;

                ///for those pesky vanilla meteors without starmap entity
                if (ClusterEventID == null || ClusterEventID == string.Empty)
                {
                    continue;
                }

                var ClusterMapShower = Assets.GetPrefab(ClusterEventID);
                var showerInstanceHolder = Util.KInstantiateUI(MeteorPrefab, ActiveMeteorsContainer, true);


                string name = ClusterMapShower.GetProperName();
                string description = METEORSEASONCYCLE.SHOWERTOOLTIP;
                var icon = showerInstanceHolder.transform.Find("TraitImage").GetComponent<Image>();
                icon.sprite = Def.GetUISprite(Assets.GetPrefab(ClusterEventID)).first;

                var meteortypes = meteorEvent.GetMeteorsInfo();
                foreach (var meteor in meteortypes)
                {
                    description += "\n • ";
                    description += Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                UIUtils.TryChangeText(showerInstanceHolder.transform, "Label", name);
                UIUtils.AddSimpleTooltipToObject(showerInstanceHolder.transform, description);


                ShowerTypes[gameplayEvent.Id] = showerInstanceHolder;
            }
            RefreshMeteorLists();
        }
        void InitializeTraitContainer()
        {
            foreach (var kvp in ModAssets.AllTraitsWithRandom)
            {
                var TraitHolder = Util.KInstantiateUI(TraitPrefab, ActiveTraitsContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var RemoveButton = TraitHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);

                var combined = UIUtils.ColorText(name.ToString(), kvp.Value.colorHex);

                var icon = TraitHolder.transform.Find("TraitImage").GetComponent<Image>();

                icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);
                if (kvp.Key == ModAssets.CustomTraitID)
                {
                    //UIUtils.ListAllChildren(TraitHolder.transform);
                    combined = UIUtils.RainbowColorText(name.ToString());
                    TraitHolder.transform.Find("AwailableRandomTraits").gameObject.SetActive(true);

                    RandomTraitBlacklistOpener = TraitHolder.transform.Find("AwailableRandomTraits").FindOrAddComponent<FButton>();
                    RandomTraitBlacklistOpener.gameObject.SetActive(true);
                    UIUtils.AddSimpleTooltipToObject(RandomTraitBlacklistOpener.transform, AWAILABLERANDOMTRAITS.TOOLTIP);
                    RandomTraitBlacklistOpener.OnClick += () => TraitSelectorScreen.InitializeView(null, () => RefreshTraitList(), true);
                }

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);



                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.RemoveWorldTrait(kvp.Value);
                    }
                    RefreshTraitList();
                };

                if (kvp.Key == ModAssets.CustomTraitID)
                {
                    RandomTraitDeleteButton = RemoveButton;
                }

                Traits[kvp.Value.filePath] = TraitHolder;
            }
            RefreshTraitList();
        }

        public void RefreshPlanetBiomes()
        {
            if (SelectedPlanet == null || SelectedPlanet.world == null)
                return;
            foreach (var biomeHolder in PlanetBiomes.Values)
            {
                biomeHolder.SetActive(false);
            }
            Regex rx = new Regex(@"subworlds[\\\/](.*)[\\\/]");
            foreach (var subworld in SelectedPlanet.world.subworldFiles)
            {

                var match = rx.Match(subworld.name);
                string biomeName = string.Empty;
                if (!match.Success || match.Groups.Count < 2)
                {
                    continue;
                }

                biomeName = match.Groups[1].ToString();
                if (PlanetBiomes.ContainsKey(biomeName))
                    PlanetBiomes[biomeName].SetActive(true);
            }
        }


        public void RefreshMeteorLists()
        {
            if (SelectedPlanet == null)
                return;
            foreach (var showerHolder in ShowerTypes.Values)
            {
                showerHolder.SetActive(false);
            }
            foreach (var activeShower in SelectedPlanet.CurrentMeteorShowerTypes)
            {
                if (ShowerTypes.ContainsKey(activeShower.Id))
                    ShowerTypes[activeShower.Id].SetActive(true);
            }

            foreach (var seasonHolder in SeasonTypes.Values)
            {
                seasonHolder.SetActive(false);
            }
            foreach (var activeSeason in SelectedPlanet.CurrentMeteorSeasons)
            {
                if (SeasonTypes.ContainsKey(activeSeason.Id))
                    SeasonTypes[activeSeason.Id].SetActive(true);
            }
        }
        public void RefreshTraitList()
        {
            if (SelectedPlanet == null)
                return;

            foreach (var traitContainer in Traits.Values)
            {
                traitContainer.SetActive(false);
            }
            if (SelectedPlanet.IsRandom)
            {
                Traits[ModAssets.CustomTraitID].SetActive(true);
            }
            else
            {
                foreach (var activeTrait in SelectedPlanet.CurrentTraits)
                {
                    Traits[activeTrait].SetActive(true);
                }
            }
        }
        public static void SetResetButtonStates()
        {
            if (ResetButton != null)
                ResetButton.SetInteractable(!PresetApplied);
            if (ResetAllButton != null)
                ResetAllButton.SetInteractable(!PresetApplied);
        }

        private static bool _presetApplied = false;
        public static bool PresetApplied
        {
            get
            {
                return _presetApplied;
            }
            set
            {
                _presetApplied = value;
                SetResetButtonStates();
            }
        }

        void TryApplyingCoordinates(string msg, bool Height)
        {
            if (int.TryParse(msg, out var size))
            {
                if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                {
                    if (size == (Height ? current.CustomPlanetDimensions.Y : current.CustomPlanetDimensions.X))
                        return;

                    current.ApplyCustomDimension(size, Height);
                    UpdateSizeLabels(current);
                }
            }
        }


        string Warning3 = "EC1802";
        string Warning2 = "ff8102";
        string Warning1 = "F6D42A";

        public void UpdateSizeLabels(StarmapItem current)
        {
            PlanetSizeWidth.EditTextFromData(current.CustomPlanetDimensions.X.ToString());
            PlanetSizeHeight.EditTextFromData(current.CustomPlanetDimensions.Y.ToString());
            PercentageLargerThanTerra(current, out var Percentage);
            if (Percentage > 200)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning3);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning3));
            }
            else if (Percentage > 100)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning2);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning2));
            }
            else if (Percentage > 33)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning1);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning1));
            }
            else
            {
                AsteroidSizeLabel.text = ASTEROIDSIZE.DESCRIPTOR.LABEL;
                AsteroidSizeTooltip.SetSimpleTooltip(ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);
            }
        }
        void PercentageLargerThanTerra(StarmapItem current, out int dimensions)
        {
            float TerraArea = 240 * 380;
            float CustomSize = current.CustomPlanetDimensions.X * current.CustomPlanetDimensions.Y;

            dimensions = Mathf.RoundToInt((CustomSize / TerraArea) * 100f);
            dimensions -= 100;
        }




        public class GalleryItem : KMonoBehaviour
        {
            public LocText ItemNumber;
            public FToggleButton ActiveToggle;
            public GameObject DisabledOverlay;

            public void Initialize(StarmapItem planet)
            {

                Image itemIconImage = transform.Find("Image").GetComponent<Image>();
                ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
                DisabledOverlay = transform.Find("DisabledOverlay").gameObject;
                ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
                itemIconImage.sprite = planet.planetSprite;

                UnityEngine.Rect rect = itemIconImage.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 80f;
                    itemIconImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (5 + (80 - size) / 2), size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 80f;
                    itemIconImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }



                UIUtils.AddSimpleTooltipToObject(this.transform, planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300, true);
                Refresh(planet, true);
            }
            public void Refresh(StarmapItem planet, bool inCluster, bool currentlySelected = false)
            {
                float number = planet.InstancesToSpawn;
                bool planetActive = inCluster;//CGSMClusterManager.CustomCluster.HasStarmapItem(planet.Id)

                ActiveToggle.ChangeSelection(currentlySelected);
                DisabledOverlay.SetActive(!planetActive);
                ItemNumber.gameObject.SetActive(planetActive);
                if (planetActive)
                    ItemNumber.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", number.ToString("0.0"));
            }
        }

        private void OnMouseOverToggle() => KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover"));

        public void AddItemToGallery(StarmapItem planet)
        {
            if (planetoidGridButtons.ContainsKey(planet))
            {
                SgtLogger.warning(planet.id + " was already in the gallery");
                return;
            }

            // PermitPresentationInfo presentationInfo = permit.GetPermitPresentationInfo();
            GameObject availableGridButton = Util.KInstantiateUI(PlanetoidEntryPrefab, galleryGridContainer);
            var itemLogic = availableGridButton.AddComponent<GalleryItem>();
            itemLogic.Initialize(planet);


            LocText itemNameText = availableGridButton.transform.Find("Label").GetComponent<LocText>();
            itemNameText.SetText(planet.DisplayName);
            UIUtils.TryChangeText(availableGridButton.transform, "Label", planet.DisplayName);


            itemLogic.ActiveToggle.OnClick += () => this.SelectItem(planet);
            itemLogic.ActiveToggle.OnDoubleClick += () =>
            {
                this.SelectItem(planet);
                if (SelectedPlanet != null)
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    RefreshView();
                }
            };


            planetoidGridButtons[planet] = itemLogic;
            //this.SetItemClickUISound(planet, component2);
            availableGridButton.SetActive(true);
        }
        public class CategoryItem : KMonoBehaviour
        {
            public Image CategoryIcon;
            public FToggleButton ActiveToggle;
            public StarmapItemCategory Category;

            public void Initialize(StarmapItemCategory category, Sprite newSprite)
            {
                if (newSprite != null)
                {
                    CategoryIcon = transform.Find("Image").GetComponent<Image>();
                }
                Category = category;
                ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
                Refresh(StarmapItemCategory.Starter, newSprite);
            }
            public void Refresh(StarmapItemCategory category, Sprite newSprite)
            {
                ActiveToggle.ChangeSelection(this.Category == category);
                if (newSprite != null)
                {
                    CategoryIcon.sprite = newSprite;
                }
            }
        }

        private void AddCategoryItem(StarmapItemCategory StarmapItemCategory)
        {
            GameObject categoryItem = Util.KInstantiateUI(this.PlanetoidCategoryPrefab, this.categoryListContent, true);

            string categoryName = ModAssets.Strings.CategoryEnumToName(StarmapItemCategory); //CATEGORYENUM



            categoryItem.transform.Find("Label").GetComponent<LocText>().SetText(categoryName);
            var item = categoryItem.AddOrGet<CategoryItem>();
            item.Initialize(StarmapItemCategory, Assets.GetSprite("unknown"));
            item.ActiveToggle.OnClick += (() => this.SelectCategory(StarmapItemCategory));
            this.categoryToggles.Add(StarmapItemCategory, item);
        }


        #endregion

    }
}
