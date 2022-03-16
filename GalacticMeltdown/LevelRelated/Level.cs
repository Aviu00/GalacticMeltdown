using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.LevelRelated;

internal class ChunkEventListener
{
    public event EventHandler NpcDied;
    public event EventHandler<MoveEventArgs> SomethingMoved;
    public event EventHandler NpcInvolvedInTurn;
    private void SomethingMovedHandler(object sender, MoveEventArgs e) => SomethingMoved?.Invoke(sender, e);

    private void NpcDeathHandler(object npc, EventArgs e) => NpcDied?.Invoke(npc, e);
    private void NpcInvolvedHandler(object npc, EventArgs e) => NpcInvolvedInTurn?.Invoke(npc, e);

    public ChunkEventListener(Chunk[,] chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.NpcDied += NpcDeathHandler;
            chunk.SomethingMoved += SomethingMovedHandler;
            chunk.NpcInvolvedInTurn += NpcInvolvedHandler;
        }
    }
}

public partial class Level
{
    private const int ActiveChunkRadius = DataHolder.ActiveChunkRadius;
    private const int ChunkSize = DataHolder.ChunkSize;

    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;

    private readonly Chunk[,] _chunks;

    private readonly int _finishX;
    private readonly int _finishY;

    public event EventHandler TurnFinished;
    public event EventHandler NpcDied;
    public event EventHandler<MoveEventArgs> SomethingMoved;

    public (int x, int y) Size => (_chunks.GetLength(0) * ChunkSize + 1, _chunks.GetLength(1) * ChunkSize + 1);

    public bool IsActive { get; private set; }
    public bool PlayerWon { get; private set; }

    private readonly EnemySpawner _enemySpawner;
    public Player Player { get; }
    public LevelView LevelView { get; }
    public OverlayView OverlayView { get; }

    public List<Chunk> ActiveChunks { get; private set; }

    public ObservableCollection<IControllable> ControllableObjects { get; }
    public ObservableCollection<ISightedObject> SightedObjects { get; }

    private readonly ChunkEventListener _listener;

    public Level(Chunk[,] chunks, (int x, int y) startPos, Tile[] southernWall, Tile[] westernWall,
        (int x, int y) finishPos)
    {
        _cornerTile = new Tile(DataHolder.TileTypes["wall_nesw"]);
        _chunks = chunks;
        _listener = new ChunkEventListener(_chunks);
        _listener.NpcDied += NpcDeathHandler;
        _listener.SomethingMoved += SomethingMovedHandler;

        _southernWall = southernWall;
        _westernWall = westernWall;
        (_finishX, _finishY) = finishPos;
        Player = new Player(startPos.x, startPos.y, this);
        Player.Died += PlayerDiedHandler;
        Player.Moved += ControllableMoved;
        ControllableObjects = new ObservableCollection<IControllable> {Player};
        ControllableObjects.CollectionChanged += ControllableObjectsUpdateHandler;
        SightedObjects = new ObservableCollection<ISightedObject> {Player};
        LevelView = new LevelView(this, Player);
        OverlayView = new OverlayView(this);
        IsActive = true;
        PlayerWon = false;

        _enemySpawner = new EnemySpawner(this);
        ActiveChunks = new();
        UpdateActiveChunks();
    }
    
    public bool DoTurn()
    {
        if (!IsActive) return false;

        HashSet<Actor> involved = new();
        _listener.NpcInvolvedInTurn += NpcInvolvedInTurnHandler;

        List<Actor> currentlyActive;
        bool energySpent = false;
        while ((currentlyActive = GetActive()).Any())
        {
            foreach (var actor in currentlyActive.Where(actor => !involved.Contains(actor))) WatchActor(actor);

            foreach (var actor in currentlyActive)
            {
                // A player may have reached the finish or died
                if (!IsActive) return FinishMapTurn();
                // An actor could die due to actions of another actor
                if (actor.IsActive) actor.TakeAction();
            }

            if (!energySpent) return FinishMapTurn(); // avoid infinite loop when no actor does anything

            energySpent = false;
        }

        return FinishMapTurn();

        bool FinishMapTurn()
        {
            _listener.NpcInvolvedInTurn -= NpcInvolvedInTurnHandler;
            foreach (var actor in involved) FinishActorTurn(actor);
            TurnFinished?.Invoke(this, EventArgs.Empty);
            return IsActive;
        }

        List<Actor> GetActive()
        {
            return ControllableObjects.OfType<Actor>().Concat(GetNearbyNpcs()).Where(actor => actor.IsActive).ToList();
        }

        IEnumerable<Npc> GetNearbyNpcs()
        {
            return ActiveChunks.SelectMany(chunk => chunk.GetNpcs());
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
        
        void SpentEnergyHandler(object sender, EventArgs _) => energySpent = true;
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
    
    public Tile GetTile(int x, int y)
    {
        switch (x, y)
        {
            case (-1, -1):
                return _cornerTile;
            case (-1, >= 0):
                return y >= _westernWall.Length ? null : _westernWall[y];
            case (>= 0, -1):
                return x >= _southernWall.Length ? null : _southernWall[x];
        }

        var (chunkX, chunkY) = GetChunkCoords(x, y);
        int localX = x % ChunkSize;
        int localY = y % ChunkSize;
        return !CoordsInRangeOfChunkArray(x, y, chunkX, chunkY) ? null : _chunks[chunkX, chunkY].Tiles[localX, localY];
    }

    public IObjectOnMap GetNonTileObject(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        return !CoordsInRangeOfChunkArray(x, y, chunkX, chunkY) ? null : _chunks[chunkX, chunkY].GetMapObject(x, y);
    }

    private void ControllableMoved(object sender, MoveEventArgs e)
    {
        if (GetChunkCoords(e.X0, e.Y0) != GetChunkCoords(e.X1, e.Y1))
        {
            UpdateActiveChunks();
        }

        if (!ReferenceEquals(sender, Player) || Player.X != _finishX || Player.Y != _finishY) return;
        IsActive = false;
        PlayerWon = true;
    }

    private void UpdateActiveChunks()
    {
        _enemySpawner.TargetChunks = null;
        //remove out of range chunks from ActiveChunkRadius
        ActiveChunks = ActiveChunks.Where(chunk => ControllableObjects.Any(controllable =>
        {
            var (x, y) = GetChunkCoords(controllable.X, controllable.Y);
            bool active = 
                Math.Abs(x - chunk.MapX) <= ActiveChunkRadius && Math.Abs(y - chunk.MapY) <= ActiveChunkRadius;
            if (!active)
                chunk.isActive = false;
            return active;
        })).ToList();
        
        //add new chunks to list
        foreach (var obj in ControllableObjects)
        {
            var (x, y) = GetChunkCoords(obj.X, obj.Y);
            foreach (var chunk in GetChunksAround(x, y, ActiveChunkRadius))
            {
                if (chunk.isActive) continue;
                ActiveChunks.Add(chunk);
                chunk.isActive = true;
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
        if (includeBaseChunk && (returnNotActiveChunks || chunk.isActive)) chunks.Add(chunk);
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
            if(!returnNotActiveChunks && !chunk.isActive) continue;
            hashSet.Add(newChunk);
            HashSet<Chunk> newSet = GetChunkNeighbors(newChunk, amount - 1, returnNotActiveChunks, chunk);
            if(newSet != null) hashSet.UnionWith(newSet);
        }
        return hashSet;
    }

    public HashSet<Chunk> GetChunksAroundControllable(int amount = 1, bool includeBaseChunks = true)
    {
        HashSet<Chunk> hashSet = new();
        List<(int x, int y)> prevChunks = new();
        foreach (var controllable in ControllableObjects)
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

    private IEnumerable<Chunk> GetChunksAround(int chunkXCenter, int chunkYCenter, int radius)
    {
        for (int chunkX = Math.Max(chunkXCenter - radius, 0);
             chunkX < Math.Min(chunkXCenter + radius, _chunks.GetLength(0));
             chunkX++)
        {
            for (int chunkY = Math.Max(chunkYCenter - radius, 0);
                 chunkY < Math.Min(chunkYCenter + radius, _chunks.GetLength(1));
                 chunkY++)
            {
                yield return _chunks[chunkX, chunkY];
            }
        }
    }
    
    private void PlayerDiedHandler(object _, EventArgs __)
    {
        PlayerWon = false;
        IsActive = false;
    }
    
    private void SomethingMovedHandler(object sender, MoveEventArgs e)
    {
        if (sender is Npc npc)
        {
            var chunk0 = GetChunkCoords(e.X0, e.Y0);
            var chunk1 = GetChunkCoords(e.X1, e.Y1);
            if (chunk0 != chunk1)
            {
                _chunks[chunk0.chunkX, chunk0.chunkY].RemoveNpc(npc);
                _chunks[chunk1.chunkX, chunk1.chunkY].AddNpc(npc);
            }
        }

        SomethingMoved?.Invoke(sender, e);
    }
    
    private static (int chunkX, int chunkY) GetChunkCoords(int x, int y)
    {
        return (x / ChunkSize, y / ChunkSize);
    }

    private bool CoordsInRangeOfChunkArray(int x, int y, int chunkX, int chunkY)
    {
        return x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1);
    }

    private void NpcDeathHandler(object npc, EventArgs _) => NpcDied?.Invoke(npc, EventArgs.Empty);
}