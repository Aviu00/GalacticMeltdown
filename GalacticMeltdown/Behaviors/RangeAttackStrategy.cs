using System;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class RangeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "RangeAttack";
    private const int DefaultPriority = 10;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _cooldown;
    [JsonProperty] private readonly int _rangeAttackCost;
    [JsonProperty] private readonly int _attackRange;
    [JsonProperty] private Counter _rangeAttackCounter;

    [JsonConstructor]
    private RangeAttackStrategy()
    {
    }

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
            _rangeAttackCounter = new Counter(ControlledNpc.Level, _cooldown, 0);
            ControlledNpc.Died += _rangeAttackCounter.RemoveCounter;
        }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if (_rangeAttackCounter is not null)
            ControlledNpc.Died += _rangeAttackCounter.RemoveCounter;
    }

    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if (!CanAttack()) return false;
        ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
        ControlledNpc.Energy -= _rangeAttackCost;
        if (_rangeAttackCounter is not null)
            _rangeAttackCounter.ResetTimer();
        return true;
    }

    // TODO: make advanced random damage 
    private int RandomDamage(int minDamage, int maxDamage)
    {
        return Random.Shared.Next(minDamage, maxDamage + 1);
    }

    private bool CanAttack()
    {
        // can't attack due to cooldown
        if (_rangeAttackCounter is not null && !_rangeAttackCounter.FinishedCounting)
            return false;
        // can't attack due to range
        if (UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y,
                ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y) > _attackRange)
            return false;
        foreach (var coord in Algorithms.BresenhamGetPointsOnLine(ControlledNpc.X, ControlledNpc.Y,
                     ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y))
        {
            // shoot to wall (or etc) or friendly fire
            if (ControlledNpc.Level.GetNonTileObject(coord.x, coord.y) is Enemy &&
                 coord != (ControlledNpc.X, ControlledNpc.Y) ||
                !ControlledNpc.Level.GetTile(coord.x, coord.y).IsWalkable)
            {
                return false;
            }
        }
        return true;
    }
}