using System;

namespace OnirismLiveSplit.Events;
public record SplitEventArgs(
    string SplitName,
    TimeSpan OverallTime,
    TimeSpan CurrentSplit,
    TimeSpan? Delta,
    TimeSpan? BestPossibleTime)
{
    public string SplitName = SplitName;
    public TimeSpan OverallTime = OverallTime;
    public TimeSpan CurrentSplit = CurrentSplit;
    public TimeSpan? Delta = Delta;
    public TimeSpan? BestPossibleTime = BestPossibleTime;

    public override string ToString()
    {
        return $"{SplitName}: Overall- {OverallTime}, Current- {CurrentSplit}, Delta- {Delta}, Best Possible: {BestPossibleTime}.";
    }
}
