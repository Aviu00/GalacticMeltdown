using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class MeleeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "MeleeAttack";
    private const int DefaultPriority = 10;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _meleeAttackCost;
    [JsonProperty] private Counter _meleeAttackCounter;
    [JsonProperty] private ActorStateChangerData _stateChanger;

    [JsonConstructor]
    private MeleeAttackStrategy()
    {
    }
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _meleeAttackCost = data.MeleeAttackCost;
        ControlledNpc = controlledNpc;
        _stateChanger = data.ActorStateChangerData;
        if (data.Cooldown > 0)
        {
            _meleeAttackCounter = new Counter(ControlledNpc.Level, data.Cooldown, 0);
            ControlledNpc.Died += _meleeAttackCounter.RemoveCounter;
        }
    }
    
    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if(_meleeAttackCounter is not null)
            ControlledNpc.Died += _meleeAttackCounter.RemoveCounter;
    }

    public override ActorActionInfo TryAct()
    {
        if (ControlledNpc.CurrentTarget is null || !(Math.Abs(ControlledNpc.X - ControlledNpc.CurrentTarget.X) < 2
                && Math.Abs(ControlledNpc.Y - ControlledNpc.CurrentTarget.Y) < 2
                && (_meleeAttackCounter is null || _meleeAttackCounter.FinishedCounting)))
            return null;

        bool hit = ControlledNpc.CurrentTarget.Hit
            (UtilityFunctions.CalculateMeleeDamage(_minDamage, _maxDamage, ControlledNpc.Strength), false, false);
        if (hit && _stateChanger is not null)
        {
            DataHolder.ActorStateChangers[_stateChanger.Type]
                (ControlledNpc.CurrentTarget, _stateChanger.Power, _stateChanger.Duration);
        }

        ControlledNpc.Energy -= _meleeAttackCost;
        _meleeAttackCounter?.ResetTimer();
        return new ActorActionInfo(ActorAction.MeleeAttack,
            new List<(int, int)> {(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y)});
    }
}