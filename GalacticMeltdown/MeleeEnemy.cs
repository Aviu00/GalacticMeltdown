using System.Dynamic;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy, IAttackStrategy
{
    public MoveStrategy MoveStrategy { get; set; }
    public AttackStrategy AttackStrategy { get; set; }

    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
        AttackStrategy = new AttackStrategy(this, Map);
    }

    protected override void TakeAction(int movePoints)
    {
        //calculate actions
        
        MoveStrategy.Move(0, 1);
    }
}