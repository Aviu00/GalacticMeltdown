using System;
using System.Collections;
using System.Collections.Generic;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }

    private int _lastSeenPlayerX;
    private int _lastSeenPlayerY;

    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
        _lastSeenPlayerX = X;
        _lastSeenPlayerY = Y;
    }

    protected override void UpdateLastSeenPosition()
    {
        if (SeePlayer())
        {
            _lastSeenPlayerX = Player.X;
            _lastSeenPlayerY = Player.Y;
        }
        // for tests
        //Console.WriteLine(this.GetHashCode() + ":" + SeePlayer().ToString());
    }

    private bool SeePlayer()
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
    private void MoveToGoal(IEnumerable pathToPlayer)
    {
        int costOfWay = 0;
        int diffX = 0, diffY = 0;
        foreach ((int x, int y) coords in pathToPlayer)
        {
            costOfWay += (Map.GetTile(coords.x, coords.y)).TileMoveCost;
            if (costOfWay <= this.Energy)
            {
                continue;
            }
            else
            {
                diffX = coords.x - this.X;
                diffY = coords.y - this.Y;
                break;
            }
        }
        MoveStrategy.Move(diffX, diffY);
    }
    
    // temporary Distance realisation 
    private static int GetDistance(int x0, int y0, int x1, int y1)
    {
        return (int)(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));
    }
    
    private List<((int, int), int)> GetNeighbors(int x, int y)
    {
        List<(int, int)> neighbours = new List<(int, int)>
            {(0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1 ,0), (-1, 1)};
        List<((int, int), int)> neighboursWithMoveCosts = new List<((int, int), int)>();
        foreach ((int xi, int yi) in neighbours)
        {
            neighboursWithMoveCosts.Add(((xi, yi), Map.GetTile(xi,yi).TileMoveCost));
        }
        return neighboursWithMoveCosts;
    }

    public  List<(int, int)> AStar(int x0, int y0, int x1, int y1)
    {
        List<(int, int)> path = new List<(int, int)>();
        PriorityQueue<(int, int), int> priorQueue = new PriorityQueue<(int, int), int>();
        Dictionary<(int, int), (int, int)?> cameFrom = new Dictionary<(int, int), (int, int)?>();
        Dictionary<(int, int), int> localCost = new Dictionary<(int, int), int>();
        bool finishedFindingWay = false;
        priorQueue.Enqueue((x0, y0), 0);
        cameFrom[(x0, y0)] = null;
        localCost[(x0, y0)] = 0;
        while (priorQueue.Count > 0)
        {
            (int, int) currentDot = priorQueue.Dequeue();
            if (currentDot == (x1, y1))
            {
                finishedFindingWay = true;
                break;
            }

            foreach (((int x, int y), int moveCost) in GetNeighbors(currentDot.Item1, currentDot.Item2))
            {
                (int, int) nextDot = (x, y);
                int newCost = moveCost + localCost[currentDot];
                if (!localCost.TryGetValue(nextDot, out int oldCost) || newCost < oldCost)
                {
                    localCost[nextDot] = newCost;
                    int priority = newCost + GetDistance(nextDot.Item1, nextDot.Item2, x1, y1);
                    priorQueue.Enqueue(nextDot, priority);
                    cameFrom[nextDot] = currentDot;
                }
            }
        }

        if (finishedFindingWay)
        {
            (int, int) goal = (x1, y1);
            path.Add(goal);
            while (goal != (x0, y0))
            {
                goal = (cameFrom[goal].Value.Item1, cameFrom[goal].Value.Item2);
                path.Add(goal);
            }
            path.Reverse();
        }
        return path;
    }
    
    protected override void TakeAction(int movePoints)
    {
        this.Energy = movePoints;
        IEnumerable pathToPlayer;
        // acts only if see enemy
        if (SeePlayer())
        {
            pathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y);
            MoveToGoal(pathToPlayer);
            // string for test
            //Console.WriteLine("Enenmy №-" + GetHashCode().ToString()+ "|||" + DiffX.ToString() + ":" + DiffY.ToString());
        }
        else
        {
            pathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, _lastSeenPlayerX, _lastSeenPlayerY);
            MoveToGoal(pathToPlayer);
        }
        //Console.WriteLine("Enemy №-" + GetHashCode().ToString()+ "|||" + (_lastSeenPlayerX - this.X).ToString() + ":" + (_lastSeenPlayerY - this.Y).ToString());
    }
}