using GalacticMeltdown.LevelRelated;
using System.Collections.Generic;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private const int DefaultPriority = 10;
    private readonly Level _level;
    private (int x, int y)? _wantsToGoTo;
    private LinkedListNode<(int x, int y)> _nextPathCellNode;

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

    private void SetPathTo(int x, int y)
    {
        _wantsToGoTo = (x, y);
        var path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, x, y, GetNeighbors);
        if (path is null || path.Count < 3)
        {
            _nextPathCellNode = null;
            return;
            // remove start and finish points to simplify implementation
        }

        path.RemoveFirst();
        path.RemoveLast();

        _nextPathCellNode = path.First;
    }

    public override bool TryAct()
    {
        // setting wantsToGoTo point
        if (ControlledNpc.CurrentTarget is not null &&
            _level.GetTile(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y).IsWalkable)
        {
            if (_wantsToGoTo != (ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y))
                SetPathTo(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y);
        }
        else if (_nextPathCellNode is null)
        {
            // there is place for idle movement
            return false;
        }
        else if (!(_level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is null &&
                   _level.GetTile(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y).IsWalkable))
        {
            SetPathTo(_wantsToGoTo!.Value.x, _wantsToGoTo!.Value.y);
        }

        if (_nextPathCellNode is null) return false;
        ControlledNpc.MoveNpcTo(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        _nextPathCellNode = _nextPathCellNode.Next;
        if (_nextPathCellNode is null) _wantsToGoTo = null;
        return true;
    }
}