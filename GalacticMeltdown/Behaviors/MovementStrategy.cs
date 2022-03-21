using System;
using GalacticMeltdown.LevelRelated;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "Movement";
    private const int DefaultPriority = 20;
    private Level Level => ControlledNpc.Level;
    [JsonProperty] private (int x, int y)? _wantsToGoTo;
    [JsonIgnore] private LinkedListNode<(int x, int y)> _nextPathCellNode;
    [JsonIgnore] private LinkedList<(int x, int y)> _chunkPath;
    [JsonProperty] private readonly Counter _previousTargetCounter;
    [JsonProperty] private Actor _previousTarget;
    [JsonProperty] private bool _idleMovement;
    
    [JsonIgnore]
    public Actor PreviousTarget
    {
        get => _previousTarget;
        set
        {
            if(value is not null)
                _previousTargetCounter.ResetTimer();
            _previousTarget = value;
        }
    }

    [JsonConstructor]
    private MovementStrategy()
    {
    }
    public MovementStrategy(MovementStrategyData data, Npc controlledNpc) : base(data.Priority ?? DefaultPriority)
    {
        ControlledNpc = controlledNpc;
        _previousTargetCounter = new Counter(Level, 5, 0, _ => PreviousTarget = null);
        ControlledNpc.Died += _previousTargetCounter.RemoveCounter;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _previousTargetCounter.Action = _ => PreviousTarget = null;
        ControlledNpc.Died += _previousTargetCounter.RemoveCounter;

    }

    private IEnumerable<(int, int, int)> GetNeighbors(int x, int y)
    {
        foreach ((int xi, int yi) in Algorithms.GetPointsOnSquareBorder(x, y, 1))
        {
            (int, int) chunkCoords = Level.GetChunkCoords(xi, yi);
            Tile tile = Level.GetTile(xi, yi, chunkCoords);
            if (tile is null || !tile.IsWalkable && !tile.IsDoor || !_chunkPath.Contains(chunkCoords) ||
                x == ControlledNpc.X && y == ControlledNpc.Y && Level.GetNonTileObject(xi, yi) is not null)
                continue;
            int cost;
            // if door is closed
            if (tile.IsDoor && !tile.IsWalkable)
            {
                cost = 200;
            }
            else
            {
                cost = tile.MoveCost;
            }
            yield return (xi, yi, cost);
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
        var path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, x, y, GetNeighbors);
        if (path is null || path.Count < 3)
        {
            _wantsToGoTo = null;
            _nextPathCellNode = null;
            return;
        }

        path.RemoveFirst();
        path.RemoveLast();

        _nextPathCellNode = path.First;
    }


    public override bool TryAct()
    {
        if (ControlledNpc.CurrentTarget is not null) PreviousTarget = ControlledNpc.CurrentTarget;
        if (PreviousTarget is not null && Level.GetTile(PreviousTarget.X, PreviousTarget.Y) is not null &&
            Level.GetTile(PreviousTarget.X, PreviousTarget.Y).IsWalkable)
        {
            _idleMovement = false;
            if (_wantsToGoTo != (PreviousTarget.X, PreviousTarget.Y))
            {
                _wantsToGoTo = (PreviousTarget.X, PreviousTarget.Y);
                SetPathTo(PreviousTarget.X, PreviousTarget.Y);
            }
        }
        else if (_nextPathCellNode is null)
        {
            if (_wantsToGoTo is not null)
                SetPathTo(_wantsToGoTo.Value.x, _wantsToGoTo.Value.y);
            else
                CalculateIdleMovementPath();
        }
        else if (Level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is not null)
        {
            if(_idleMovement || _wantsToGoTo is null)
                CalculateIdleMovementPath();
            else
                SetPathTo(_wantsToGoTo.Value.x, _wantsToGoTo.Value.y);
        }
        if (_idleMovement && UtilityFunctions.Chance(60))
        {
            ControlledNpc.StopTurn();
            return true;
        }
        
        if (_nextPathCellNode is null)
            return false;
        Tile nextPathTile =  Level.GetTile(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        if (!nextPathTile.IsWalkable && !nextPathTile.IsDoor ||
            Level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is not null) return false;
        // open door
        if(nextPathTile.IsDoor && !nextPathTile.IsWalkable)
            nextPathTile.InteractWithDoor.Invoke();
        ControlledNpc.MoveNpcTo(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        _nextPathCellNode = _nextPathCellNode.Next;
        if (_nextPathCellNode is null) _wantsToGoTo = null;
        return true;
    }
    private void CalculateIdleMovementPath()
    {
        _idleMovement = true;
        (int x, int y) = Level.GetChunkCoords(ControlledNpc.X, ControlledNpc.Y);
        var neighboringChunks = Level.GetChunkNeighbors(x, y, returnNotActiveChunks: false).ToList();
        var randomChunk = neighboringChunks[Random.Shared.Next(neighboringChunks.Count)];
        var randomChunkPoints = randomChunk.GetFloorTileCoords().ToList();
        (int x, int y) randomPoint = randomChunkPoints[Random.Shared.Next(randomChunkPoints.Count)];
        _wantsToGoTo = (randomPoint.x, randomPoint.y);
        SetPathTo(randomPoint.x, randomPoint.y);
    }
}