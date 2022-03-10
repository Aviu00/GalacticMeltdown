using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    public string LootTableId { get; init; }
    public Enemy(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level, 
        SortedSet<Behavior> behaviors) : base(maxHp, maxEnergy, dex, def, viewRange, x, y, level, behaviors)
    {
        //temporary stuff
        SymbolData = ('W', ConsoleColor.Red);
        BgColor = null;
    }

    public override void TakeAction()
    {
        if (CurrentTarget is null)
        {
            foreach(var target in Targets)
            {
                if (SeePoint(target.X, target.Y))
                {
                    CurrentTarget = target;
                    break;
                }
            }
        }
        base.TakeAction();
    }
}