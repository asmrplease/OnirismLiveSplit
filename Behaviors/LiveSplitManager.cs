using BepInEx;
using LiveSplitInterop.Clients;
using LiveSplitInterop.Commands;
using OnirismLiveSplit.Events;
using OnirismLiveSplit.Hooks;
using OnirismLiveSplit.Utils;
using Slate;
using System;
using System.Collections.Generic;

namespace OnirismLiveSplit.Behaviors;
public class LiveSplitManager : IDisposable
{
    readonly NamedPipeCommandClient client = new();

    public static event Action<SplitEventArgs>? OnSplitOccurred;
    public static event Action? OnRunStarted;

    static Dictionary<GameEvent, TimeSpan?> completedEvents = [];

    public LiveSplitManager()
    {
        try { client.Connect(5); } catch (TimeoutException e) { Log.Warning(e.Message); return; }

        GameEvents.OnSceneExit += HandleSceneExit;
        GameEvents.OnLoadComplete += HandleSceneLoaded;
        GameEvents.OnEvent += HandleEvent;
        Cutscene.OnCutsceneStarted += HandleCutsceneStart;
        Cutscene.OnCutsceneStopped += HandleCutsceneEnd;

        OnSplitOccurred += DebugSplitHandler;
        OnRunStarted += DebugStartedHandler;
    }

    public void Dispose()
    {
        GameEvents.OnSceneExit -= HandleSceneExit;
        GameEvents.OnLoadComplete -= HandleSceneLoaded;
        GameEvents.OnEvent -= HandleEvent;
        Cutscene.OnCutsceneStarted -= HandleCutsceneStart;
        Cutscene.OnCutsceneStopped -= HandleCutsceneEnd;
        client?.Dispose();
    }

    void HandleEvent(GameEvent gameEvent)
    {
        Log.Debug($"HandleEvent({gameEvent}");
        //if (this.lastSplit is null) { Log.Error("last split time is in an invalid state."); return; }
        if (gameEvent.name.IsNullOrWhiteSpace()) { Log.Warning("gameevent with no name"); return; }
        if (gameEvent.name == "Cutscene_intro_start") { NewRun(); return; }

        Log.Debug("Trying to split...");
        string liveSplitName = "";
        try 
        {
            Log.Debug("Trying to get current split name...");
            liveSplitName = SplitOn.StrictSplits ? client.GetCurrentSplitName() : "";
            Log.Debug($"Got current split: {liveSplitName}");
        }
        catch (Exception e) 
        {
            Log.Error("An error occurred getting split name:");
            Log.Error(e.Message); 
            return; 
        }

        Log.Debug($"{liveSplitName}/{gameEvent.name}");
        if (SplitOn.StrictSplits && !liveSplitName.EndsWith(gameEvent.name)) { Log.Info($"GameEvent {gameEvent.name} was not expected event {liveSplitName}"); return; }

        var mode = SplitOn.GetModeForType(gameEvent.type);
        if (mode == RepeatMode.Never) { Log.Debug($"{gameEvent} is configured to not trigger a split."); return; }
        if (mode == RepeatMode.Unique && completedEvents.ContainsKey(gameEvent)) { Log.Info($"Event {gameEvent} has already occurred"); return; }

        Log.Info("Splitting!");
        client.Split();
        var now             = client.GetCurrentTime();
        var delta           = client.GetDelta();
        var bestPossible    = client.GetBestPossibleTime();
        var lastSplit       = client.GetLastSplitTime();
        if (delta        is not TimeSpan) { Log.Warning("Failed to get Delta"); }
        if (bestPossible is not TimeSpan) { Log.Warning("Failed to get best possible time"); }
        if (lastSplit    is not TimeSpan) { Log.Warning("Failed to get Delta"); }
        completedEvents.TryAdd(gameEvent, lastSplit);
        OnSplitOccurred?.Invoke(new SplitEventArgs(gameEvent, liveSplitName, now, delta, bestPossible));
    }

    void DebugStartedHandler()
    {
        Log.Info("Run started Event!");
    }

    void DebugSplitHandler(SplitEventArgs obj)
    {
        Log.Info($"Split occurred event {obj.ToString()}");
    }

    //static int cooldownSeconds = 1;
    //static TimeSpan cooldown = new(0, 0, cooldownSeconds);
    //TimeSpan? lastSplit;

    void HandleCutsceneStart(Cutscene _)
    {
        Log.Debug("LiveSplitManager.HandleCutsceneStart()");
        if (PauseOn.Cutscene) client.Pause();
    }

    void HandleCutsceneEnd(Cutscene _)
    {
        Log.Debug("LiveSplitManager.HandleCutsceneEnd()");
        if (PauseOn.Cutscene) client.Resume();
    }

    /// <summary>
    /// Responds to a new scene. Ignores main menu and the LoadingZone scene.
    /// </summary>
    /// <param name="sceneName">The name of the loaded scene.</param>
    void HandleSceneLoaded(string sceneName)
    {
        switch (sceneName)
        {
            case "INTRO_CUTSCENES": break;// NewRun(); break;
            case "Main_menu_new": break;
            case "LoadingZone": break;
            default: SceneChanged(); break;
        }
    }

    /// <summary>
    /// Responds to the start of a scene load by pausing the timer.
    /// </summary>
    /// <param name="sceneName"></param>
    void HandleSceneExit(string sceneName)
    {
        if (sceneName == "Main_menu_new") return;
        if (sceneName == "LoadingZone") return;

        if (PauseOn.Loading) client.Pause();
        GameEvents.NotifyEvent(new(EventType.SceneLoad, sceneName));
    }

    void SceneChanged()
    {
        Log.Debug("LiveSplitManager.SceneChanged()");

        if (PauseOn.Loading) client.Resume();
    }

    void NewRun()
    {
        Log.Debug("LiveSplitManager.NewRun()");
        completedEvents = [];
        client.Reset();
        client.Start();
        OnRunStarted?.Invoke();
    }
}
