using System;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Utility;

public class Counter
{
    protected readonly LimitedNumber Timer;
    protected readonly Level Level;
    protected Action<Counter> Action;

    public Counter(Level level, int timer, Action<Counter> action)
    {
        level.TurnFinished += NextTurn;
        Level = level;
        Timer = new LimitedNumber(timer);
        Action = action;
    }

    protected Counter(Level level, int timer) : this(level, timer, counter => counter.StopTimer())
    {
    }

    private void NextTurn(object sender, EventArgs _)
    {
        Timer.Value--;
        if(Timer.Value != 0) return;
        Action(this);
    }

    public void ResetTimer()
    {
        Timer.Value = Timer.MaxValue;
    }

    public void StopTimer()
    {
        Level.TurnFinished -= NextTurn;
    }
}