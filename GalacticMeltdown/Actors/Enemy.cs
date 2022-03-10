using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;

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
        if (SeePoint(CurrentTarget.X, CurrentTarget.Y))
        {
            int tempX = X;
            int tempY = Y;
            int tempEnergy = this.Energy;
            MovementStrategy currentStrategy = new MovementStrategy(Level);
            foreach (var coord in currentStrategy.AStar(this.X, this.Y, CurrentTarget.X,CurrentTarget.Y))
            {
                tempEnergy -= Level.GetTile(coord.Item1, coord.Item2).TileMoveCost;
                if (tempEnergy < 0)
                {
                    tempX = coord.Item1;
                    tempY = coord.Item2;
                    break;
                }
            }
            MoveTo(tempX, tempY);
        }
        //calculate target (CurrentTarget)
        base.TakeAction();
    }
}