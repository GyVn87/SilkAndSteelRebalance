using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using UnityEngine;

namespace SilkAndSteelRebalance.Abilities {
    [HarmonyPatch]
    internal static class Bind {
        [HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
        [HarmonyPostfix]
        private static void HeroController_StartPostfix(HeroController __instance) {
            GameObject heroGameObject = __instance.gameObject;
            PlayMakerFSM? bindFsm = FsmUtil.GetFsm(heroGameObject, "Bind"); 
            if (bindFsm is null) {
                Utils.Log.Error("Not found FSM: Bind");
                return;
            }

            FsmUtil.RemoveFirstActionOfType<TakeSilk>(bindFsm, "Remove Silk?");
        }
    }
}
