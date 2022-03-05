using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy
{
    public MeleeEnemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        MoveStrategy = new MoveStrategy(this, Level);
    }

    protected override void TakeAction(int movePoints)
    {
        //calculate actions
        
        MoveStrategy.Move(0, 1);
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
        
    }
}