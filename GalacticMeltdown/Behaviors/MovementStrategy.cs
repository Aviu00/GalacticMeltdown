using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    [JsonProperty] protected override string Strategy => "Movement";
    private const int DefaultPriority = 20;
    private Level Level => ControlledNpc.Level;
    [JsonProperty] private (int x, int y)? _wantsToGoTo;
    private LinkedListNode<(int x, int y)> _nextPathCellNode;
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
        _previousTargetCounter = new Counter(Level, 4, 0, _ => PreviousTarget = null);
        ControlledNpc.Died += _previousTargetCounter.RemoveCounter;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _previousTargetCounter.Action = _ => PreviousTarget = null;
        ControlledNpc.Died += _previousTargetCounter.RemoveCounter;

    }

    private void SetPathTo(int destX, int destY)
    {
        var chunkPath = Level.GetPathBetweenChunks(ControlledNpc.X, ControlledNpc.Y, destX, destY);
        if (chunkPath is null)
        {
            _wantsToGoTo = null;
            _nextPathCellNode = null;
            return;
        }
        var path = Algorithms.AStar(ControlledNpc.X, ControlledNpc.Y, destX, destY, GetNeighbors);
        if (path is null || path.Count < 3)
        {
            _wantsToGoTo = null;
            _nextPathCellNode = null;
            return;
        }

        path.RemoveFirst();
        path.RemoveLast();

        _nextPathCellNode = path.First;
        
        IEnumerable<(int, int, int)> GetNeighbors(int x, int y)
        {
            foreach ((int xi, int yi) in Algorithms.GetPointsOnSquareBorder(x, y, 1))
            {
                (int, int) chunkCoords = Level.GetChunkCoords(xi, yi);
                Tile tile = Level.GetTile(xi, yi);
                if (tile is null || !tile.IsWalkable && !tile.IsDoor || !chunkPath.Contains(chunkCoords) ||
                    x == ControlledNpc.X && y == ControlledNpc.Y && Level.GetNonTileObject(xi, yi) is not null)
                    continue;
            
                // if door is closed
                int cost = tile.IsDoor && !tile.IsWalkable
                    ? EnergyCosts.DoorInteraction + MapData.TileTypes["door-open"].MoveCost
                    : tile.MoveCost;
                yield return (xi, yi, cost);
            }
        }
    }


    public override ActorActionInfo TryAct()
    {
        if (ControlledNpc.CurrentTarget is not null) PreviousTarget = ControlledNpc.CurrentTarget;
        Tile previousTargetTile = null;
        if (PreviousTarget is not null)
            previousTargetTile = Level.GetTile(PreviousTarget.X, PreviousTarget.Y);
        if (previousTargetTile is not null && previousTargetTile.IsWalkable)
        {
            _idleMovement = false;
            if (_wantsToGoTo != (PreviousTarget.X, PreviousTarget.Y) || _nextPathCellNode is not null
                && Level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is not null)
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
        if (_idleMovement && UtilityFunctions.Chance(50))
        {
            ControlledNpc.StopTurn();
            return new ActorActionInfo(ActorAction.StopTurn, new List<(int, int)>());
        }

        if (_nextPathCellNode is null)
            return null;
        if (Level.GetNonTileObject(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y) is not null) return null;
        Tile nextPathTile =  Level.GetTile(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        if (!nextPathTile.IsWalkable)
        {
            if (!nextPathTile.IsDoor) return null;
            Level.InteractWithDoor(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y, ControlledNpc);
            return new ActorActionInfo(ActorAction.InteractWithDoor,
                new List<(int, int)> {(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y)});;
        }
        int oldX = ControlledNpc.X, oldY = ControlledNpc.Y; 
        int newX = _nextPathCellNode.Value.x, newY = _nextPathCellNode.Value.y;
        ControlledNpc.MoveNpcTo(_nextPathCellNode.Value.x, _nextPathCellNode.Value.y);
        _nextPathCellNode = _nextPathCellNode.Next;
        if (_nextPathCellNode is null) _wantsToGoTo = null;
        return new ActorActionInfo(ActorAction.Move,
            new List<(int, int)> {(oldX, oldY), (newX, newY)});
    }
    private void CalculateIdleMovementPath()
    {
        _idleMovement = true;
        (int x, int y) = Level.GetChunkCoords(ControlledNpc.X, ControlledNpc.Y);
        List<Chunk> neighboringChunks = Level.GetChunkNeighbors(x, y, returnNotActiveChunks: false).ToList();
        if(neighboringChunks.Count == 0) return;
        Chunk randomChunk = neighboringChunks[Random.Shared.Next(neighboringChunks.Count)];
        List<(int, int)> randomChunkPoints = randomChunk.GetFloorTileCoords();
        if (randomChunkPoints.Count == 0) return;
        (int x, int y) randomPoint = randomChunkPoints[Random.Shared.Next(randomChunkPoints.Count)];
        _wantsToGoTo = (randomPoint.x, randomPoint.y);
        SetPathTo(randomPoint.x, randomPoint.y);
    }

    public override List<string> GetDescription()
    {
        return new()
        {
            "",
            "Can move"
        };
    }
}