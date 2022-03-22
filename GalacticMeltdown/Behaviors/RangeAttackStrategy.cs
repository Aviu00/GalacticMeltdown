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
    [JsonProperty] private readonly int _spread;
    [JsonProperty] private Counter _rangeAttackCounter;
    [JsonProperty] private ActorStateChangerDataExtractor.ActorStateChangerData _stateChanger;

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
        _stateChanger = data.ActorStateChangerData;
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
        // if there is nothing what can stop projectile
        if (!CanAttack()) return false;
        var damage = 0;
        if (UtilityFunctions.Chance(UtilityFunctions.CalculateChanceToHitForRangeEnemy
            ((int) UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y,
                ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y), _spread)))
            damage = RandomDamage(_minDamage, _maxDamage);
        ControlledNpc.CurrentTarget.Hit(damage, true, false);
        if (_stateChanger is not null)
        {
            DataHolder.ActorStateChangers[_stateChanger.Type]
                (ControlledNpc.CurrentTarget, _stateChanger.Power, _stateChanger.Duration);
        }
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