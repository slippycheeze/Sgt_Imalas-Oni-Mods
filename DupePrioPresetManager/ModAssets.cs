﻿
using UnityEngine;
using UtilLibs;

namespace DupePrioPresetManager
{
    internal class ModAssets
    {
        public static string DupeTemplatePath;
        public static string FoodTemplatePath;
        public static GameObject PresetWindowPrefab;
        public static void LoadAssets()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundle("dupe_prio_preset_window", platformSpecific: true);
            PresetWindowPrefab = bundle.LoadAsset<GameObject>("Assets/PresetWindowDupePrios.prefab");

            //UIUtils.ListAllChildren(PresetWindowPrefab.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(PresetWindowPrefab);
        }
        public static GameObject ParentScreen
        {
            get
            {
                return parentScreen;
            }
            set
            {
                if (UnityPresetScreen_Priorities.Instance != null && parentScreen != value)
                {
                    //UnityPresetScreen.Instance.transform.SetParent(parentScreen.transform, false);
                    UnityEngine.Object.Destroy(UnityPresetScreen_Priorities.Instance);
                    UnityPresetScreen_Priorities.Instance = null;
                }
                parentScreen = value;
            }
        }
        private static GameObject parentScreen = null;
    }
}