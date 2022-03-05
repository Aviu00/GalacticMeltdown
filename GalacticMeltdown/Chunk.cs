using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown;
public class Chunk
{
    public double Difficulty { get; }
    public Tile[,] Tiles { get; }
    public int MapX { get; }
    public int MapY { get; }

    public List<Enemy> Enemies = new();

    public Chunk(Tile[,] tiles, double difficulty, int x, int y)
    {
        MapX = x;
        MapY = y;
        Tiles = tiles;
        Difficulty = difficulty;
    }

    public IObjectOnMap GetEntity(int x, int y)
    {
        IObjectOnMap objectOnMap = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        //Check other IObjectOnMap list here
        return objectOnMap;
    }
}