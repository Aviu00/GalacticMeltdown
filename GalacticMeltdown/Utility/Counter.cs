using System;
using System.Runtime.Serialization;
using GalacticMeltdown.LevelRelated;
using Newtonsoft.Json;

namespace GalacticMeltdown.Utility;

public class Counter
{
    [JsonProperty] public readonly LimitedNumber Timer;
    [JsonProperty] protected readonly Level Level;
    [JsonIgnore] public Action<Counter> Action;
    [JsonIgnore] public bool FinishedCounting => Timer.Value == 0;

    [JsonConstructor]
    protected Counter()
    {
    }
    public Counter(Level level, int timer, int startingTime, Action<Counter> action = null)
    {
        level.TurnFinished += NextTurn;
        Level = level;
        Timer = new LimitedNumber(startingTime, timer, 0);
        if (action is not null) Action = action;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        Level.TurnFinished += NextTurn;
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

    public void RemoveCounter(object _ = null, EventArgs __ = null)
    {
        Level.TurnFinished -= NextTurn;
        ResetTimer();
    }
}
