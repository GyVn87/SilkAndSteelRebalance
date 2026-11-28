using GlobalSettings;
using HarmonyLib;

namespace ToolsRebalanced.Tools.BlueTools {
    [HarmonyPatch]
    internal static class EggOfFlealia {
        private const int fleaBuffSkillCost = 3;

        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SilkSkillCost), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ChangeSilkSkillCost(ref int __result) {
            if (Gameplay.FleaCharmTool.IsEquipped)
                __result = fleaBuffSkillCost;
        }
    }
}
