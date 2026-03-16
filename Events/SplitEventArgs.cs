using System;

namespace OnirismLiveSplit.Events;
public class SplitEventArgs
{
    public string SplitName;
    public TimeSpan OverallTime;
    public TimeSpan CurrentSplit;
    public TimeSpan Delta;
    public TimeSpan BestPossibleTime;

    public SplitEventArgs(
        string SplitName, 
        TimeSpan OverallTime, 
        TimeSpan CurrentSplit, 
        TimeSpan Delta,
        TimeSpan BestPossibleTime)
    {
        this.SplitName = SplitName;
        this.OverallTime = OverallTime;
        this.CurrentSplit = CurrentSplit;
        this.Delta = Delta;
        this.BestPossibleTime = BestPossibleTime;
    }

    public override string ToString()
    {
        return $"{SplitName}: Overall- {OverallTime}, Current- {CurrentSplit}, Delta- {Delta}, Best Possible: {BestPossibleTime}.";
    }
}
