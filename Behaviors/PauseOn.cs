using BepInEx.Configuration;

namespace OnirismLiveSplit.Behaviors;
public class PauseOn
{
    const string SectionName = "Autopause Settings";
    public static bool Loading => LoadingPauseConfig?.Value ?? true;
    static ConfigEntry<bool>? LoadingPauseConfig;

    public static bool Cutscene => CutscenePauseConfig?.Value ?? false;
    static ConfigEntry<bool>? CutscenePauseConfig;

    public PauseOn(ConfigFile config)
    {
        LoadingPauseConfig  = config.Bind(SectionName, "Pause during Scene Loads", true, "Automatically pause timer while scene is loading");
        CutscenePauseConfig = config.Bind(SectionName, "Pause during Cutscenes", false, "Automatically pause timer while cutscenes are playing");
    }
}
