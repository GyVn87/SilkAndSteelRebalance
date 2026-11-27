using GlobalSettings;
using HarmonyLib;
using System.Reflection;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace SilkAndSteelRebalance.Tools.YellowTools {
    [HarmonyPatch]
    internal static class MagnetiteBrooch {
        private static bool _cachedConfig = false;
        private static ToolItem? _magnetTool;
        private static ToolItem? _magnetBuffTool;
        private static float _magnetBuffDelayMultiplier;
        private static GameObject? _magnetEffect;
        private static MinMaxFloat _magnetStartDelay;
        private static MinMaxFloat _magnetStartHeight;
        private static float _magnetStartMoveDuration;

        [HarmonyPatch(typeof(CurrencyObjectBase), nameof(CurrencyObjectBase.Awake))]
        [HarmonyPrefix]
        private static void AwakePrefix(CurrencyObjectBase __instance,
                                        ref ToolItem? ___magnetTool,
                                        ref ToolItem? ___magnetBuffTool,
                                        ref GameObject? ___magnetEffect,
                                        ref float ___magnetBuffDelayMultiplier,
                                        ref MinMaxFloat ___magnetStartDelay,
                                        ref MinMaxFloat ___magnetStartHeight,
                                        ref float ___magnetStartMoveDuration) {
            if (__instance is not ShellShard || ___magnetTool != null)
                return;

            if (!TryCacheRosaryMagnetConfig())
                return;

            ___magnetTool = _magnetTool;
            ___magnetBuffTool = _magnetBuffTool;
            ___magnetBuffDelayMultiplier = _magnetBuffDelayMultiplier;
            ___magnetStartDelay = _magnetStartDelay;
            ___magnetStartHeight = _magnetStartHeight;
            ___magnetStartMoveDuration = _magnetStartMoveDuration;

            if (_magnetEffect != null && ___magnetEffect == null) {
                GameObject effectClone = Object.Instantiate(_magnetEffect, __instance.transform, false);
                effectClone.name = _magnetEffect.name;
                effectClone.transform.localPosition = Vector3.zero;
                effectClone.transform.localRotation = Quaternion.identity;
                effectClone.SetActive(false);

                ___magnetEffect = effectClone;
            }
        }

        private static bool TryCacheRosaryMagnetConfig() {
            if (_cachedConfig) 
                return _magnetTool != null;

            GameObject rosaryPrefab = GlobalSettings.Gameplay.LargeGeoPrefab; 
            if (rosaryPrefab == null)
                return false;

            var source = rosaryPrefab.GetComponent<CurrencyObjectBase>();
            if (source == null) 
                return false;

            _magnetTool = AccessTools.FieldRefAccess<CurrencyObjectBase, ToolItem?>(source, "magnetTool");
            _magnetBuffTool = AccessTools.FieldRefAccess<CurrencyObjectBase, ToolItem?>(source, "magnetBuffTool");
            _magnetBuffDelayMultiplier = AccessTools.FieldRefAccess<CurrencyObjectBase, float>(source, "magnetBuffDelayMultiplier");
            _magnetEffect = AccessTools.FieldRefAccess<CurrencyObjectBase, GameObject?>(source, "magnetEffect");
            _magnetStartDelay = AccessTools.FieldRefAccess<CurrencyObjectBase, MinMaxFloat>(source, "magnetStartDelay");
            _magnetStartHeight = AccessTools.FieldRefAccess<CurrencyObjectBase, MinMaxFloat>(source, "magnetStartHeight");
            _magnetStartMoveDuration = AccessTools.FieldRefAccess<CurrencyObjectBase, float>(source, "magnetStartMoveDuration");

            _cachedConfig = _magnetTool != null;

            return _cachedConfig;
        }
    }
}
