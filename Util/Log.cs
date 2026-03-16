using BepInEx.Logging;

namespace OnirismLiveSplit.Utils;

/// <summary>
/// Global instance for logging to make message creation simple.
/// </summary>
internal class Log
{
    static ManualLogSource? i;
    public Log(ManualLogSource manualLogSource) { i = manualLogSource; }

    public static void Debug(string message) => i?.LogDebug(message);
    public static void Info(string message) => i?.LogInfo(message);
    public static void Message(string message) => i?.LogMessage(message);
    public static void Warning(string message) => i?.LogWarning(message);
    public static void Error(string message) => i?.LogError(message);    
    public static void Fatal(string message) => i?.LogFatal(message);
}
