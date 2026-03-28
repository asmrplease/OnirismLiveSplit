using HarmonyLib;
using Onirism;
using OnirismLiveSplit.Events;

namespace OnirismLiveSplit.Hooks;

[HarmonyPatch(typeof(PhoneTrigger), nameof(PhoneTrigger.StartPlayerDialogue))]
public static class PhoneTriggerWatcher
{

    [HarmonyPrefix]
    public static void Prefix(PhoneTrigger __instance)
    {
        GameEvents.NotifyEvent(new(EventType.JulieCall, __instance.dialogueName));
    }
}

[HarmonyPatch(typeof(SceneLoaderSvc), nameof(SceneLoaderSvc.LoadScene), [typeof(SceneConfig), typeof(SaveData)])]
public static class SceneSwitchPatch
{
    [HarmonyPrefix] public static void Prefix(SceneConfig sceneConfig) => GameEvents.NotifyExit(sceneConfig.sceneName);
}

/// <summary>
/// NPCToggle is a component seemingly found on all Sector Activators. 
/// This patch attaches our custom component to sector activators.
/// </summary>
[HarmonyPatch(typeof(NPCToggle), "Awake")]
public static class NPCTogglePatch
{
    [HarmonyPostfix]
    public static void Postfix(NPCToggle __instance)
    {
        __instance.gameObject.AddComponent<ColliderSplitter>();
    }
}


/// <summary>
/// WIP- Currently triggers on every type of pickup, including small pearls, which is probably not consistent enough to be useful.
/// </summary>
[HarmonyPatch(typeof(pickup), nameof(pickup.OnDestroy))]
public static class PickupPatch
{
    [HarmonyPrefix]
    public static void Prefix(pickup __instance) { GameEvents.NotifyEvent(new(EventType.ItemCollection, __instance.name)); }
}

/// <summary>
/// WIP- Intended to trigger a split when a new Raggs Shop is added to the save file
/// </summary>
[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.AddEvent), [typeof(SaveData), typeof(string)])]
public static class ShopPatch
{
    [HarmonyPrefix]
    public static void Prefix(string eventName) 
    {
        if (eventName.EndsWith(')')) return;

        GameEvents.NotifyEvent(new(EventType.GameEventSaved, eventName)); 
    }
}