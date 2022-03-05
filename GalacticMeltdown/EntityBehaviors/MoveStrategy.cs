using System.Collections.Generic;
using System;
namespace GalacticMeltdown.EntityBehaviors;

public class MoveStrategy : Behavior
{
    private readonly Map _map;
    public MoveStrategy(Enemy target, Map map)
    {
        Target = target;
        _map = map;
    }
    // temporary Distance realisation 
    private static int GetDistance(int x0, int y0, int x1, int y1)
    {
        return (int)(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));
    }
    private List<((int, int), int)> GetNeighbors(int x, int y)
    {
        List<(int, int)> neighbours = new List<(int, int)>
            {(x, 1 + y), (1 + x, 1 + y), (1 + x, y), (1 + x, y - 1), (x, y - 1), (x - 1, y - 1), (x - 1 ,y), (x - 1,y + 1)};
        List<((int, int), int)> neighboursWithMoveCosts = new List<((int, int), int)>();
        foreach ((int xi, int yi) in neighbours)
        {
            neighboursWithMoveCosts.Add(((xi, yi), _map.GetTile(xi,yi).TileMoveCost));
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
                if (_map.GetTile(nextDot.Item1, nextDot.Item2).IsWalkable &&
                    (!localCost.TryGetValue(nextDot, out int oldCost) || newCost < oldCost))
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
    public void Move(int relX, int relY)
    {
        var tile = _map.GetTile(Target.X + relX, Target.Y + relY);
        var entity = _map.GetEntity(Target.X + relX, Target.Y + relY);
        if (tile.IsWalkable && entity == null)
        {
            Target.X += relX;
            Target.Y += relY;
            Target.Energy -= tile.TileMoveCost;
        }
        _map.UpdateEnemyPosition(Target, Target.X - relX, Target.Y - relY);
    }

}