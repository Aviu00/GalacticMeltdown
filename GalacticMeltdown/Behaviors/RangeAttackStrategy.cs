using System;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class RangeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "RangeAttack";
    [JsonProperty] private const int DefaultPriority = 20;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _cooldown;
    [JsonProperty] private readonly int _rangeAttackCost;
    [JsonProperty] private readonly int _attackRange;
    [JsonIgnore] private Counter rangeAtackCounter;
    
    public RangeAttackStrategy(RangeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _cooldown = data.Cooldown;
        _rangeAttackCost = data.RangeAttackCost;
        _attackRange = data.AttackRange;
        ControlledNpc = controlledNpc;
        if (_cooldown > 0)
        {
            rangeAtackCounter = new Counter(ControlledNpc.Level, _cooldown, 0);   
        }
    }
    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        // if there is nothing what can stop projectile
        if (UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y,
                ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y) < _attackRange &&
            Algorithms.BresenhamGetPointsOnLine(ControlledNpc.X, ControlledNpc.Y, ControlledNpc.CurrentTarget.X,
                ControlledNpc.CurrentTarget.Y).All(coord => ControlledNpc.Level.GetTile(coord.x, coord.y).IsWalkable) &&
            (rangeAtackCounter is null || rangeAtackCounter.FinishedCounting))
        {
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
            ControlledNpc.Energy -= _rangeAttackCost;
            if (rangeAtackCounter is not null)
                rangeAtackCounter.ResetTimer();
            return true;
        }
        return false;
    }
    // TODO: make advanced random damage 
    private int RandomDamage(int minDamage, int maxDamage)
    {
        return Random.Shared.Next(minDamage, maxDamage + 1);
    }
}