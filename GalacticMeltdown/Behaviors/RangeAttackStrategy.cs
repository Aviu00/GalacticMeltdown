using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class RangeAttackStrategy : Behavior
{
    private const int DefaultPriority = 20;
    private readonly int _minDamage;
    private readonly int _maxDamage;
    private readonly int _cooldown;
    private readonly int _rangeAttackCost;
    private readonly int _attackRange;
    private Counter rangeAtackCounter;
    
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
            rangeAtackCounter = new Counter(ControlledNpc.Level, _cooldown);   
        }
    }
    public override bool TryAct()
    {
        return false;
    }
}