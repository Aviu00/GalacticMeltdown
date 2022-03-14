using GalacticMeltdown.LevelRelated;
using System.Collections.Generic;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private const int DefaultPriority = 10;
    private readonly Level _level;
    private (int x, int y)? _wantsToGoTo;
    private LinkedListNode<(int x, int y)> _currentPathNode;
    private LinkedList<(int x, int y)> _path;

    public MovementStrategy(Level level, int priority = DefaultPriority)
    {
        _priority = priority;
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
                neighboursWithMoveCosts.Add(((xi, yi), _level.GetTile(xi, yi).MoveCost));
            }
        }

        return neighboursWithMoveCosts;
    }

    public override bool TryAct()
    {
        // setting wantsToGoTo point
        // second condition for no clip use
        if (ControlledNpc.CurrentTarget is not null && _wantsToGoTo != (ControlledNpc.X, ControlledNpc.Y)
            && !_level.GetTile(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y).IsWalkable)
        {
            _wantsToGoTo = (ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y);
            _path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, _wantsToGoTo.Value.x, _wantsToGoTo.Value.y,
                GetNeighbors);
            
            if (_path is null || _path.Count < 2) return false;
            
            // remove start and finish points to simplify implementation
            _path.RemoveFirst();
            _path.RemoveLast();
            
            _currentPathNode = _path.First;
        }
        else
        {
            if (ControlledNpc.CurrentTarget is null)
            {
                // there is place for idle movement logic
                return false;
            }

            if (_level.GetTile(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y).IsWalkable)
            {
                return false;
            }

            if (_wantsToGoTo == (ControlledNpc.X, ControlledNpc.Y))
            {
                _currentPathNode = _currentPathNode.Next;
            }
        }

        if (_currentPathNode is not null)
        {
            ControlledNpc.MoveNpcTo(_currentPathNode.Value.x, _currentPathNode.Value.y);
        }
        else
        {
            return false;
        }
        //LinkedList<(int, int)> path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, _wantsToGoTo.Value.x, 
        //    _wantsToGoTo.Value.y, GetNeighbors);

        /*foreach ((int x, int y) coords in _path)
        {
            if (coords != _wantsToGoTo)
            {
                ControlledNpc.MoveNpcTo(coords.x, coords.y);
                ControlledNpc.Energy -= _level.GetTile(coords.x, coords.y).MoveCost;
            }
        }*/
        return true;
        //if CurrentTarget is not null, then move towards CurrentTarget;
        //else if _wantsToGoTo is not null, then move there; else Idle movement
    }
}