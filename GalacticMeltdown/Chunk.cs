using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown;
public class Chunk
{
    private bool _seededSpawn = true;
    private double Difficulty { get; }
    public Tile[,] Tiles { get; }

    public List<Enemy> Enemies { get; }

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

    public void SuggestEnemySpawn()
    {
        if (_seededSpawn)
        {
            // use DataHolder.CurrentSeed and Difficulty
            _seededSpawn = false;
        }
        else
        {
            // spawn with some low-ish chance
        }
    }
}