using System;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Utility;

public class Counter
{
    public readonly LimitedNumber Timer;
    protected readonly Level Level;
    protected Action<Counter> Action;
    public bool FinishedCounting => Timer.Value == 0;

    public Counter(Level level, int timer, int startingTime, Action<Counter> action = null)
    {
        level.TurnFinished += NextTurn;
        Level = level;
        Timer = new LimitedNumber(startingTime, timer, 0);
        if (action is not null) Action = action;
    }

    private void NextTurn(object _, EventArgs __)
    {
        if (FinishedCounting) return;
        Timer.Value--;
        if (!FinishedCounting) return;
        Action?.Invoke(this);
    }

    public void ResetTimer()
    {
        Timer.Value = Timer.MaxValue.Value;
    }

    public void StopTimer()
    {
        Timer.Value = 0;
    }

    public void RemoveCounter()
    {
        Level.TurnFinished -= NextTurn;
        ResetTimer();
    }
}
