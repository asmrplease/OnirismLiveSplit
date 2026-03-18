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
        var now = client.GetCurrentTime();
        if (this.lastSplit is null)
        {
            client.Split();
            this.lastSplit = now;
            return;
        }

        var mode = SplitOn.GetModeForType(gameEvent.type);
        if (mode == RepeatMode.Never) { Log.Debug($"{gameEvent} is configured to not trigger a split."); return; }
        if (mode == RepeatMode.Unique && completedEvents.ContainsKey(gameEvent)) { Log.Info($"Event {gameEvent} has already occurred"); return; }
        
        TimeSpan diff = (TimeSpan)(now - this.lastSplit);
        if (diff <= cooldown) { Log.Info("split time was too close to previous split"); return; }

        if (gameEvent.name.IsNullOrWhiteSpace()) { Log.Warning("gameevent with no name");return; }
        string liveSplitName = client.GetCurrentSplitName();
        Log.Debug($"{liveSplitName}/{gameEvent.name}");
        if (!liveSplitName.Contains(gameEvent.name)) { Log.Info($"GameEvent {gameEvent.name} was not expected event {liveSplitName}"); return; }

        client.Split();
        this.lastSplit = now;
        var delta = client.GetDelta();
        var bestPossible = client.GetBestPossibleTime();
        var lastSplit = client.GetLastSplitTime();
        if (delta is not TimeSpan) { Log.Warning("Failed to get Delta"); }
        if (bestPossible is not TimeSpan) { Log.Warning("Failed to get best possible time"); }
        if (lastSplit is not TimeSpan) { Log.Warning("Failed to get Delta"); }
        completedEvents.Add(gameEvent, lastSplit);
        OnSplitOccurred?.Invoke(new SplitEventArgs(gameEvent, liveSplitName, now, diff, delta, bestPossible));
    }

    void DebugStartedHandler()
    {
        Log.Debug("Run started Event!");
    }

    void DebugSplitHandler(SplitEventArgs obj)
    {
        Log.Error($"Split occurred event {obj.ToString()}");
    }

    static int cooldownSeconds = 1;
    static TimeSpan cooldown = new(0, 0, cooldownSeconds);
    TimeSpan? lastSplit;

    void HandleCutsceneStart(Cutscene _)
    {
        if (PauseOn.Cutscene) client.Pause();
    }

    void HandleCutsceneEnd(Cutscene _)
    {
        if (PauseOn.Cutscene) client.Resume();
    }

    /// <summary>
    /// Responds to a new scene. Ignores main menu and the LoadingZone scene.
    /// </summary>
    /// <param name="sceneName"></param>
    void HandleSceneLoaded(string sceneName)
    {
        switch (sceneName)
        {
            case "INTRO_CUTSCENES": NewRun(); break;
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
        if (PauseOn.Loading) client.Resume();
    }

    void NewRun()
    {
        lastSplit = null;
        completedEvents = [];
        client.Reset();
        client.Start();
        OnRunStarted?.Invoke();
    }
}
