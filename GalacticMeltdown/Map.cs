namespace GalacticMeltdown;

public class Map
{
    public readonly int MapSeed;
    private SubMap[,] _map;
    public readonly SubMap StartPoint;

    public Map(SubMap[,] map, int seed, SubMap startPoint)
    {
        _map = map;
        MapSeed = seed;
        StartPoint = startPoint;
    }


    public Tile GetTile(int x, int y)
    {
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