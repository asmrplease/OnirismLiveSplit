using System;

namespace OnirismLiveSplit.Events;
public record SplitEventArgs(
    GameEvent GameEvent,
    string SplitName,
    TimeSpan OverallTime,
    TimeSpan? Delta,
    TimeSpan? BestPossibleTime)
{
    public GameEvent GameEvent = GameEvent;
    public string SplitName = SplitName;
    public TimeSpan OverallTime = OverallTime;
    public TimeSpan? Delta = Delta;
    public TimeSpan? BestPossibleTime = BestPossibleTime;

    public override string ToString()
    {
        return $"{GameEvent}-{SplitName}: Overall- {OverallTime}, Delta- {Delta}, Best Possible: {BestPossibleTime}.";
    }
}
