using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }

    protected int LastSeenPlayerX;
    protected int LastSeenPlayerY;

    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
        LastSeenPlayerX = X;
        LastSeenPlayerY = Y;
    }

    protected bool SeePlayer()
    {
        bool flag = true;
        foreach (var coords in Algorithms.BresenhamGetPointsOnLine(this.X, this.Y, Player.X, Player.Y))
        {
            if (!Map.GetTile(coords.x, coords.y).IsWalkable)
            {
                flag = false;
            }
        }
        return flag;
    }

    // makes many moves
    protected void MoveToGoal(IEnumerable PathToPlayer)
    {
        int CostOfWay = 0;
        int DiffX = 0, DiffY = 0;
        foreach ((int x, int y) coords in PathToPlayer)
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
    }
    
    protected override void TakeAction(int movePoints)
    {
        this.Energy = movePoints;
        IEnumerable PathToPlayer;
        // acts only if see enemy
        if (SeePlayer())
        {
            PathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y);
            int CostOfWay = 0;
            LastSeenPlayerX = Player.X;
            LastSeenPlayerY = Player.Y;
            MoveToGoal(PathToPlayer);
            // string for test
            //Console.WriteLine("Enenmy №-" + GetHashCode().ToString()+ "|||" + DiffX.ToString() + ":" + DiffY.ToString());
        }
        else
        {
            PathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, LastSeenPlayerX, LastSeenPlayerY);
            MoveToGoal(PathToPlayer);
        }
        Console.WriteLine("Enenmy №-" + GetHashCode().ToString()+ "|||" + (LastSeenPlayerX - this.X).ToString() + ":" + (LastSeenPlayerY - this.Y).ToString());
    }
}