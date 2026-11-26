using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using UnityEngine;

namespace SilkAndSteelRebalance.Abilities {
    [HarmonyPatch]
    internal static class Clawline {
        [HarmonyPatch(typeof(DamageEnemies), nameof(DamageEnemies.Awake))]
        [HarmonyPrefix]
        private static void DamageEnemies_AwakePrefix(DamageEnemies __instance) {
            if (__instance is not null && __instance.GetName() == "Harpoon Dash Damager")
                __instance.SetSilkGenerationNone();
        }

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
        [HarmonyPostfix]
        private static void HeroController_StartPostfix(HeroController __instance) {
            GameObject heroGameObject = __instance.gameObject;
            PlayMakerFSM? harpoonDashFsm = FsmUtil.GetFsm(heroGameObject, "Harpoon Dash");
            if (harpoonDashFsm is null) return;

            FsmUtil.RemoveFirstActionOfType<TakeSilk>(harpoonDashFsm, "Take Control");

            FsmState canDoState = FsmUtil.MustGetState(harpoonDashFsm, "Can Do?");
            IntCompare? intCompareAction = FsmUtil.GetFirstActionOfType<IntCompare>(canDoState);
            if (intCompareAction is not null) {
                intCompareAction.integer1 = 1;
                intCompareAction.integer2 = 0;
            }
        }
    }
}
