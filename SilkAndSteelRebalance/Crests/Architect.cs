using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using static ToolItemManager;

namespace SilkAndSteelRebalance.Crests {
    [HarmonyPatch]
    internal static class Architect {
        private const int cappedQuickCraftUses = 1;
        private static int currentQuickCraftUses = 0;

        [HarmonyPatch(typeof(ToolItemManager), nameof(TryReplenishTools))]
        [HarmonyPrefix]
        private static bool TryReplenishToolsPrefix(ReplenishMethod method, ref bool __result) {
            if (method != ReplenishMethod.QuickCraft)
                return true;

            if (currentQuickCraftUses >= cappedQuickCraftUses) {
                __result = false;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(ToolItemManager), nameof(TryReplenishTools))]
        [HarmonyPostfix]
        private static void TryReplenishToolsPostfix(bool doReplenish, ReplenishMethod method, ref bool __result) {
            if (method == ReplenishMethod.Bench) {
                currentQuickCraftUses = 0;
                return;
            }

            if (doReplenish && method == ReplenishMethod.QuickCraft && __result)
                currentQuickCraftUses++;
        }
    }
}
