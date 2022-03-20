using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MeleeAttackStrategy : Behavior
{
    private const int DefaultPriority = 20;
    private int _minDamage;
    private readonly int _maxDamage;
    private int _cooldown;
    private int _meleeAttackCost;
    
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = data.MinDamage;
        _maxDamage = data.MaxDamage;
        _cooldown = data.Cooldown;
        _meleeAttackCost = data.MeleeAttackCost;
        ControlledNpc = controlledNpc;
    }
    
    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is null)
            return false;
        if (UtilityFunctions.GetDistance(ControlledNpc.X, ControlledNpc.Y, ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y) < 2)
        {
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, RandomDamage(_minDamage, _maxDamage));
            ControlledNpc.Energy -= _meleeAttackCost;
            return true;
        }

        return false;
    }

    // TODO: make advanced random damage 
    private int RandomDamage(int minDamage, int maxDamage)
    {
        if (UtilityFunctions.Chance(10))
        {
            return 0;
        }
        else
        {
            return Random.Shared.Next(minDamage, maxDamage);
        }
    }
}