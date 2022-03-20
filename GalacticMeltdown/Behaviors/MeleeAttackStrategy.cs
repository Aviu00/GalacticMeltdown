using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MeleeAttackStrategy : Behavior
{
    private const int DefaultPriority = 20;
    private readonly int _minDamage;
    private readonly int _maxDamage;
    private readonly int _cooldown;
    private readonly int _meleeAttackCost;
    private Counter meleeAtackCounter;
    
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _cooldown = data.Cooldown;
        _meleeAttackCost = data.MeleeAttackCost;
        ControlledNpc = controlledNpc;
        if (_cooldown > 0)
        {
            meleeAtackCounter = new Counter(ControlledNpc.Level, _cooldown);   
        }
    }
    
    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if (UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y, ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y) < 2 &&
            (meleeAtackCounter is null || meleeAtackCounter.FinishedCounting))
        {
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
            ControlledNpc.Energy -= _meleeAttackCost;
            if (meleeAtackCounter is not null)
                meleeAtackCounter.ResetTimer();
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