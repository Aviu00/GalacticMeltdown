using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class RangeAttackStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "RangeAttack";
    private const int DefaultPriority = 15;
    [JsonProperty] private readonly int _minDamage;
    [JsonProperty] private readonly int _maxDamage;
    [JsonProperty] private readonly int _cooldown;
    [JsonProperty] private readonly int _rangeAttackCost;
    [JsonProperty] private readonly int _attackRange;
    [JsonProperty] private readonly int _spread;
    [JsonProperty] private Counter _rangeAttackCounter;
    [JsonProperty] private IEnumerable<ActorStateChangerData> _stateChangers;

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
        _spread = data.Spread;
        ControlledNpc = controlledNpc;
        _stateChangers = data.ActorStateChangerData;
        if (_cooldown <= 0) return;
        _rangeAttackCounter = new Counter(ControlledNpc.Level, _cooldown, 0);
        ControlledNpc.Died += _rangeAttackCounter.RemoveCounter;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if (_rangeAttackCounter is not null)
            ControlledNpc.Died += _rangeAttackCounter.RemoveCounter;
    }

    public override ActorActionInfo TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return null;
        
        if (!CanAttack()) return null;
        // Failed to shoot
        if (!UtilityFunctions.Chance(UtilityFunctions.RangeAttackHitChance
            (UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y,
                ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y), _spread)))
            return new ActorActionInfo(ActorAction.Shoot, new List<(int, int)>());
        
        int damage = RandomDamage(_minDamage, _maxDamage);
        ControlledNpc.CurrentTarget.Hit(damage, true, false);
        UtilityFunctions.ApplyStateChangers(_stateChangers, ControlledNpc.CurrentTarget);
        ControlledNpc.Energy -= _rangeAttackCost;
        _rangeAttackCounter?.ResetTimer();
        return new ActorActionInfo(ActorAction.Shoot,
            Algorithms.BresenhamGetPointsOnLine(ControlledNpc.X, ControlledNpc.Y, ControlledNpc.CurrentTarget.X,
                    ControlledNpc.CurrentTarget.Y)
                .Skip(1)
                .ToList());
    }

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
        // Shouldn't happen, but just in case
        if (ControlledNpc.X == ControlledNpc.CurrentTarget.X && ControlledNpc.Y == ControlledNpc.CurrentTarget.Y)
            return false;
        foreach ((int x, int y) in Algorithms.BresenhamGetPointsOnLine(ControlledNpc.X, ControlledNpc.Y,
                     ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y).Skip(1))
        {
            // Prevent shooting at walls or non-targeted actors
            if (ControlledNpc.Level.GetTile(x, y) is {IsWalkable: false}
                || ControlledNpc.Level.GetNonTileObject(x, y) is Actor objectInTheWay
                && !ControlledNpc.Targets.Contains(objectInTheWay))
                return false;
        }
        return true;
    }
}