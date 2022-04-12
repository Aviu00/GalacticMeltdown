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
    [JsonProperty] private IEnumerable<ActorStateChangerData> _stateChangers;

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
        _stateChangers = data.ActorStateChangerData;
        if (data.Cooldown <= 0) return;
        _meleeAttackCounter = new Counter(ControlledNpc.Level, data.Cooldown, 0);
        ControlledNpc.Died += _meleeAttackCounter.RemoveCounter;
    }
    
    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if(_meleeAttackCounter is not null)
            ControlledNpc.Died += _meleeAttackCounter.RemoveCounter;
    }

    public override ActorActionInfo TryAct()
    {
        if (ControlledNpc.CurrentTarget is null || _meleeAttackCounter is not null
            && !_meleeAttackCounter.FinishedCounting ||
            !UtilityFunctions.ObjectInSquareArea
                (ControlledNpc.X, ControlledNpc.Y, ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y, 1))
            return null;

        bool hit = ControlledNpc.CurrentTarget.Hit
            (UtilityFunctions.CalculateMeleeDamage(_minDamage, _maxDamage, ControlledNpc.Strength), false, false);
        if (hit)
            UtilityFunctions.ApplyStateChangers(_stateChangers, ControlledNpc.CurrentTarget);

        ControlledNpc.Energy -= _meleeAttackCost;
        _meleeAttackCounter?.ResetTimer();
        return new ActorActionInfo(hit ? ActorAction.MeleeAttackHit : ActorAction.MeleeAttackMissed,
            new List<(int, int)> {(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y)});
    }
}