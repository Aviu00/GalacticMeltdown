using GalacticMeltdown.data;

namespace GalacticMeltdown;

public class Map
{
    public readonly int MapSeed;
    private SubMap[,] _map;
    public readonly SubMap StartPoint;
    private Tile[] _southernWall;
    private Tile[] _westernWall;
    private Tile _cornerTile = new Tile(GameManager.TileTypesExtractor.TileTypes["wall_nesw"]);

    public Map(SubMap[,] map, int seed, SubMap startPoint, Tile[] southernWall, Tile[] westernWall)
    {
        _map = map;
        MapSeed = seed;
        StartPoint = startPoint;
        _southernWall = southernWall;
        _westernWall = westernWall;
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