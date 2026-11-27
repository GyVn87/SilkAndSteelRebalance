using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsRebalanced.Tools {
    [HarmonyPatch]
    internal static class ToolReplenishingCost {
        private static readonly Dictionary<int, float> pouchLevelCostMultiplier = new() {
            {0, 1.0f },
            {1, 0.9f },
            {2, 0.8f },
            {3, 0.7f },
            {4, 0.6f }
        };

        [HarmonyPatch(typeof(GlobalSettings.Gameplay), nameof(GlobalSettings.Gameplay.ToolReplenishCost), MethodType.Getter)]
        [HarmonyPostfix]
        private static void Postfix(ref int __result) {
            int currentPouchLevel = PlayerData.instance.ToolPouchUpgrades;
            __result = Mathf.RoundToInt((float)__result * pouchLevelCostMultiplier.GetValueOrDefault(currentPouchLevel, 1f));
        }
    }
}
