using System.Linq;
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
    private int _cost;
    
    public MeleeAttackStrategy(MeleeAttackStrategyData data, Npc controlledNpc, int minDamage, int maxDamage, 
        int cooldown, int cost) : base(data.Priority ?? DefaultPriority)
    {
        _minDamage = minDamage;
        _maxDamage = maxDamage;
        _cooldown = cooldown;
        _cost = cost;
        ControlledNpc = controlledNpc;
    }
    
    public override bool TryAct()
    {
        if (Algorithms.GetPointsOnSquareBorder(ControlledNpc.X, ControlledNpc.Y, 1).
            Contains((ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y)))
        {
            // there is temporary damage value is MaxDamage
            // TODO: write function of random damage
            ControlledNpc.CurrentTarget.Hit(ControlledNpc, _maxDamage);
            return true;
        }

        return false;
    }
}