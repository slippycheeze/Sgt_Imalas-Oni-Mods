﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class SolidFuelLoaderConfig : IBuildingConfig
    {
        public const string ID = "RTB_SolidFuelLoader";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef() => BaseFuelLoader.CreateBaseFuelLoaderPort(ID, "conduit_port_solid_loader_kanim", ConduitType.Solid, width: 1,height: 2);
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) 
        {
            BaseFuelLoader.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid, 20f, FuelLoaderComponent.LoaderType.Fuel);
        }
        public override void DoPostConfigureComplete(GameObject go) 
        { 
            BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, true); 
        }
    }
}
