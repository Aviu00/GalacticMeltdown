using System;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Actors.Enemies;

public class MeleeEnemy : Enemy
{
    public MeleeEnemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        MoveStrategy = new MoveStrategy(this, Level);
        SymbolData = ('W', ConsoleColor.Red);
        BgColor = null;
    }

    protected override void TakeAction()
    {
        //calculate actions
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}