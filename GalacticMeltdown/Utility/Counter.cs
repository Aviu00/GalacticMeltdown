using System;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Utility;

public class Counter
{
    protected readonly LimitedNumber Timer;
    protected readonly Level Level;
    protected Action<Counter> Action;
    public bool FinishedCounting => Timer.Value == 0;

    public Counter(Level level, int timer, Action<Counter> action)
    {
        level.TurnFinished += NextTurn;
        Level = level;
        Timer = new LimitedNumber(timer);
        Action = action;
    }

    public Counter(Level level, int timer) : this(level, timer, _ => { })
    {
    }

    private void NextTurn(object _, EventArgs __)
    {
        if (FinishedCounting) return;
        Timer.Value--;
        if (!FinishedCounting) return;
        Action(this);
    }

    public void ResetTimer()
    {
        Timer.Value = Timer.MaxValue;
    }

    public void StopTimer()
    {
        Level.TurnFinished -= NextTurn;
        ResetTimer();
    }
}