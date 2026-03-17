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
        var mode = SplitOn.GetModeForType(gameEvent.type);
        if (mode == RepeatMode.Never) { Log.Debug($"{gameEvent} is configured to not trigger a split."); return; }
        if (mode == RepeatMode.Unique && completedEvents.ContainsKey(gameEvent)) { Log.Info($"Event {gameEvent} has already occurred"); return; }
        //if (!eventWhitelist.TryGetValue(eventName, out _)) { Log.Debug($"Event {eventName} is not in whitelist."); return; }

        TrySplit();
        var split = client.GetLastSplitTime();
        if (split is not TimeSpan) { Log.Warning("Failed to get Delta");}
        completedEvents.Add(gameEvent, split);
    }

    void DebugStartedHandler()
    {
        Log.Debug("Run started Event!");
    }

    void DebugSplitHandler(SplitEventArgs obj)
    {
        Log.Debug($"Split occurred event {obj.ToString()}");
    }

    static int cooldownSeconds = 5;
    static TimeSpan cooldown = new(0, 0, cooldownSeconds);
    TimeSpan? lastSplit;

    void TrySplit()
    {
        Log.Info("Split");
        var now = client.GetCurrentTime();
        if (lastSplit is null)
        {
            client.Split();
            lastSplit = now;
            return;
        }

        TimeSpan diff = (TimeSpan)(now - lastSplit);
        //Log.Info($"now: {now}, diff: {diff}");
        if (diff <= cooldown) { Log.Info("split time was too close to previous split"); return; }

        string splitName = client.GetCurrentSplitName();
        client.Split();
        lastSplit = now;
        var delta = client.GetDelta();
        var bestPossible = client.GetBestPossibleTime();
        if (delta is not TimeSpan) { Log.Warning("Failed to get Delta"); }
        if (bestPossible is not TimeSpan) { Log.Warning("Failed to get best possible time"); }

        OnSplitOccurred?.Invoke(new SplitEventArgs(splitName, now, diff, delta, bestPossible));
    }

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
