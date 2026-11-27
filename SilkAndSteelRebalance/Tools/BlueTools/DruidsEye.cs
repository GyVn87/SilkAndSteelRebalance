using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ToolsRebalanced.Tools.BlueTools {
    [HarmonyPatch]
    internal static class DruidsEye {
        private const int druidsEyeRestore = 1;
        private const int druidsEyesRestore = 2;

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.DoMossToolHit))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> AdjustSilkRestore(IEnumerable<CodeInstruction> instructions) {
            FieldInfo mossCreep1HitsInfo = AccessTools.Field(typeof(HeroController), "mossCreep1Hits");
            FieldInfo mossCreep2HitsInfo = AccessTools.Field(typeof(HeroController), "mossCreep2Hits");

            var codeMatcher = new CodeMatcher(instructions);

         //   IL_002e: ldflda int32 HeroController::mossCreep1Hits
         //   IL_0033: ldc.i4.1
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldflda, mossCreep1HitsInfo),
                    new CodeMatch(OpCodes.Ldc_I4_1)
                )
                .ThrowIfInvalid("ToolsRebalanced.Tools.DruidsEye.AdjustSilkRestore: Could not find call to HeroController::mossCreep1Hits")
                .Set(OpCodes.Ldc_I4, druidsEyeRestore);

         //   IL_0042: ldflda int32 HeroController::mossCreep2Hits
         //   IL_0047: ldc.i4.2
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldflda, mossCreep2HitsInfo),
                    new CodeMatch(OpCodes.Ldc_I4_2)
                )
                .ThrowIfInvalid("ToolsRebalanced.Tools.DruidsEye.AdjustSilkRestore: Could not find call to HeroController::mossCreep2Hits")
                .Set(OpCodes.Ldc_I4, druidsEyesRestore);

            return codeMatcher.Instructions();
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.TakeDamage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TriggerEffectMultipleTimes(IEnumerable<CodeInstruction> instructions) {
            MethodInfo applyMultipleMossToolEffectsInfo = AccessTools.Method(typeof(DruidsEyeHelper), nameof(DruidsEyeHelper.ApplyMultipleMossToolEffects));
            MethodInfo doMossToolHitInfo = AccessTools.Method(typeof(HeroController), "DoMossToolHit");

            var codeMatcher = new CodeMatcher(instructions);

        //  DoMossToolHit();
        //  IL_0663: ldarg.0
        //  IL_0664: call instance void HeroController::DoMossToolHit()
            codeMatcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call, doMossToolHitInfo)
                )
                .ThrowIfInvalid("ToolsRebalanced.Tools.DruidsEye.TriggerEffectMultipleTimes: Could not find call to HeroController::DoMossToolHit");
            codeMatcher.Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, applyMultipleMossToolEffectsInfo)
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
