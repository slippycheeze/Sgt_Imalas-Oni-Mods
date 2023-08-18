﻿using Imalas_TwitchChaosEvents.Meteors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.OmegaSawblade
{
    internal class OmegaSawbladeConfig : IEntityConfig
    {
        public const string ID = "ITC_OmegaSawblade";
        public GameObject CreatePrefab()
        {
            GameObject entity = EntityTemplates.CreateEntity(ID, STRINGS.ITEMS.ICT_OMEGASAWBLADE.NAME);
            
            entity.AddOrGet<SaveLoadRoot>();
            entity.AddOrGet<LoopingSounds>();

            
            KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
            kbatchedAnimController.AnimFiles = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "omega_sawblade_kanim")
            };
            kbatchedAnimController.isMovable = true;
            kbatchedAnimController.initialAnim = "rotating";
            kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
            kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
            entity.AddOrGet<KCircleCollider2D>().radius = 0.75f; 
            entity.AddComponent<CircleCollider2D>().radius = 0.75f;
            entity.AddOrGet<OmegaSawblade>();
            return entity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst)
        {

        }
    }
}
