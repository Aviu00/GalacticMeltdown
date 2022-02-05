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
        if (x < 0 || mapX >= _map.GetLength(0) || y < 0 || mapY >= _map.GetLength(1))
        {
            return null;
        }

        return _map[mapX, mapY].Tiles[localX, localY];
    }
}