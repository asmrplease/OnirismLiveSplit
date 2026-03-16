using BepInEx.Configuration;
using OnirismLiveSplit.Events;

namespace OnirismLiveSplit.Behaviors;
public class SplitOn
{
    const string SectionName = "Autosplit Settings";

    static ConfigEntry<RepeatMode>? SceneLoadConfig;
    static ConfigEntry<RepeatMode>? SectorActivatorConfig;
    static ConfigEntry<RepeatMode>? CutscenesConfig;
    static ConfigEntry<RepeatMode>? ItemCollectConfig;
    static ConfigEntry<RepeatMode>? JulieCallConfig;
    //public static bool UseWhitelist = false;
    //public static bool UseBlacklist = false;

    public SplitOn(ConfigFile config)
    {
        SceneLoadConfig         = config.Bind(SectionName, "Split on Scene Load",   RepeatMode.Never, "Performs a split automatically when a scene has loaded.");
        SectorActivatorConfig   = config.Bind(SectionName, "Split on Sector Activators", RepeatMode.Never, "Performs a split automatically when a sector activator is triggered.");
        CutscenesConfig         = config.Bind(SectionName, "Split on Cutscenes",    RepeatMode.Never, "Performs a split automatically when a cutsene starts.");
        ItemCollectConfig       = config.Bind(SectionName, "Split on Item Collect", RepeatMode.Never, "Performs a split automatically when an item is picked up.");
        JulieCallConfig         = config.Bind(SectionName, "Split on Julie Calls",  RepeatMode.Never, "Performs a split automatically when a Julie phonecall starts.");
    }

    public static RepeatMode GetModeForType(EventType eventType)
    {
        return eventType switch
        {
            EventType.SceneLoad         => SceneLoadConfig?.Value       ?? RepeatMode.Never,
            EventType.SectorActivator   => SectorActivatorConfig?.Value ?? RepeatMode.Never,
            EventType.Cutscene          => CutscenesConfig?.Value       ?? RepeatMode.Never,
            EventType.ItemCollection    => ItemCollectConfig?.Value     ?? RepeatMode.Never,
            EventType.JulieCall         => JulieCallConfig?.Value       ?? RepeatMode.Never,
            _                           => RepeatMode.Never,
        };
    }
}

public enum RepeatMode
{
    Never,
    Unique,
    Always,
}