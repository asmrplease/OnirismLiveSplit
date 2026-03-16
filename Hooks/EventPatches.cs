using HarmonyLib;

namespace OnirismLiveSplit.Hooks;

[HarmonyPatch(typeof(PhoneTrigger), nameof(PhoneTrigger.StartPlayerDialogue))]
public static class PhoneTriggerWatcher
{

    [HarmonyPrefix]
    public static void Prefix(PhoneTrigger __instance)
    {
        GameEvents.NotifyEvent(new(Events.EventType.JulieCall, __instance.dialogueName));
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.LoadScene))]
public static class SceneSwitchPatch
{
    [HarmonyPrefix] public static void Prefix(string sceneName) => GameEvents.NotifyExit(sceneName);
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
    public static void Prefix(pickup __instance)
    {
        GameEvents.NotifyEvent(new(Events.EventType.ItemCollection, __instance.name));
    }
}