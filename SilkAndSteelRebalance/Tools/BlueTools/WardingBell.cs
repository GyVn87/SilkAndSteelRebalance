using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using UnityEngine;

namespace SilkAndSteelRebalance.Tools.BlueTools {
    [HarmonyPatch]
    internal static class WardingBell {
        private const int refundedSilk = 3;

        [HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
        [HarmonyPostfix]
        private static void HeroController_StartPostfix(HeroController __instance) {
            GameObject heroGameObject = __instance.gameObject;
            PlayMakerFSM? bindFsm = FsmUtil.GetFsm(heroGameObject, "Bind");
            if (bindFsm is null) {
                Utils.Log.Error("Not found FSM: Bind");
                return;
            }

            FsmState removeSilkState = FsmUtil.MustGetState(bindFsm, "Remove Silk?");
            CheckIfToolEquipped? checkIfToolEquippedAction = FsmUtil.GetFirstActionOfType<CheckIfToolEquipped>(removeSilkState);
            if (checkIfToolEquippedAction is null) {
                Utils.Log.Error("Not found Action: CheckIfToolEquipped");
                return;
            }

            FsmEvent? originalTrueEvent = checkIfToolEquippedAction.trueEvent;
            if (originalTrueEvent is null) {
                Utils.Log.Error("Not found event: CheckIfToolEquipped.trueEvent");
                return;
            }
            checkIfToolEquippedAction.trueEvent = null;

            FsmUtil.InsertLambdaMethodAfter(checkIfToolEquippedAction, (finish) => {
                if (checkIfToolEquippedAction.IsTrue) {
                    WardingBellHelper.RefundSilk(refundedSilk);
                    bindFsm.fsm.Event(originalTrueEvent);
                }
                finish();
            });
        }
    }

    internal static class WardingBellHelper {
        internal static void RefundSilk(int silk) {
            HeroController.instance.AddSilk(silk, false);
        }
    }
}
