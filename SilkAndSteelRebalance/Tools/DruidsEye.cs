using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SilkAndSteelRebalance.Tools {
    [HarmonyPatch]
    internal static class DruidsEye {
        private const int druidsEyeRestore = 2;
        private const int druidsEyesRestore = 3;

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.DoMossToolHit))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> AdjustSilkRestore(IEnumerable<CodeInstruction> instructions) {
            var codeMatcher = new CodeMatcher(instructions);

         //   IL_0027: ldarg.0
         //   IL_0028: call class ToolItem GlobalSettings.Gameplay::get_MossCreep1Tool()
         //   IL_002d: ldarg.0
         //   IL_002e: ldflda int32 HeroController::mossCreep1Hits
         //   IL_0033: ldc.i4.1
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldflda),
                    new CodeMatch(OpCodes.Ldc_I4_1)
                )
                .ThrowIfInvalid("SilkAndSteelRebalance.Tools.DruidsEye.AdjustSilkRestore: Could not find call to HeroController::mossCreep1Hits")
                .Set(OpCodes.Ldc_I4, druidsEyeRestore);

         //   IL_003b: ldarg.0
         //   IL_003c: call class ToolItem GlobalSettings.Gameplay::get_MossCreep2Tool()
         //   IL_0041: ldarg.0
         //   IL_0042: ldflda int32 HeroController::mossCreep2Hits
         //   IL_0047: ldc.i4.2
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldflda),
                    new CodeMatch(OpCodes.Ldc_I4_2)
                )
                .ThrowIfInvalid("SilkAndSteelRebalance.Tools.DruidsEye.AdjustSilkRestore: Could not find call to HeroController::mossCreep2Hits")
                .Set(OpCodes.Ldc_I4, druidsEyesRestore);

            return codeMatcher.Instructions();
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.TakeDamage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TakeDamagePostfix(IEnumerable<CodeInstruction> instructions) {
            MethodInfo ApplyMultipleMossToolEffectsInfo = AccessTools.Method(typeof(DruidsEyeHelper), nameof(DruidsEyeHelper.ApplyMultipleMossToolEffects));
            MethodInfo DoMossToolHitInfo = AccessTools.Method(typeof(HeroController), "DoMossToolHit");

            var codeMatcher = new CodeMatcher(instructions);

        //  DoMossToolHit();
        //  IL_0663: ldarg.0
        //  IL_0664: call instance void HeroController::DoMossToolHit()
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call, DoMossToolHitInfo)
                )
                .ThrowIfInvalid("SilkAndSteelRebalance.Tools.DruidsEye.TakeDamagePostfix: Could not find call to HeroController::DoMossToolHit");
            codeMatcher.Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, ApplyMultipleMossToolEffectsInfo)
                );

            return codeMatcher.Instructions();
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.DoSpecialDamage))]
        [HarmonyPostfix]
        private static void DoSpecialPostfix(HeroController __instance, int damageAmount) {
            DruidsEyeHelper.ApplyMultipleMossToolEffects(__instance, damageAmount);
        }        
    }

    internal static class DruidsEyeHelper {
        internal static void ApplyMultipleMossToolEffects(HeroController heroController, int damageAmount) {
            if (damageAmount <= 1 || damageAmount >= 10) return;

            int extraEffects = damageAmount - 1;
            for (int i = 0; i < extraEffects; i++)
                heroController.DoMossToolHit();
        }
    }
}
