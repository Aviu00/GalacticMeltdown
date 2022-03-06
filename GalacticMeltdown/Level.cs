using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public delegate void TurnFinishedEventHandler();

public partial class Level
{
    private const int EnemyRadiusPlayer = 3;
    private const int EnemyRadiusControllable = 1;
    
    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;
    
    private Chunk[,] _chunks;

    private int _finishX;
    private int _finishY;

    public event TurnFinishedEventHandler TurnFinished;

    public bool IsActive { get; private set; }

    public Player Player { get; }
    public LevelView LevelView { get; }
    public OverlayView OverlayView { get; }
    
    public List<IControllable> ControllableObjects { get; }
    public ObservableCollection<ISightedObject> SightedObjects { get; }

    public Level(Chunk[,] chunks, (int x, int y) startPos, Tile[] southernWall, Tile[] westernWall, (int x, int y) finishPos)
    {
        _cornerTile = new Tile(DataHolder.TileTypes["wall_nesw"]);
        _chunks = chunks;
        _southernWall = southernWall;
        _westernWall = westernWall;
        (_finishX, _finishY) = finishPos;
        Player = new Player(startPos.x, startPos.y, this);
        ControllableObjects = new List<IControllable> { Player };
        SightedObjects = new ObservableCollection<ISightedObject> { Player };
        LevelView = new LevelView(this);
        OverlayView = new OverlayView(this);
        IsActive = true;
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
        int mapX = x / 25;
        int mapY = y / 25;
        int localX = x % 25;
        int localY = y % 25;
        if (!(x >= 0 && mapX < _chunks.GetLength(0) && y >= 0 && mapY < _chunks.GetLength(1)))
        {
            return null;
        }

        return _chunks[mapX, mapY].Tiles[localX, localY];
    }

    public IObjectOnMap GetNonTileObject(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        int mapX = x / 25;
        int mapY = y / 25;
        if (!(x >= 0 && mapX < _chunks.GetLength(0) && y >= 0 && mapY < _chunks.GetLength(1)))
        {
            return null;
        }
        return _chunks[mapX, mapY].GetMapObject(x, y);
    }

    public IDrawable GetDrawable(int x, int y)
    {
        return (IDrawable) GetNonTileObject(x, y) ?? GetTile(x, y);
    }

    private static (int chunkX, int chunkY) GetChunk(int x, int y)
    {
        return (x / 25, y / 25);
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
            List<Actor> active = new List<Actor>(ControllableObjects.FindAll(obj => obj is Actor)
                .ConvertAll(obj => (Actor) obj));
            active.AddRange(GetRespondingNpcs());
            return active.Except(inactive).ToList();
        }
    }

    public void UpdateEnemyPosition(Enemy enemy, int oldX, int oldY)
    {
        int mapX = enemy.X / 25;
        int mapY = enemy.Y / 25;
        int oldMapX = oldX / 25;
        int oldMapY = oldY / 25;
        if (!(0 <= mapX && mapX < _chunks.GetLength(0) && 0 <= mapY && mapY < _chunks.GetLength(1))) return;
        if (mapX != oldMapX || oldY != oldMapY)
        {
            if (oldX >= 0 && oldY >= 0)
            {
                _chunks[oldMapX, oldMapY].Enemies.Remove(enemy);
            }
            _chunks[mapX, mapY].Enemies.Add(enemy);
        }
    }
}