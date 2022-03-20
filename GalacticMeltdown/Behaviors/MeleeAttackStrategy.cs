using System.Linq;
using System;
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
    [JsonIgnore] private Counter meleeAtackCounter;
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _cooldown = data.Cooldown;
        _meleeAttackCost = data.MeleeAttackCost;
        ControlledNpc = controlledNpc;
        if (_cooldown > 0)
        {
            meleeAtackCounter = new Counter(ControlledNpc.Level, _cooldown, 0);
        }
    }

    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if (Algorithms.GetPointsOnSquareBorder(ControlledNpc.X, ControlledNpc.Y, 1).
                Contains((ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y)) &&
            (meleeAtackCounter is null || meleeAtackCounter.FinishedCounting))
        {
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
            ControlledNpc.Energy -= _meleeAttackCost;
            if (meleeAtackCounter is not null)
                meleeAtackCounter.ResetTimer();
        }

        return false;
    }
    // TODO: make advanced random damage 
    private int RandomDamage(int minDamage, int maxDamage)
    {
        return Random.Shared.Next(minDamage, maxDamage + 1);
    }
}