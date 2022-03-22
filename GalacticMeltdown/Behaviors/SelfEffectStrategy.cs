using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class SelfEffectStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "SelfEffect";
    private const int DefaultPriority = 5;
    [JsonProperty] private Counter _selfEffectStrategyCounter;
    [JsonProperty] private ActorStateChangerData _stateChanger;

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

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if (_selfEffectStrategyCounter is not null)
            ControlledNpc.Died += _selfEffectStrategyCounter.RemoveCounter;
    }
    
    public override bool TryAct()
    {
        if (_stateChanger is null ||
            _selfEffectStrategyCounter is not null && !_selfEffectStrategyCounter.FinishedCounting) return false;
        
        DataHolder.ActorStateChangers[_stateChanger.Type](ControlledNpc, _stateChanger.Power,
            _stateChanger.Duration);
        _selfEffectStrategyCounter?.ResetTimer();
        return true;

    }
}