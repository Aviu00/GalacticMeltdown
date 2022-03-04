using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }

    public MeleeEnemy(int x, int y, Level level, Player player) : base(x, y, level, player)
    {
        MoveStrategy = new MoveStrategy(this, Level);
    }

    protected override void TakeAction(int movePoints)
    {
        //calculate actions
        
        MoveStrategy.Move(0, 1);
    }
}