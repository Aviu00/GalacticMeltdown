using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class SelfEffectStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "SelfEffect";
    private const int DefaultPriority = 25;
    [JsonProperty] private Counter _selfEffectStrategyCounter;
    [JsonProperty] private ActorStateChangerDataExtractor.ActorStateChangerData _stateChanger;

    public SelfEffectStrategy(SelfEffectStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        ControlledNpc = controlledNpc;
        _stateChanger = data.ActorStateChangerData;
        if (data.Cooldown > 0)
        {
            _selfEffectStrategyCounter = new Counter(ControlledNpc.Level, data.Cooldown, 0);
            ControlledNpc.Died += _selfEffectStrategyCounter.RemoveCounter;
        }
    }

    public override bool TryAct()
    {
        if (_stateChanger is not null &&
            (_selfEffectStrategyCounter is null || _selfEffectStrategyCounter.FinishedCounting))
        {
            DataHolder.ActorStateChangers[_stateChanger.Type](ControlledNpc, _stateChanger.Power,
                _stateChanger.Duration);
            if (_selfEffectStrategyCounter is not null) _selfEffectStrategyCounter.ResetTimer();
            return true;
        }

        return false;
    }
}