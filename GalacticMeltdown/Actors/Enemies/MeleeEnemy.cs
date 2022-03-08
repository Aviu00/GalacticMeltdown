using System;
using System.Collections.Generic;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Actors.Enemies;

public class MeleeEnemy : Enemy
{
    protected override List<Behavior> Behaviors { get; }

    public MeleeEnemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        Behaviors = new List<Behavior> {new MovementStrategy(this, Level)};
        SymbolData = ('W', ConsoleColor.Red);
        BgColor = null;
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}