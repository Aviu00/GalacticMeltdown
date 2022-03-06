using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown;
public class Chunk
{
    public double Difficulty { get; }
    public Tile[,] Tiles { get; }

    public List<Enemy> Enemies;

    public Chunk(Tile[,] tiles, double difficulty)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        Enemies = new List<Enemy>();
    }

    public IObjectOnMap GetMapObject(int x, int y)
    {
        IObjectOnMap objectOnMap = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        //Check other IObjectOnMap list here
        return objectOnMap;
    }

    public List<Npc> GetNpcs()
    {
        List <Npc> npcs = new();
        npcs.AddRange(Enemies);
        return npcs;
    }
}