using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public class Level
{
    private const int ActiveChunkRadius = ChunkConstants.ActiveChunkRadius;
    private const int ChunkSize = ChunkConstants.ChunkSize;

    [JsonProperty] private readonly Tile[] _southernWall;
    [JsonProperty] private readonly Tile[] _westernWall;
    [JsonProperty] private readonly Chunk[,] _chunks;
    private Tile _cornerTile;

    [JsonProperty] public readonly Player Player;
    [JsonProperty] private (int x, int y) _finishPos;

    public event EventHandler InvolvedInTurn;
    public event EventHandler TurnFinished;
    public event EventHandler NpcDied;
    public event EventHandler<TileChangeEventArgs> TileChanged; 
    public event EventHandler<ActorActionEventArgs> ActorDidSomething; 

    [JsonIgnore]
    public (int x, int y) Size => (_chunks.GetLength(0) * ChunkSize + 1, _chunks.GetLength(1) * ChunkSize + 1);
    [JsonProperty] public bool? PlayerWon { get; private set; }

    [JsonProperty] private readonly EnemySpawner _enemySpawner;
    [JsonIgnore] public OverlayView OverlayView;
    [JsonIgnore] public MinimapView MinimapView;

    [JsonIgnore] private List<Chunk> _activeChunks;

    [JsonProperty] private readonly ObservableCollection<IControllable> _controllableObjects;
    [JsonProperty] public readonly ObservableCollection<ISightedObject> SightedObjects;
    [JsonProperty] public readonly LevelView LevelView;

    [JsonProperty] private Dictionary<(int x, int y), Counter> _doorCounters;

    [JsonProperty] private int _currentlyActiveIndex;
    [JsonProperty] private bool _energySpent;
    [JsonIgnore] private bool _turnAborted;
    [JsonProperty] private List<Actor> _activeActors;


    [JsonConstructor]
    private Level()
    {
    }
    

    public Level(Chunk[,] chunks, (int x, int y) startPos, Tile[] southernWall, Tile[] westernWall,
        (int x, int y) finishPos)
    {
        _chunks = chunks;
        _doorCounters = new Dictionary<(int x, int y), Counter>();

        _southernWall = southernWall;
        _westernWall = westernWall;
        _finishPos = finishPos;
        PlayerWon = null;
        Player = new Player(startPos.x, startPos.y, this);
        (int x, int y) = GetChunkCoords(startPos.x, startPos.y);
        _chunks[x, y].WasVisitedByPlayer = true;
        _enemySpawner = new EnemySpawner(this);
        SightedObjects = new ObservableCollection<ISightedObject> {Player};
        _controllableObjects = new ObservableCollection<IControllable> {Player};
        LevelView = new LevelView(this);

        Init();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        for (int i = 0; i < _chunks.GetLength(0); i++)
        {
            for (int j = 0; j < _chunks.GetLength(1); j++)
            {
                _chunks[i, j].GetNpcs().ForEach(SubscribeNpc);
            }
        }
        foreach (var keyValuePair in _doorCounters)
        {
            (int x, int y) = keyValuePair.Key;
            keyValuePair.Value.Action = counter =>
            {
                if (GetNonTileObject(x, y) is not null)
                    counter.Timer.Value++;
                else
                    InteractWithDoor(x, y);
            };
        }
        Init();
    }

    private void Init()
    {
        _cornerTile = new Tile(MapData.TileTypes["wall_nesw"]);
        OverlayView = new OverlayView(this);
        Player.Died += PlayerDiedHandler;
        Player.Moved += ControllableMoved;
        _controllableObjects.CollectionChanged += ControllableObjectsUpdateHandler;
        MinimapView = new MinimapView(_chunks, Player);
        _activeChunks = new();
        UpdateActiveChunks();
    }
    
    public void DoTurn()
    {
        if (PlayerWon is not null) return;

        List<Actor> inActiveChunks = GetActorsInActiveChunks();
        HashSet<Actor> involved = new();
        foreach (Actor actor in inActiveChunks) WatchActor(actor);
        InvolvedInTurn += NpcInvolvedInTurnHandler;

        while ((_activeActors ??= GetActive(inActiveChunks)).Any())
        {
            for (; _currentlyActiveIndex < _activeActors.Count; _currentlyActiveIndex++)
            {
                // The player may have reached the finish or died
                if (PlayerWon is not null)
                {
                    FinishLevelTurn();
                    return;
                }
                
                Actor actor = _activeActors[_currentlyActiveIndex];
                // An actor could have died due to actions of another actor
                if (actor.IsActive)
                {
                    ActorActionInfo actionInfo = actor.TakeAction();
                    if (actionInfo is not null)
                        ActorDidSomething?.Invoke(actor,
                            new ActorActionEventArgs(actionInfo.Action, actionInfo.AffectedCells));
                }

                if (_turnAborted)
                {
                    _turnAborted = false;
                    return;
                }
            }

            if (!_energySpent)
            {
                FinishLevelTurn();
                return;
            }

            _energySpent = false;

            inActiveChunks = GetActorsInActiveChunks();
            foreach (Actor actor in inActiveChunks.Where(actor => !involved.Contains(actor))) WatchActor(actor);
            
            ResetActiveActors();
        }

        FinishLevelTurn();
        return;

        void FinishLevelTurn()
        {
            ResetActiveActors();
            InvolvedInTurn -= NpcInvolvedInTurnHandler;
            foreach (var actor in involved) FinishActorTurn(actor);
            TurnFinished?.Invoke(this, EventArgs.Empty);
        }

        List<Actor> GetActive(IEnumerable<Actor> possiblyActive)
        {
            return possiblyActive.Where(actor => actor.IsActive).ToList();
        }

        List<Actor> GetActorsInActiveChunks()
        {
            return _controllableObjects.OfType<Actor>().Concat(GetActiveChunkNpcs()).ToList();
        }

        IEnumerable<Npc> GetActiveChunkNpcs()
        {
            return _activeChunks.SelectMany(chunk => chunk.GetNpcs());
        }
        
        void WatchActor(Actor actor)
        {
            actor.SpentEnergy += SpentEnergyHandler;
            involved.Add(actor);
        }

        void FinishActorTurn(Actor actor)
        {
            actor.SpentEnergy -= SpentEnergyHandler;
            actor.FinishTurn();
        }
        
        void NpcInvolvedInTurnHandler(object npc, EventArgs _)
        {
            if (!involved.Contains(npc)) involved.Add((Npc) npc);
        }
        
        void SpentEnergyHandler(object sender, EventArgs _) => _energySpent = true;

        void ResetActiveActors()
        {
            _activeActors = null;
            _currentlyActiveIndex = 0;
        }
    }

    public IDrawable GetDrawable(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        if (x == -1 || y == -1) return GetTile(x, y);
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        return !CoordsInRangeOfChunkArray(x, y, chunkX, chunkY)
            ? null
            : _chunks[chunkX, chunkY].GetDrawable(x, y) ?? GetTile(x, y);
    }
    
    public Tile GetTile(int x, int y, (int chunkX, int chunkY)? chunkCoords = null)
    {
        switch (x, y)
        {
            case (-1, -1):
                return _cornerTile;
            case (-1, >= 0):
                return y >= _westernWall.Length ? null : _westernWall[y];
            case (>= 0, -1):
                return x >= _southernWall.Length ? null : _southernWall[x];
            case not (>= 0, >= 0):
                return null;
        }

        var (chunkX, chunkY) = chunkCoords ?? GetChunkCoords(x, y);
        if (chunkX >= _chunks.GetLength(0) || chunkY >= _chunks.GetLength(1)) return null;
        int localX = x % ChunkSize;
        int localY = y % ChunkSize;
        return _chunks[chunkX, chunkY].Tiles[localX, localY];
    }

    public IObjectOnMap GetNonTileObject(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        return !CoordsInRangeOfChunkArray(x, y, chunkX, chunkY) ? null : _chunks[chunkX, chunkY].GetMapObject(x, y);
    }

    private void ControllableMoved(object sender, MoveEventArgs e)
    {
        bool isPlayer = ReferenceEquals(sender, Player);
        var controllable = (IControllable) sender;
        if (GetChunkCoords(e.OldX, e.OldY) != GetChunkCoords(controllable.X, controllable.Y))
        {
            UpdateActiveChunks();
            if (isPlayer)
            {
                (int x, int y) = GetChunkCoords(controllable.X, controllable.Y);
                if (CoordsInRangeOfChunkArray(controllable.X, controllable.Y, x, y))
                    _chunks[x, y].WasVisitedByPlayer = true;
            }
        }

        if (isPlayer && Player.X == _finishPos.x && Player.Y == _finishPos.y) PlayerWon = true;
    }

    private void UpdateActiveChunks()
    {
        _enemySpawner.TargetChunks = null;
        //remove out of range chunks from ActiveChunkRadius
        _activeChunks = _activeChunks.Where(chunk => _controllableObjects.Any(controllable =>
        {
            var (x, y) = GetChunkCoords(controllable.X, controllable.Y);
            bool active = 
                Math.Abs(x - chunk.MapX) <= ActiveChunkRadius + 1 && Math.Abs(y - chunk.MapY) <= ActiveChunkRadius + 1;
            if (!active)
                chunk.IsActive = false;
            return active;
        })).ToList();
        
        //add new chunks to list
        foreach (var obj in _controllableObjects)
        {
            var (x, y) = GetChunkCoords(obj.X, obj.Y);
            foreach (var chunk in GetChunksAround(x, y, ActiveChunkRadius))
            {
                if (chunk.IsActive) continue;
                _activeChunks.Add(chunk);
                chunk.IsActive = true;
                if (chunk.WasActiveBefore) continue;
                chunk.WasActiveBefore = true;
                _enemySpawner.SpawnEnemiesInChunk(chunk);
            }
        }
    }

    /// <summary>
    /// Get the neighboring chunks of a chunk
    /// </summary>
    /// <param name="chunkX">x coordinate of a base chunk</param>
    /// <param name="chunkY">y coordinate of a base chunk(used in recursion)</param>
    /// <param name="includeBaseChunk">set false to not include chunk with chunkX, chunkY coords</param>
    /// <param name="returnNotActiveChunks">set false to return only active chunks</param>
    /// <param name="amount">amount of iterations. 1: get neighboring chunks; 2: get neighboring chunks and their
    /// neighboring chunks; and so on...</param>
    /// <returns></returns>
    public HashSet<Chunk> GetChunkNeighbors(int chunkX, int chunkY, bool includeBaseChunk = true, 
        bool returnNotActiveChunks = true, int amount = 1)
    {
        Chunk chunk = _chunks[chunkX, chunkY];
        HashSet<Chunk> chunks = GetChunkNeighbors(chunk, amount, includeBaseChunk);
        if (includeBaseChunk && (returnNotActiveChunks || chunk.IsActive)) chunks.Add(chunk);
        else chunks.Remove(chunk);
        return chunks;
    }
    private HashSet<Chunk> GetChunkNeighbors(Chunk chunk, int amount, bool returnNotActiveChunks,
        Chunk prevChunk = null)
    {
        if (amount <= 0) return null;
        HashSet<Chunk> hashSet = new();
        foreach ((int x, int y) in chunk.NeighborCoords)
        {
            if(prevChunk != null && (x, y) == (prevChunk.MapX, prevChunk.MapY)) continue; 
            Chunk newChunk = _chunks[x, y];
            if(!returnNotActiveChunks && !chunk.IsActive) continue;
            hashSet.Add(newChunk);
            HashSet<Chunk> newSet = GetChunkNeighbors(newChunk, amount - 1, returnNotActiveChunks, chunk);
            if(newSet != null) hashSet.UnionWith(newSet);
        }
        return hashSet;
    }

    public HashSet<Chunk> GetChunksAroundControllable(int amount = 1, bool includeBaseChunks = true)
    {
        HashSet<Chunk> hashSet = new();
        if (amount <= 0) return hashSet;
        List<(int x, int y)> prevChunks = new();
        foreach (var controllable in _controllableObjects)
        {
            (int x, int y) = GetChunkCoords(controllable.X, controllable.Y);
            if(prevChunks.Contains((x, y)) || !CoordsInRangeOfChunkArray(controllable.X, controllable.Y, x, y)) 
                continue;
            prevChunks.Add((x, y));
            hashSet.UnionWith(GetChunkNeighbors(_chunks[x, y], amount, true));
            if (includeBaseChunks) hashSet.Add(_chunks[x, y]);
        }

        if (includeBaseChunks) return hashSet;
        
        foreach (var (x, y) in prevChunks)
        {
            hashSet.Remove(_chunks[x, y]);
        }
        
        return hashSet;
    }

    private void ControllableObjectsUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (var controllableObject in e.NewItems)
            {
                ((IControllable) controllableObject).Moved += ControllableMoved;
            }

        if (e.OldItems is null) return;
        foreach (var controllableObject in e.OldItems)
        {
            ((IControllable) controllableObject).Moved -= ControllableMoved;
        }
    }

    public IEnumerable<Chunk> GetChunksAround(int chunkXCenter, int chunkYCenter, int radius)
    {
        for (int chunkX = Math.Max(chunkXCenter - radius, 0);
             chunkX < Math.Min(chunkXCenter + radius + 1, _chunks.GetLength(0));
             chunkX++)
        {
            for (int chunkY = Math.Max(chunkYCenter - radius, 0);
                 chunkY < Math.Min(chunkYCenter + radius + 1, _chunks.GetLength(1));
                 chunkY++)
            {
                yield return _chunks[chunkX, chunkY];
            }
        }
    }
    
    private void PlayerDiedHandler(object _, EventArgs __)
    {
        PlayerWon = false;
    }
    
    private void SomethingMovedHandler(object sender, MoveEventArgs e)
    {
        if (sender is Npc npc)
        {
            var chunk0 = GetChunkCoords(e.OldX, e.OldY);
            var chunk1 = GetChunkCoords(npc.X, npc.Y);
            if (chunk0 != chunk1)
            {
                _chunks[chunk0.chunkX, chunk0.chunkY].RemoveNpc(npc);
                _chunks[chunk1.chunkX, chunk1.chunkY].AddNpc(npc);
            }
        }
    }
    
    public static (int chunkX, int chunkY) GetChunkCoords(int x, int y)
    {
        return (x / ChunkSize, y / ChunkSize);
    }

    private bool CoordsInRangeOfChunkArray(int x, int y, int chunkX, int chunkY)
    {
        return x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1);
    }

    public void AddNpc(Npc npc)
    {
        SubscribeNpc(npc);
        (int x, int y) = GetChunkCoords(npc.X, npc.Y);
        _chunks[x, y].AddNpc(npc);
    }

    public void SubscribeNpc(Npc npc)
    {
        npc.InvolvedInTurn += InvolvedInTurnHandler;
        npc.Died += NpcDeathHandler;
        npc.Moved += SomethingMovedHandler;
    }

    public void AddItem(Item item, int x, int y)
    {
        if (!Inbounds(x, y)) return;
        (int chunkX, int chunkY) = GetChunkCoords(x, y);
        _chunks[chunkX, chunkY].AddItem(item, x, y);
    }

    public List<Item> GetItems(int x, int y)
    {
        if (!Inbounds(x, y)) return null;
        (int chunkX, int chunkY) = GetChunkCoords(x, y);
        return _chunks[chunkX, chunkY].GetItems(x, y);
    }

    public LinkedList<(int, int)> GetPathBetweenChunks(int x0, int y0, int x1, int y1, bool onlyActiveChunks = true)
    {
        (int chunkX0, int chunkY0) = GetChunkCoords(x0, y0);
        (int chunkX1, int chunkY1) = GetChunkCoords(x1, y1);
        if (chunkX0 == chunkX1 && chunkY0 == chunkY1)
        {
            LinkedList<(int, int)> list = new();
            list.AddLast((chunkX0, chunkY0));
            return list;
        }
        return Algorithms.AStar(chunkX0, chunkY0, chunkX1, chunkY1,
            (x, y) => MapRouteGetNeighbors(x, y, onlyActiveChunks));
    }

    private IEnumerable<(int x, int y, int cost)> MapRouteGetNeighbors(int x, int y, bool onlyActiveChunks)
    {
        foreach ((int neighborX, int neighborY) in _chunks[x, y].NeighborCoords)
        {
            if(onlyActiveChunks && !_chunks[neighborX, neighborY].IsActive) continue;
            yield return (neighborX, neighborY, 1);
        }
    }

    public bool InteractWithDoor(int x, int y, Actor actor = null)
    {
        Tile tile = GetTile(x, y);
        if (tile is null || !tile.IsDoor || GetNonTileObject(x, y) is not null)
            return false;
        tile.InteractWithDoor();
        Counter doorCounter;
        if (!_doorCounters.ContainsKey((x, y)))
        {
            doorCounter = new Counter(this, 20, 20, counter =>
            {
                if (GetNonTileObject(x, y) is not null)
                    counter.Timer.Value++;
                else
                    InteractWithDoor(x, y);
            });
            _doorCounters.Add((x, y), doorCounter);
        }
        else
        {
            doorCounter = _doorCounters[(x, y)];
        }
        if (tile.IsWalkable)
            doorCounter.ResetTimer();
        else
            doorCounter.StopTimer();
        if (actor is not null) actor.Energy -= 100;
        TileChanged?.Invoke(this, new TileChangeEventArgs((x, y)));
        return true;
    }

    private bool Inbounds(int x, int y)
    {
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        return x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1);
    }

    private void NpcDeathHandler(object npc, EventArgs _)
    {
        Npc diedNpc = (Npc) npc;
        (int x, int y) = GetChunkCoords(diedNpc.X, diedNpc.Y);
        _chunks[x, y].RemoveNpc(diedNpc);
        NpcDied?.Invoke(npc, EventArgs.Empty);
    }
    
    public void AbortTurn() => _turnAborted = true;

    private void InvolvedInTurnHandler(object npc, EventArgs e) => InvolvedInTurn?.Invoke(npc, e);
}