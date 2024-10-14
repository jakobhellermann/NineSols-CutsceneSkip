using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using NineSolsAPI;
using UnityEngine;
using UnityEngine.Playables;
using Object = System.Object;
using PluginInfo = CutsceneSkipMod.PluginInfo;

namespace CutsceneSkip;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class CutsceneSkip : BaseUnityPlugin {
    private ConfigEntry<KeyboardShortcut> configShortcutSkipCutscene = null!;

    private Harmony harmony = null!;

    private void Awake() {
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);

        try {
            harmony = Harmony.CreateAndPatchAll(typeof(CutsceneSkip).Assembly);
        } catch (Exception e) {
            Log.Error(e);
        }

        configShortcutSkipCutscene = Config.Bind("Shortcuts",
            "SkipCutscene",
            new KeyboardShortcut(KeyCode.Escape),
            "Skip the current cutscene");

        KeybindManager.Add(this, SkipCutscene, () => configShortcutSkipCutscene.Value);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loadesd!");
    }


    public static bool SkippingCutscene = false;

    private static void SkipCutscene() {
        if (GameCore.Instance is not { } gameCore) return;

        var skippedSomething = false;

        if (gameCore.currentCutScene is SimpleCutsceneManager cutsceneManager) {
            skippedSomething = true;
            cutsceneManager.playableDirector.playableGraph.GetRootPlayable(0).SetDone(true);
        }

        if (DialoguePlayer.Instance.OnMission) {
            skippedSomething = true;
            SkippingCutscene = true;
            SkipDialogueStep();
        }

        if (!skippedSomething) {
            ToastManager.Toast($"nothing to skip");
        }
    }

    private static void SkipDialogueStep() {
        var dialoguePlayer = DialoguePlayer.Instance;
        if (!dialoguePlayer.OnMission) return;

        var nextChat = typeof(DialoguePlayer).GetMethod("NextChat", BindingFlags.Instance | BindingFlags.NonPublic)!;

        const int repetitions = 20;
        for (var i = 0; dialoguePlayer.OnMission && i < repetitions; i++) {
            try {
                nextChat.Invoke(dialoguePlayer, []);
            } catch (Exception e) {
                ToastManager.Toast(e);
            }
        }
    }

    public static async void EndSkipDialogue() {
        var gracePeriod = TimeSpan.FromSeconds(2);
        await UniTask.Delay(gracePeriod, DelayType.DeltaTime);
        CutsceneSkip.SkippingCutscene = false;
    }

    private void Update() {
        if (SkippingCutscene) {
            SkipDialogueStep();
        }
    }

    private void OnDestroy() {
        harmony?.UnpatchSelf();
    }
}
