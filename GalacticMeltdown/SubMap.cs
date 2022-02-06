using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown;
public class SubMap
{
    public double Difficulty { get; }
    public Tile[,] Tiles { get; }
    public int MapX { get; }
    public int MapY { get; }

    public List<Enemy> Enemies = new();

    public SubMap(Tile[,] tiles, double difficulty, int x, int y)
    {
        MapX = x;
        MapY = y;
        Tiles = tiles;
        Difficulty = difficulty;
    }

    public IEntity GetEntity(int x, int y)
    {
        IEntity entity = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        //Check other IEntity list here
        return entity;
    }
}