using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;
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
    private const int ActiveChunkRadius = 3;

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
    
    public Player Player { get; }
    public LevelView LevelView { get; }
    public OverlayView OverlayView { get; }

    public List<Chunk> ActiveChunks { get; private set; }

    public ObservableCollection<IControllable> ControllableObjects { get; }
    public ObservableCollection<ISightedObject> SightedObjects { get; }

    private readonly ChunkEventListener _listener;
    // test functions
    private void TestAddEnemy(int x, int y, Enemy enemy)
    {
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        _chunks[chunkX, chunkY].AddNpc(enemy);
    }

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
        // test enemy
        Enemy enemy1 = 
            new Enemy(100, 100, 10, 10, 20,
                startPos.x + 5, startPos.y + 5, this, 
                new(new Behavior.BehaviorComparer()){new MovementStrategy(this)});
        enemy1.Targets = new HashSet<Actor>(){Player};
        TestAddEnemy(startPos.x + 1, startPos.y + 1, enemy1);
        Player.Died += PlayerDiedHandler;
        Player.Moved += ControllableMoved;
        ControllableObjects = new ObservableCollection<IControllable> {Player};
        ControllableObjects.CollectionChanged += ControllableObjectsUpdateHandler;
        SightedObjects = new ObservableCollection<ISightedObject> {Player};
        LevelView = new LevelView(this);
        OverlayView = new OverlayView(this);
        IsActive = true;
        PlayerWon = false;
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
            foreach (var actor in currentlyActive.Where(actor => !involved.Contains(actor)))
            {
                WatchActor(actor);
            }

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
            foreach (var actor in involved)
            {
                FinishActorTurn(actor);
            }

            TurnFinished?.Invoke(this, EventArgs.Empty);
            return IsActive;
        }

        List<Actor> GetActive()
        {
            List<Actor> active = new List<Actor>(ControllableObjects.OfType<Actor>());
            active.AddRange(GetNearbyNpcs());
            foreach (var npc in GetNearbyNpcs())
            {
                if (!involved.Contains(npc)) involved.Add(npc);
                if (npc.IsActive) active.Add(npc);
            }

            return active;
        }

        List<Npc> GetNearbyNpcs()
        {
            List<Npc> npcs = new();
            ActiveChunks.ForEach(chunk => npcs.AddRange(chunk.GetNpcs()));
            return npcs;
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
        return (IDrawable) GetNonTileObject(x, y) ?? GetTile(x, y);
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
        if (!(x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1)))
        {
            return null;
        }

        return _chunks[chunkX, chunkY].Tiles[localX, localY];
    }

    public IObjectOnMap GetNonTileObject(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        var (chunkX, chunkY) = GetChunkCoords(x, y);
        if (!(x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1)))
        {
            return null;
        }

        return _chunks[chunkX, chunkY].GetMapObject(x, y);
    }

    private void ControllableMoved(object sender, MoveEventArgs e)
    {
        if (GetChunkCoords(e.X0, e.Y0) != GetChunkCoords(e.X1, e.Y1))
        {
            UpdateActiveChunks();
        }

        if (ReferenceEquals(sender, Player) && Player.X == _finishX && Player.Y == _finishY)
        {
            IsActive = false;
            PlayerWon = true;
        }
    }

    private void UpdateActiveChunks()
    {
        //remove out of range chunks from ActiveChunkRadius
        ActiveChunks = ActiveChunks.Where(chunk => ControllableObjects.Any(controllable =>
        {
            var (x, y) = GetChunkCoords(controllable.X, controllable.Y);
            return Math.Abs(x - chunk.MapX) <= ActiveChunkRadius && Math.Abs(y - chunk.MapY) <= ActiveChunkRadius;
        })).ToList();
        
        //add new chunks to list
        foreach (var obj in ControllableObjects)
        {
            var (x, y) = GetChunkCoords(obj.X, obj.Y);
            foreach (var chunk in GetChunksAround(x, y, ActiveChunkRadius))
            {
                if (ActiveChunks.Contains(chunk)) continue;
                ActiveChunks.Add(chunk);
                chunk.WasActiveBefore = true;
            }
        }
    }


    /// <summary>
    /// Get the neighboring chunks of a chunk
    /// </summary>
    /// <param name="chunkX">x coordinate of a base chunk</param>
    /// <param name="chunkY">y coordinate of a base chunk(used in recursion)</param>
    /// <param name="includeBaseChunk">set false to not include chunk with chunkX, chunkY coords</param>
    /// <param name="amount">amount of iterations. 1: get neighboring chunks; 2: get neighboring chunks and their
    /// neighboring chunks; and so on...</param>
    /// <returns></returns>
    public HashSet<Chunk> GetChunkNeighbors(int chunkX, int chunkY, bool includeBaseChunk = true, int amount = 1)
    {
        Chunk chunk = _chunks[chunkX, chunkY];
        HashSet<Chunk> chunks = GetChunkNeighbors(chunk, amount);
        if(includeBaseChunk) chunks.Add(chunk);
        return chunks;
    }
    private HashSet<Chunk> GetChunkNeighbors(Chunk chunk, int amount, Chunk prevChunk = null)
    {
        if (amount <= 0) return null;
        HashSet<Chunk> hashSet = new();
        foreach ((int x, int y) in chunk.NeighborCoords)
        {
            if(prevChunk != null && (x, y) == (prevChunk.MapX, prevChunk.MapY)) continue; 
            Chunk newChunk = _chunks[x, y];
            hashSet.Add(newChunk);
            HashSet<Chunk> newSet = GetChunkNeighbors(newChunk, amount - 1, chunk);
            if(newSet != null) hashSet.UnionWith(newSet);
        }
        return hashSet;
    }
    
    //private void SpawnEnemies()
    //{
    //    foreach (var (chunkX, chunkY) in GetChunkIndexes())
    //    {
    //        _chunks[chunkX, chunkY].SpawnEnemies();
    //    }
//
    //    IEnumerable<(int chunkX, int chunkY)> GetChunkIndexes()
    //    {
    //        foreach (var controllable in ControllableObjects)
    //        {
    //            var (controllableChunkX, controllableChunkY) = GetChunkCoords(controllable.X, controllable.Y);
    //            foreach (var chunkCoords in Algorithms.GetPointsOnSquareBorder(controllableChunkX, controllableChunkY,
    //                         SpawnRadius))
    //            {
    //                if (ControllableObjects.All(obj =>
    //                    {
    //                        var (objChunkX, objChunkY) = GetChunkCoords(obj.X, obj.Y);
    //                        return chunkCoords.x >= 0 && chunkCoords.x < _chunks.GetLength(0) 
    //                            && chunkCoords.y >= 0 && chunkCoords.y < _chunks.GetLength(1)
    //                            && Math.Abs(chunkCoords.x - objChunkX) >= SpawnRadius
    //                            && Math.Abs(chunkCoords.y - objChunkY) >= SpawnRadius;
    //                    }))
    //                    yield return chunkCoords;
    //            }
    //        }
    //    }
    //}

    private void ControllableObjectsUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (var controllableObject in e.NewItems)
            {
                ((IControllable) controllableObject).Moved += ControllableMoved;
            }

        if (e.OldItems is not null)
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

    private void NpcDeathHandler(object npc, EventArgs _) => NpcDied?.Invoke(npc, EventArgs.Empty);
}