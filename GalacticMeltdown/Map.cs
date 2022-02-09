using System.Collections.Generic;
using GalacticMeltdown.data;

namespace GalacticMeltdown;

public partial class Map
{
    public readonly int MapSeed;
    public Player Player { get; }
    private SubMap[,] _map;
    private readonly SubMap _startPoint;
    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;
    public readonly string MapString;//for debugging
    
    public Map(SubMap[,] map, int seed, SubMap startPoint, Tile[] southernWall, 
        Tile[] westernWall, Dictionary<string, TileTypeData> tileTypes, string mapString)
    {
        _cornerTile = new Tile(tileTypes["wall_nesw"]);
        _map = map;
        MapSeed = seed;
        _startPoint = startPoint;
        _southernWall = southernWall;
        _westernWall = westernWall;
        MapString = mapString;
        Player = new Player(_startPoint.MapX * 25 + 12, _startPoint.MapY * 25 + 12, GetTile, GetEntity);
        MeleeEnemy enemy = new MeleeEnemy(_startPoint.MapX * 25 + 15, _startPoint.MapY * 25 + 15, this, Player);
        MeleeEnemy enemy2 = new MeleeEnemy(_startPoint.MapX * 25 + 5, _startPoint.MapY * 25 + 5, this, Player);
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
        if (!(x >= 0 && mapX < _map.GetLength(0) && y >= 0 && mapY < _map.GetLength(1)))
        {
            return null;
        }

        return _map[mapX, mapY].Tiles[localX, localY];
    }

    public IEntity GetEntity(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        int mapX = x / 25;
        int mapY = y / 25;
        if (!(x >= 0 && mapX < _map.GetLength(0) && y >= 0 && mapY < _map.GetLength(1)))
        {
            return null;
        }
        return _map[mapX, mapY].GetEntity(x, y);
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
        if (!(0 <= mapX && mapX < _map.GetLength(0) && 0 <= mapY && mapY < _map.GetLength(1))) return;
        if (mapX != oldMapX || oldY != oldMapY)
        {
            if (oldX >= 0 && oldY >= 0)
            {
                _map[oldMapX, oldMapY].Enemies.Remove(enemy);
            }
            _map[mapX, mapY].Enemies.Add(enemy);
        }
    }
}