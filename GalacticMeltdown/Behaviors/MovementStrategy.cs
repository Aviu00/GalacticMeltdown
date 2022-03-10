using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private const int DefaultPriority = 10;
    private readonly Level _level;
    private (int x, int y)? _wantsToGoTo = null;

    public MovementStrategy(Level level, int? priority = null)
    {
        _priority = priority ?? DefaultPriority;
        _level = level;
    }
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
            neighboursWithMoveCosts.Add(((xi, yi), _level.GetTile(xi,yi).TileMoveCost));
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
                if (_level.GetTile(nextDot.Item1, nextDot.Item2).IsWalkable &&
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
    public override bool TryAct()
    {
        if(Target.CurrentTarget != null)
        {
            List<(int, int)> path = AStar(Target.X, Target.Y, Target.CurrentTarget.X, Target.CurrentTarget.Y);
            foreach ((int x, int y) coords in path)
            {
                //Target.MoveNpcTo(coords.x - Target.X, coords.y - Target.Y);
                Target.MoveNpcTo(coords.x, coords.y);
            }
        }
        else
        {
            // here is place for moving without CurrentTarget
            return false;
        }
        /*if (Target == null)
        {
            return false;
        }*/

        //if CurrentTarget is not null, then move towards CurrentTarget;
        //else if _wantsToGoTo is not null, then move there; else Idle movement
        return true;
    }
}