using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown;
public class Chunk
{
    private bool _seededSpawn = true;
    private double Difficulty { get; }
    public Tile[,] Tiles { get; }

    public List<Enemy> Enemies { get; }

    public event MovedEventHandler NpcMoved;
    public event DiedEventHandler NpcDied;

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

    private void MovedHandler(IMovable sender, int x0, int y0, int x1, int y1)
    {
        NpcMoved?.Invoke(sender, x0, y0, x1, y1);
    }

    private void DiedHandler(Actor sender)
    {
        RemoveNpc((Npc) sender);
        NpcDied?.Invoke(sender);
    }

    public List<Npc> GetNpcs()
    {
        List <Npc> npcs = new();
        npcs.AddRange(Enemies);
        return npcs;
    }

    public void AddNpc(Npc npc)
    {
        if (npc is Enemy enemy) Enemies.Add(enemy);
        npc.Moved += MovedHandler;
    }

    public void RemoveNpc(Npc npc)
    {
        if (npc is Enemy enemy) Enemies.Remove(enemy);
        npc.Moved -= MovedHandler;
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
        // Sub to Died and Moved events
    }
}