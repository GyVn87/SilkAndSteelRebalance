using GlobalEnums;
using HarmonyLib;

namespace ToolsRebalanced.Gameplay {
    [HarmonyPatch]
    internal static class CurrencyNotBreakOnSpike {
        private const int disabledHazardBitMask = (int)HazardType.SPIKES;
        

        [HarmonyPatch(typeof(BreakOnHazard), nameof(BreakOnHazard.Start))]
        [HarmonyPrefix]
        private static void DisableBreakOnSpike(BreakOnHazard __instance, ref int ___hazardMask) {
            if (__instance is null)
                return;

            if (__instance.gameObject.CompareTag("Geo")) {
                ___hazardMask &= ~(1 << disabledHazardBitMask);
            }
        }
    }
}
