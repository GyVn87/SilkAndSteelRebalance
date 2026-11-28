using GlobalSettings;
using HarmonyLib;
using System;

namespace ToolsRebalanced.Tools.BlueTools {
    [HarmonyPatch]
    internal static class SpoolExtender {
        private const int baseRewardStrands = 4;
        private const int silkPartsGain = 1;

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.SilkGain), [typeof(HitInstance)])]
        [HarmonyPostfix]
        private static void ExtraSilkOnHit(HeroController __instance, HitInstance hitInstance) {
            if (hitInstance.SilkGeneration != HitSilkGeneration.None && Gameplay.SpoolExtenderTool.IsEquipped) {
                int currentSilk = PlayerData.instance.silk;
                int currentSilkMax = PlayerData.instance.CurrentSilkMax;
                int rewardZone = currentSilkMax - RewardStrands(currentSilkMax) + 1;

                if (currentSilk >= rewardZone)
                    __instance.AddSilkParts(silkPartsGain);
            }
        }

        private static int RewardStrands(int currentSilkMax) => (baseRewardStrands + (currentSilkMax - 12) / 2);
    }
}
