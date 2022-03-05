using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public delegate void TurnFinishedEventHandler();

public partial class Level
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDex = 16;
    private const int PlayerDef = 4;
    
    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;
    
    private Chunk[,] _chunks;

    private int _finishX;
    private int _finishY;

    public event TurnFinishedEventHandler TurnFinished;
    
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
        Player = new Player(PlayerHp, PlayerEnergy, PlayerDex, PlayerDef, startPos.x, startPos.y, this);
        ControllableObjects = new List<IControllable> { Player };
        SightedObjects = new ObservableCollection<ISightedObject> { Player };
        LevelView = new LevelView(this);
        OverlayView = new OverlayView(this);
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

    public IEntity GetEntity(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        int mapX = x / 25;
        int mapY = y / 25;
        if (!(x >= 0 && mapX < _chunks.GetLength(0) && y >= 0 && mapY < _chunks.GetLength(1)))
        {
            return null;
        }
        return _chunks[mapX, mapY].GetEntity(x, y);
    }

    public IDrawable GetDrawable(int x, int y)
    {
        return (IDrawable) GetEntity(x, y) ?? GetTile(x, y);
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