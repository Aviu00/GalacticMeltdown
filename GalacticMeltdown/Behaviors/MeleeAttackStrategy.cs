using System.Linq;
using System;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class MeleeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "MeleeAttack";
    [JsonProperty] private const int DefaultPriority = 20;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _cooldown;
    [JsonProperty] private readonly int _meleeAttackCost;
    [JsonIgnore] private Counter _meleeAttackCounter;
    private readonly Counter _meleeAtackCounter;

    [JsonConstructor]
    private MeleeAttackStrategy()
    {
    }
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _cooldown = data.Cooldown;
        _meleeAttackCost = data.MeleeAttackCost;
        ControlledNpc = controlledNpc;
        if (_cooldown > 0)
        {
            _meleeAtackCounter = new Counter(ControlledNpc.Level, _cooldown, 0);
        }
    }
    
    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        ControlledNpc.Died += _meleeAtackCounter.RemoveCounter;
    }

    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if (Algorithms.GetPointsOnSquareBorder(ControlledNpc.X, ControlledNpc.Y, 1).
                Contains((ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y)) &&
            (_meleeAtackCounter is null || _meleeAtackCounter.FinishedCounting))
        {
            ControlledNpc.CurrentTarget.Hit(RandomDamage(_minDamage, _maxDamage), false, false);
            ControlledNpc.Energy -= _meleeAttackCost;
            if (_meleeAtackCounter is not null)
                _meleeAtackCounter.ResetTimer();
        }

        return false;
    }
    // TODO: make advanced random damage 
    private int RandomDamage(int minDamage, int maxDamage)
    {
        return Random.Shared.Next(minDamage, maxDamage + 1);
    }
}