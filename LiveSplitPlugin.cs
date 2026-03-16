using BepInEx;
using OnirismLiveSplit.Utils;
using HarmonyLib;
using OnirismLiveSplit.Hooks;
using OnirismLiveSplit.Behaviors;

namespace OnirismLiveSplit;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Onirism.exe")]

public class LiveSplitPlugin : BaseUnityPlugin
{
    readonly GameEvents sceneDetection = new();
    readonly LiveSplitManager splitManager = new();
    readonly Harmony harmony = new("OnirismLiveSplit");
    SplitOn? SplitSettings;
    PauseOn? PauseSettings;

    void Awake()
    {
        new Log(Logger);
        Log.Info($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} Awake().");

        SplitSettings = new SplitOn(this.Config);
        PauseSettings = new PauseOn(this.Config);
        harmony.PatchAll();
        Log.Info("LiveSplitPlugin Awake() complete.");
    }

    void OnDestroy()
    {
        Log.Info("OnDestroy()");
        harmony.UnpatchSelf();
        splitManager.Dispose();
        sceneDetection.Dispose();
    }
}
