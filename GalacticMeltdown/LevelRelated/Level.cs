using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Actors;
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
    private const int EnemyRadiusPlayer = 3;
    private const int EnemyRadiusControllable = 1;

    private const int SpawnRadius = 5;

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

    private readonly ChunkEventListener _listener;

    public Player Player { get; }
    public LevelView LevelView { get; }
    public OverlayView OverlayView { get; }

    public ObservableCollection<IControllable> ControllableObjects { get; }
    public ObservableCollection<ISightedObject> SightedObjects { get; }

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
        LevelView = new LevelView(this);
        OverlayView = new OverlayView(this);
        IsActive = true;
        PlayerWon = false;
    }

    private void SomethingMovedHandler(object sender, MoveEventArgs e)
    {
        if (sender is Npc npc)
        {
            var chunk0 = GetChunk(e.X0, e.Y0);
            var chunk1 = GetChunk(e.X1, e.Y1);
            if (chunk0 != chunk1)
            {
                _chunks[chunk0.chunkX, chunk0.chunkY].RemoveNpc(npc);
                _chunks[chunk1.chunkX, chunk1.chunkY].AddNpc(npc);
            }
        }

        SomethingMoved?.Invoke(sender, e);
    }

    private void NpcDeathHandler(object npc, EventArgs _) => NpcDied?.Invoke(npc, EventArgs.Empty);

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

    private void PlayerDiedHandler(object _, EventArgs __)
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
                foreach (var chunkCoords in Algorithms.GetPointsOnSquareBorder(controllableChunkX, controllableChunkY,
                             SpawnRadius))
                {
                    if (ControllableObjects.All(obj =>
                        {
                            var (objChunkX, objChunkY) = GetChunk(obj.X, obj.Y);
                            return chunkCoords.x >= 0 && chunkCoords.x < _chunks.GetLength(0) 
                                && chunkCoords.y >= 0 && chunkCoords.y < _chunks.GetLength(1)
                                && Math.Abs(chunkCoords.x - objChunkX) >= SpawnRadius
                                && Math.Abs(chunkCoords.y - objChunkY) >= SpawnRadius;
                        }))
                        yield return chunkCoords;
                }
            }
        }
    }

    private void ControllableMoved(object sender, MoveEventArgs e)
    {
        if (GetChunk(e.X0, e.Y0) != GetChunk(e.X1, e.Y1))
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
            case (-1, -1):
                return _cornerTile;
            case (-1, >= 0):
                return y >= _westernWall.Length ? null : _westernWall[y];
            case (>= 0, -1):
                return x >= _southernWall.Length ? null : _southernWall[x];
        }

        var (chunkX, chunkY) = GetChunk(x, y);
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
        return (x / ChunkSize, y / ChunkSize);
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
                if (actor.IsActive) actor.DoAction();
            }

            if (!energySpent) return FinishMapTurn(); // avoid infinite loop when no actor does anything

            energySpent = false;
        }

        return FinishMapTurn();

        void SpentEnergyHandler(object sender, EventArgs _) => energySpent = true;

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

        void NpcInvolvedInTurnHandler(object npc, EventArgs _)
        {
            if (!involved.Contains(npc)) involved.Add((Npc) npc);
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

        List<Npc> GetNearbyNpcs()
        {
            List<Npc> npcs = new();
            var (chunkX, chunkY) = GetChunk(Player.X, Player.Y);
            foreach (var chunk in GetChunksAround(chunkX, chunkY, EnemyRadiusPlayer))
            {
                npcs.AddRange(chunk.GetNpcs());
            }

            foreach (var controllable in ControllableObjects.Where(controllable =>
                         !ReferenceEquals(controllable, Player)))
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
            active.AddRange(GetNearbyNpcs());
            foreach (var npc in GetNearbyNpcs())
            {
                if (!involved.Contains(npc)) involved.Add(npc);
                if (npc.IsActive) active.Add(npc);
            }

            return active;
        }
    }
}