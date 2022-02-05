using System.Collections.Generic;
using GalacticMeltdown.data;

namespace GalacticMeltdown;

public class Map
{
    public readonly int MapSeed;
    public Player Player { get; }
    private SubMap[,] _map;
    public readonly SubMap StartPoint;
    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;
    public readonly string MapString;//for debugging

    private event Player.TakeAction OnPlayerMove;
    public Map(SubMap[,] map, int seed, SubMap startPoint, Tile[] southernWall, 
        Tile[] westernWall, Dictionary<string, TileTypeData> tileTypes, string mapString)
    {
        _cornerTile = new Tile(tileTypes["wall_nesw"]);
        _map = map;
        MapSeed = seed;
        StartPoint = startPoint;
        _southernWall = southernWall;
        _westernWall = westernWall;
        MapString = mapString;
        Player = new Player(StartPoint.MapX * 25 + 12, StartPoint.MapY * 25 + 12, GetTile);
        Enemy enemy = new MeleeEnemy(StartPoint.MapX * 25 + 13, StartPoint.MapY * 25 + 13, this, Player);
    }


    public Tile GetTile(int x, int y)//remove this method late
    {
        return GetTile(x, y, null, null);
    }
    public Tile GetTile(int x, int y, int? mapX/* = null*/, int? mapY/* = null*/)
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
        mapX ??= x / 25;
        mapY ??= y / 25;
        int localX = x % 25;
        int localY = y % 25;
        if (x < 0 || mapX >= _map.GetLength(0) || y < 0 || mapY >= _map.GetLength(1))
        {
            return null;
        }

        return _map[mapX.Value, mapY.Value].Tiles[localX, localY];
    }

    public IEntity GetEntity(int x, int y, int? mapX = null, int? mapY = null)
    {
        mapX ??= x / 25;
        mapY ??= y / 25;
        if (x < 0 || mapX >= _map.GetLength(0) || y < 0 || mapY >= _map.GetLength(1))
        {
            return null;
        }
        return _map[mapX.Value, mapY.Value].GetEntity(x, y);
    }

    public IDrawable GetDrawable(int x, int y)
    {
        int mapX = x / 25;
        int mapY = y / 25;
        return (IDrawable)GetEntity(x, y, mapX, mapY) ?? GetTile(x, y, mapX, mapY);
    }

    public void UpdateEnemyPosition(Enemy enemy, int oldX, int oldY)
    {
        int mapX = enemy.X / 25;
        int mapY = enemy.Y / 25;
        int oldMapX = oldX / 25;
        int oldMapY = oldY / 25;
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