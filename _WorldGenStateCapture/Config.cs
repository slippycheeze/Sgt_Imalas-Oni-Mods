﻿using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _WorldGenStateCapture
{
    [Serializable]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("World Parser")]
    public class Config : SingletonOptions<Config>
    {
        [Option("STRINGS.WORLDPARSERMODCONFIG.TARGETCLUSTERBASE.NAME", "STRINGS.WORLDPARSERMODCONFIG.TARGETCLUSTERBASE.DESC")]
        [JsonProperty]
        public string TargetCoordinateBase { get; set; }
        [Option("STRINGS.WORLDPARSERMODCONFIG.TARGETCLUSTERDLC.NAME", "STRINGS.WORLDPARSERMODCONFIG.TARGETCLUSTERDLC.DESC")]
        [JsonProperty]
        public string TargetCoordinateDLC { get; set; }

        [Option("STRINGS.WORLDPARSERMODCONFIG.TARGETNUMBER.NAME", "STRINGS.WORLDPARSERMODCONFIG.TARGETNUMBER.DESC")]
        [JsonProperty]
        public int TargetNumber { get; set; }
        [Option("STRINGS.WORLDPARSERMODCONFIG.TARGETINFINITE.NAME", "STRINGS.WORLDPARSERMODCONFIG.TARGETINFINITE.DESC")]
        [JsonProperty]
        public bool ContinuousParsing { get; set; }
        public Config()
        {
            TargetCoordinateBase = "SNDST-A";
            TargetCoordinateDLC = "SNDST-C";
            TargetNumber = 2;
            ContinuousParsing = false;
        }
    }
}
