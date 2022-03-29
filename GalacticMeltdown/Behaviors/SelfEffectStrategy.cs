using System.Collections.Generic;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
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
    [JsonProperty] private IEnumerable<ActorStateChangerData> _stateChangers;
    [JsonProperty] private bool _activateWhenTargetIsVisible;
    [JsonProperty] private int _energyCost;

    [JsonConstructor]
    private SelfEffectStrategy()
    {
    }
    
    public SelfEffectStrategy(SelfEffectStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _activateWhenTargetIsVisible = data.ActivateIfTargetIsVisible;
        _energyCost = data.SelfEffectCost;
        ControlledNpc = controlledNpc;
        _stateChangers = data.ActorStateChangerData;
        if (data.Cooldown <= 0) return;
        _selfEffectStrategyCounter = new Counter(ControlledNpc.Level, data.Cooldown, 0);
        ControlledNpc.Died += _selfEffectStrategyCounter.RemoveCounter;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if (_selfEffectStrategyCounter is not null)
            ControlledNpc.Died += _selfEffectStrategyCounter.RemoveCounter;
    }
    
    public override ActorActionInfo TryAct()
    {
        if (_stateChangers is null || _activateWhenTargetIsVisible && ControlledNpc.CurrentTarget is null ||
            _selfEffectStrategyCounter is not null && !_selfEffectStrategyCounter.FinishedCounting) return null;

        foreach (var stateChanger in _stateChangers)
        {
            DataHolder.ActorStateChangers[stateChanger.Type](ControlledNpc, stateChanger.Power,
                stateChanger.Duration);
        }

        _selfEffectStrategyCounter?.ResetTimer();
        ControlledNpc.Energy -= _energyCost;
        return new ActorActionInfo(ActorAction.ApplyEffect, new List<(int, int)> {(ControlledNpc.X, ControlledNpc.Y)});
    }
}