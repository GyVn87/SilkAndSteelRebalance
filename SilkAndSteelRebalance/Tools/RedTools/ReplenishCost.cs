using HarmonyLib;
using UnityEngine;

namespace SilkAndSteelRebalance.Tools.RedTools {
    [HarmonyPatch]
    internal static class ReplenishCost {
        private const float costMultiplier = 0.75f;

        [HarmonyPatch(typeof(GlobalSettings.Gameplay), nameof(GlobalSettings.Gameplay.ToolReplenishCost), MethodType.Getter)]
        [HarmonyPostfix]
        private static void Postfix(ref int __result) {
            __result = Mathf.RoundToInt((float)__result * costMultiplier);
        }
    }
}
