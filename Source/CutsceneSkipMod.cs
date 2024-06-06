using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NineSolsAPI;
using UnityEngine;
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

        harmony = Harmony.CreateAndPatchAll(typeof(CutsceneSkip).Assembly);
        configShortcutSkipCutscene = Config.Bind("Shortcuts", "SkipCutscene", new KeyboardShortcut(KeyCode.Escape), "Skip the current cutscene");
        
        KeybindManager.Add(this, SkipCutscene, () => configShortcutSkipCutscene.Value);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }


    private void SkipCutscene() {
    }

    private void OnDestroy() {
        harmony.UnpatchSelf();
    }
}
