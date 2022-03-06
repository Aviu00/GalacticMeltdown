using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public delegate void TurnFinishedEventHandler();

public partial class Level
{
    private const int EnemyRadiusPlayer = 3;
    private const int EnemyRadiusControllable = 1;

    private const int SpawnRadius = 5;

    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;

    private Chunk[,] _chunks;

    private readonly int _finishX;
    private readonly int _finishY;

    public event TurnFinishedEventHandler TurnFinished;
    public event DiedEventHandler NpcDied;
    public event MovedEventHandler SomethingMoved;

    public (int x, int y) Size
    {
        get => (_chunks.GetLength(0) * DataHolder.ChunkSize + 1, _chunks.GetLength(1) * DataHolder.ChunkSize + 1);
    }

    public bool IsActive { get; private set; }
    public bool PlayerWon { get; private set; }

    public Player Player { get; }
    public LevelView LevelView { get; }
    public OverlayView OverlayView { get; }

    public ObservableCollection<IControllable> ControllableObjects { get; }
    public ObservableCollection<ISightedObject> SightedObjects { get; }

    public Level(Chunk[,] chunks, (int x, int y) startPos, Tile[] southernWall, Tile[] westernWall, (int x, int y) finishPos)
    {
        _cornerTile = new Tile(DataHolder.TileTypes["wall_nesw"]);
        _chunks = chunks;
        foreach (var chunk in _chunks)
        {
            chunk.NpcDied += NpcDeathHandler;
            chunk.SomethingMoved += SomethingMovedHandler;
        }
        _southernWall = southernWall;
        _westernWall = westernWall;
        (_finishX, _finishY) = finishPos;
        Player = new Player(startPos.x, startPos.y, this);
        Player.Died += PlayerDiedHandler;
        Player.Moved += ControllableMoved;
        ControllableObjects = new ObservableCollection<IControllable> { Player };
        ControllableObjects.CollectionChanged += ControllableObjectsUpdateHandler;
        SightedObjects = new ObservableCollection<ISightedObject> { Player };
        LevelView = new LevelView(this);
        OverlayView = new OverlayView(this);
        IsActive = true;
        PlayerWon = false;
    }

    private void SomethingMovedHandler(IMovable movable, int x0, int y0, int x1, int y1)
    {
        if (movable is Npc npc)
        {
            var chunk0 = GetChunk(x0, y0);
            var chunk1 = GetChunk(x1, y1);
            if (chunk0 != chunk1)
            {
                _chunks[chunk0.chunkX, chunk0.chunkY].RemoveNpc(npc);
                _chunks[chunk1.chunkX, chunk1.chunkY].AddNpc(npc);
            }
        }
        
        SomethingMoved?.Invoke(movable, x0, y0, x1, y1);
    }

    private void NpcDeathHandler(Actor npc) => NpcDied?.Invoke(npc);

    private void ControllableObjectsUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null) foreach (var controllableObject in e.NewItems)
        {
            ((IControllable) controllableObject).Moved += ControllableMoved;
        }
        if (e.OldItems is not null) foreach (var controllableObject in e.OldItems)
        {
            ((IControllable) controllableObject).Moved -= ControllableMoved;
        }
    }

    private void PlayerDiedHandler(Actor actor)
    {
        PlayerWon = false;
        IsActive = false;
    }

    private void SpawnEnemies()
    {
        foreach (var (chunkX, chunkY) in GetChunkIndexes())
        {
            _chunks[chunkX, chunkY].SuggestEnemySpawn();
        }
        
        IEnumerable<(int chunkX, int chunkY)> GetChunkIndexes()
        {
            foreach (var controllable in ControllableObjects)
            {
                var (controllableChunkX, controllableChunkY) = GetChunk(controllable.X, controllable.Y);
                foreach (var chunkCoords in Algorithms.GetPointsOnSquareBorder(controllableChunkX,
                             controllableChunkY, SpawnRadius))
                {
                    if (ControllableObjects.All(obj =>
                        {
                            var (objChunkX, objChunkY) = GetChunk(obj.X, obj.Y);
                            return Math.Abs(chunkCoords.x - objChunkX) >= SpawnRadius
                                   && Math.Abs(chunkCoords.y - objChunkY) >= SpawnRadius
                                   && chunkCoords.x >= 0 && chunkCoords.x < _chunks.GetLength(0)
                                   && chunkCoords.y >= 0 && chunkCoords.y < _chunks.GetLength(1);
                        }))
                        yield return chunkCoords;
                }
            }
        }
    }

    private void ControllableMoved(IMovable sender, int x0, int y0, int x1, int y1)
    {
        if (GetChunk(x0, y0) != GetChunk(x1, y1))
        {
            SpawnEnemies();
        }
        if (ReferenceEquals(sender, Player) && Player.X == _finishX && Player.Y == _finishY)
        {
            IsActive = false;
            PlayerWon = true;
        }
    }

    public Tile GetTile(int x, int y)
    {
        switch (x, y)
        {
            case(-1, -1):
                return _cornerTile;
            case(-1, >= 0):
                return y >= _westernWall.Length ? null : _westernWall[y];
            case(>= 0, -1):
                return x >= _southernWall.Length ? null : _southernWall[x];
        }

        var (chunkX, chunkY) = GetChunk(x, y);
        int localX = x % DataHolder.ChunkSize;
        int localY = y % DataHolder.ChunkSize;
        if (!(x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1)))
        {
            return null;
        }

        return _chunks[chunkX, chunkY].Tiles[localX, localY];
    }

    public IObjectOnMap GetNonTileObject(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        var (chunkX, chunkY) = GetChunk(x, y);
        if (!(x >= 0 && chunkX < _chunks.GetLength(0) && y >= 0 && chunkY < _chunks.GetLength(1)))
        {
            return null;
        }
        return _chunks[chunkX, chunkY].GetMapObject(x, y);
    }

    public IDrawable GetDrawable(int x, int y)
    {
        return (IDrawable) GetNonTileObject(x, y) ?? GetTile(x, y);
    }

    private static (int chunkX, int chunkY) GetChunk(int x, int y)
    {
        return (x / DataHolder.ChunkSize, y / DataHolder.ChunkSize);
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

    public bool DoTurn()
    {
        if (!IsActive) return false;
        
        HashSet<Actor> inactive = new();
        HashSet<Actor> affected = new();
        List<Actor> currentlyActive;
        while ((currentlyActive = GetActive()).Any())
        {
            foreach (var actor in currentlyActive.Where(actor => !affected.Contains(actor)))
            {
                Watch(actor);
            }
            foreach (var actor in currentlyActive)
            {
                // A player may have reached the finish or died
                if (!IsActive) return false;
                // An actor could die due to actions of another actor
                if (!inactive.Contains(actor)) actor.DoAction();
            }
        }

        foreach (var actor in affected)
        {
            FinishTurn(actor);
        }
        
        TurnFinished?.Invoke();
        return IsActive;

        void BecameInactiveHandler(Actor sender)
        {
            if (!inactive.Contains(sender)) inactive.Add(sender);
        }

        void Watch(Actor actor)
        {
            actor.Stopped += BecameInactiveHandler;
            actor.Died += BecameInactiveHandler;
            actor.RanOutOfEnergy += BecameInactiveHandler;
            affected.Add(actor);
        }

        void FinishTurn(Actor actor)
        {
            actor.RanOutOfEnergy -= BecameInactiveHandler;
            actor.Stopped -= BecameInactiveHandler;
            actor.Died -= BecameInactiveHandler;
            actor.FinishTurn();
        }
        
        List<Npc> GetRespondingNpcs()
        {
            List <Npc> npcs = new();
            var (chunkX, chunkY) = GetChunk(Player.X, Player.Y);
            foreach (var chunk in GetChunksAround(chunkX, chunkY, EnemyRadiusPlayer))
            {
                npcs.AddRange(chunk.GetNpcs());
            }

            foreach (var controllable in ControllableObjects.Where(controllable => !ReferenceEquals(controllable, Player)))
            {
                (chunkX, chunkY) = GetChunk(controllable.X, controllable.Y);
                foreach (var chunk in GetChunksAround(chunkX, chunkY, EnemyRadiusControllable))
                {
                    npcs.AddRange(chunk.GetNpcs());
                }
            }

            return npcs;
        }

        List<Actor> GetActive()
        {
            List<Actor> active = new List<Actor>(ControllableObjects.OfType<Actor>());
            active.AddRange(GetRespondingNpcs());
            return active.Except(inactive).ToList();
        }
    }
}