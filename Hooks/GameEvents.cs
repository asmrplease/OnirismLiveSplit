using OnirismLiveSplit.Events;
using OnirismLiveSplit.Utils;
using Slate;
using System;
using UnityEngine.SceneManagement;

namespace OnirismLiveSplit.Hooks;

public class GameEvents : IDisposable
{
    /// <summary>
    /// Fires when the scene is about to unload, passes the name of the next scene
    /// </summary>
    public static event Action<string>? OnSceneExit;
    /// <summary>
    /// Fires when scene loading is complete and gameplay is about to start, passes the name of the newly loaded scene
    /// </summary>
    public static event Action<string>? OnLoadComplete;
    /// <summary>
    /// Fires when any event is detected
    /// </summary>
    public static event Action<GameEvent>? OnEvent;

    public GameEvents()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        Cutscene.OnCutsceneStarted += OnCutsceneStarted;
    }

    public void Dispose()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Cutscene.OnCutsceneStarted -= OnCutsceneStarted;
    }

    void OnCutsceneStarted(Cutscene c) => OnEvent?.Invoke(new (EventType.Cutscene, c.name));

    public static void NotifyExit(string sceneName) 
    {
        OnEvent?.Invoke(new (EventType.SceneLoad, sceneName));
        OnSceneExit?.Invoke(sceneName);
    } 

    public static void NotifyEvent(GameEvent gameEvent) => OnEvent?.Invoke(gameEvent);

    public static void NotifyLoadComplete(string sceneName) => OnLoadComplete?.Invoke(sceneName);

    void HandleSceneLoaded(Scene s, LoadSceneMode m)
    {
        Log.Info($"Scene Loaded {s.name}");
        NotifyLoadComplete(s.name);
    }
}