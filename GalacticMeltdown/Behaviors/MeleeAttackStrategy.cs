using System;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class MeleeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "MeleeAttack";
    [JsonProperty] private const int DefaultPriority = 15;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _meleeAttackCost;
    [JsonIgnore] private Counter _meleeAttackCounter;

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

    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if ( Math.Abs(ControlledNpc.X - ControlledNpc.CurrentTarget.X) < 2 && 
             Math.Abs(ControlledNpc.Y - ControlledNpc.CurrentTarget.Y) < 2 &&
            (_meleeAttackCounter is null || _meleeAttackCounter.FinishedCounting))
        {
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
            ControlledNpc.Energy -= _meleeAttackCost;
            if (_meleeAttackCounter is not null)
                _meleeAttackCounter.ResetTimer();
            return true;
        }

        return false;
    }
    private int RandomDamage(int minDamage, int maxDamage)
    {
        return Random.Shared.Next(minDamage, maxDamage + 1);
    }
}