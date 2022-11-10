﻿using Database;
using KnastoronOniMods;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
namespace Robo_Rockets 
{
    class AIControlModuleLearningConfig : IBuildingConfig
    {
        public const string ID = "RR_AILearningControlModule";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] matCosts = {300f};

            string[] construction_materials = {
                "Steel"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "rocket_habitat_ai_module_kanim", 1000, 400f, matCosts, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);

            go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
            var aiConfig = go.AddOrGet<AIPassengerModule>();
            aiConfig.interiorReverbSnapshot = AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot;
            aiConfig.AiSpeed = 0.75f;
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/AIRocketV2";
            go.AddOrGet<NavTeleporter>();
            go.AddOrGet<LaunchableRocketCluster>();
            go.AddOrGet<RocketAiConditions>();

            go.AddOrGet<RocketProcessConditionDisplayTarget>();
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;


            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1] //top module attaches here
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
            };

            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 50f;

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = ModAssets.Tags.SpaceBrain;
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.refillMass = storage.capacityKg;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
           
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MINOR_PLUS);

            go.AddOrGet<BuildingCellVisualizer>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
        }
        

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
        }
    }

}
