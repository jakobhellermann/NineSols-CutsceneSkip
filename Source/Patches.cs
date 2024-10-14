using System;
using HarmonyLib;
using UnityEngine;

namespace CutsceneSkip;

[HarmonyPatch]
public class Patches {
    /*[HarmonyPatch(typeof(Timer),
        nameof(Timer.AddTask),
        [typeof(Timer), typeof(Action), typeof(float), typeof(MonoBehaviour), typeof(string)])]
    [HarmonyPrefix]
    private static bool AddTask(ref Action misson) {
        if (!CutsceneSkip.SkippingCutscene) return true;

        misson();
        return false;
    }

    [HarmonyPatch(typeof(Timer),
        nameof(Timer.AddTask),
        [typeof(Action), typeof(float), typeof(GameObject)])]
    [HarmonyPrefix]
    private static bool AddTask2(ref Action misson) {
        if (!CutsceneSkip.SkippingCutscene) return true;

        misson();
        return false;
    }*/

    [HarmonyPatch(typeof(SimpleCutsceneManager), "PlayAnimation")]
    [HarmonyPrefix]
    private static void PlayAnimation(ref float speed) {
        if (!CutsceneSkip.SkippingCutscene) return;

        speed = 1000;
    }

    [HarmonyPatch(typeof(DialoguePlayer), "EndDialogue")]
    [HarmonyPrefix]
    private static bool Y() {
        CutsceneSkip.EndSkipDialogue();
        return true;
    }
}