using GlobalSettings;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace ToolsRebalanced.Tools.BlueTools {
    [HarmonyPatch]
    internal static class BarbedBracelet {
        private const float damageTakenMutiplier = 1f;
        private const float damageDealtMultiplier = 1.2f;
        private const int silksTakenOnHit = 4;
        private const ToolItemType toolType = ToolItemType.Blue;

        [HarmonyPatch(typeof(GlobalSettings.Gameplay), nameof(GlobalSettings.Gameplay.BarbedWireDamageTakenMultiplier), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ChangeDamageTakenMultiplier(ref float __result) {
            __result = damageTakenMutiplier;
        }

        [HarmonyPatch(typeof(GlobalSettings.Gameplay), nameof(GlobalSettings.Gameplay.BarbedWireDamageDealtMultiplier), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ChangeDamageDealtMultipler(ref float __result) {
            __result = damageDealtMultiplier;
        }

        [HarmonyPatch(typeof(ToolItem), nameof(ToolItem.Type), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ChangeToolTypeProperty(ToolItem __instance, ref ToolItemType ___type, ref ToolItemType __result) {
            if (__instance != null && __instance.name == "Barbed Wire") {
                ___type = toolType;
                __result = toolType;
            }
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.TakeDamage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TakeSilkOnHit(IEnumerable<CodeInstruction> instructions) {
            MethodInfo takeSilkInfo = AccessTools.Method(typeof(BarbedBracelet), nameof(BarbedBracelet.TakeSilk));
            MethodInfo doMossToolHitInfo = AccessTools.Method(typeof(HeroController), "DoMossToolHit");

            var codeMatcher = new CodeMatcher(instructions);

            //  DoMossToolHit();
            //  IL_0663: ldarg.0
            //  IL_0664: call instance void HeroController::DoMossToolHit()
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call, doMossToolHitInfo)
                )
                .ThrowIfInvalid("ToolsRebalanced.Tools.BarbedBracelet.TakeSilkOnHit: Could not find call to HeroController::DoMossToolHit");
            codeMatcher.Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, takeSilkInfo)
                );

            return codeMatcher.Instructions();
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.DoSpecialDamage))]
        [HarmonyPostfix]
        private static void DoSpecialDamagePostfix(int damageAmount) {
            TakeSilk(damageAmount);
        }

        private static void TakeSilk(int damageAmount) {
            ToolItem barbedWireTool = Gameplay.BarbedWireTool;
            if ((bool)barbedWireTool && barbedWireTool.IsEquipped)
                HeroController.instance.TakeSilk(silksTakenOnHit * damageAmount);
        }
    }
}
