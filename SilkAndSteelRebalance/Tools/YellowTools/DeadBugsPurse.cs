using HarmonyLib;

namespace ToolsRebalanced.Tools.YellowTools {
    [HarmonyPatch]
    internal static class DeadBugsPurse {
        private const float rosaryHoldPercent = 0.8f;

        [HarmonyPatch(typeof(GlobalSettings.Gameplay), nameof(GlobalSettings.Gameplay.DeadPurseHoldPercent), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ChangeHoldPercent(ref float __result) {
            __result = rosaryHoldPercent;
        } 
    }
}
