using System;
using GalacticMeltdown.LevelRelated;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private const int DefaultPriority = 10;
    private Level Level => ControlledNpc.Level;
    private (int x, int y)? _wantsToGoTo;
    private LinkedListNode<(int x, int y)> _nextPathCellNode;
    private LinkedList<(int x, int y)> _chunkPath;

    public MovementStrategy(MovementStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        ControlledNpc = controlledNpc;
    }

    private IEnumerable<(int, int, int)> GetNeighbors(int x, int y)
    {
        foreach ((int xi, int yi) in Algorithms.GetPointsOnSquareBorder(x, y, 1))
        {
            Tile tile = Level.GetTile(xi, yi);
            if (tile is null || !tile.IsWalkable || !_chunkPath.Contains(Level.GetChunkCoords(xi, yi)) ||
                !(!(x == ControlledNpc.X && y == ControlledNpc.Y) || Level.GetNonTileObject(xi, yi) is null))
                continue;
            yield return (xi, yi, tile.MoveCost);
        }
    }

    private void SetPathTo(int x, int y)
    {
        _chunkPath = Level.GetPathBetweenChunks(ControlledNpc.X, ControlledNpc.Y, x, y);
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
            Level.GetTile(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y).IsWalkable)
        {
            if (_wantsToGoTo != (ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y))
                SetPathTo(ControlledNpc.CurrentTarget.X, ControlledNpc.CurrentTarget.Y);
        }
        else if (_nextPathCellNode is null)
        {
            CalculateIdleMovementPath();
        }
        else if (!(Level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is null &&
                   Level.GetTile(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y).IsWalkable))
        {
            SetPathTo(_wantsToGoTo!.Value.x, _wantsToGoTo!.Value.y);
        }

        if (_nextPathCellNode is null || 
            !Level.GetTile(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y).IsWalkable) return false;
        ControlledNpc.MoveNpcTo(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        _nextPathCellNode = _nextPathCellNode.Next;
        if (_nextPathCellNode is null) _wantsToGoTo = null;
        return true;
    }
    private void CalculateIdleMovementPath()
    {
        _wantsToGoTo = null;
        (int x, int y) = Level.GetChunkCoords(ControlledNpc.X, ControlledNpc.Y);
        var neighboringChunks = Level.GetChunkNeighbors(x, y, returnNotActiveChunks: false).ToList();
        var randomChunk = neighboringChunks[Random.Shared.Next(neighboringChunks.Count)];
        var randomChunkPoints = randomChunk.GetFloorTileCoords().ToList();
        (int x, int y) randomPoint = randomChunkPoints[Random.Shared.Next(randomChunkPoints.Count)];
        SetPathTo(randomPoint.x, randomPoint.y);
    }
}