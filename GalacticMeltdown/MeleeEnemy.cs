using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }

    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
    }

    protected override void TakeAction(int movePoints)
    {
        this.Energy = movePoints;
        bool flag = true;
        IEnumerable PathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y); 
        // check if enemy see player
        foreach (var coords in Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y))
        {
            if (!Map.GetTile(coords.x, coords.y).IsWalkable)
            {
                flag = false;
            }
        }

        int DiffX = 0, DiffY = 0;
        // acts only if see enemy
        if (flag == true)
        {
            int CostOfWay = 0;
            foreach (var coords in Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y))
            {
                CostOfWay += (Map.GetTile(coords.x, coords.y)).TileMoveCost;
                if (CostOfWay < this.Energy)
                {
                    continue;
                }
                else
                {
                    DiffX = coords.x - this.X;
                    DiffY = coords.y - this.Y;
                    break;
                }
            }
            MoveStrategy.Move(DiffX, DiffY);
            // string for test
            //Console.WriteLine("Enenmy №-" + GetHashCode().ToString()+ "|||" + DiffX.ToString() + ":" + DiffY.ToString());
        }

    }
}