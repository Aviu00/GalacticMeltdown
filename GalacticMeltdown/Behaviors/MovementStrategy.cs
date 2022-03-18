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
    private LinkedList<(int x, int y)> _chunkPath;

    public MovementStrategy(Level level, int priority = DefaultPriority)
    {
        _priority = priority;
        _level = level;
    }

    private IEnumerable<(int, int, int)> GetNeighbors(int x, int y)
    {
        foreach ((int xi, int yi) in Algorithms.GetPointsOnSquareBorder(x, y, 1))
        {
            Tile tile = _level.GetTile(xi, yi);
            if (tile is null || !tile.IsWalkable || !_chunkPath.Contains(Level.GetChunkCoords(xi, yi))) continue;
            yield return (xi, yi, tile.MoveCost);
        }
    }

    private void SetPathTo(int x, int y)
    {
        _chunkPath = _level.GetPathBetweenChunks(ControlledNpc.X, ControlledNpc.Y, x, y);
        if (_chunkPath is null)
        {
            _wantsToGoTo = null;
            _nextPathCellNode = null;
            return;
        }
        _wantsToGoTo = (x, y);
        var path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, x, y, GetNeighbors);
        if (path is null || path.Count < 3)
        {
            _nextPathCellNode = null;
            return;
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
            // TODO: idle movement
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