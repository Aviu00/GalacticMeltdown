using System.Dynamic;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class RangeEnemy : Enemy, IMoveStrategy, IAttackStrategy
{
    public MoveStrategy MoveStrategy { get; set; }
    public AttackStrategy AttackStrategy { get; set; }
    
    public RangeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
        AttackStrategy = new AttackStrategy(this, Map);
    }
    
    protected override void TakeAction(int movePoints)
    {
        //calculate actions
        //temporary actions
        
        MoveStrategy.Move(-1, 0);
    }
}