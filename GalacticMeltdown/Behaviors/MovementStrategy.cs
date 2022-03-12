using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private const int DefaultPriority = 10;
    private readonly Level _level;
    private (int x, int y)? _wantsToGoTo;

    public MovementStrategy(Level level, int? priority = null)
    {
        _priority = priority ?? DefaultPriority;
        _level = level;
    }
    private List<((int, int), int)> GetNeighbors(int x, int y)
    {
        List<((int, int), int)> neighboursWithMoveCosts = new List<((int, int), int)>();
        foreach ((int xi, int yi) in Algorithms.GetPointsOnSquareBorder(x, y, 1))
        {
            if (_level.GetTile(xi, yi).IsWalkable)
                /*(_level.GetNonTileObject(nextDot.Item1, nextDot.Item2) is not null &&*/
            {
                neighboursWithMoveCosts.Add(((xi, yi), _level.GetTile(xi,yi).TileMoveCost));
            }
        }
        return neighboursWithMoveCosts;
    }
    public override bool TryAct()
    {
        // setting wantsToGoTo point
        if(Target.CurrentTarget is not null)
        {
            _wantsToGoTo = (Target.CurrentTarget.X, Target.CurrentTarget.Y);
        }
        else
        {
            if (_wantsToGoTo == (Target.X, Target.Y) || _wantsToGoTo is null)
            {
                return false;
            }
        }

        IEnumerable<(int, int)> path = Algorithms.AStar(Target.X, Target.Y, _wantsToGoTo.Value.x, 
            _wantsToGoTo.Value.y, GetNeighbors);
        if (path is null)
        {
            return false;
        }
        foreach ((int x, int y) coords in path)
        {
            if (Target.Energy > 0 && coords != _wantsToGoTo)
            {
                Target.MoveNpcTo(coords.x, coords.y);
                Target.Energy -= _level.GetTile(coords.x, coords.y).TileMoveCost;
            }
            else
            {
                // there is place for Idle movement
                break;
            }
        }
        return true;
        //if CurrentTarget is not null, then move towards CurrentTarget;
        //else if _wantsToGoTo is not null, then move there; else Idle movement
    }
}