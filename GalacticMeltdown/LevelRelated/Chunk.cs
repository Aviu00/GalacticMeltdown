using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.LevelRelated;

public class Chunk
{
    private bool _seededSpawn = true;
    
    public event EventHandler<MoveEventArgs> SomethingMoved;
    public event EventHandler NpcDied;
    public event EventHandler NpcInvolvedInTurn;
    
    private double Difficulty { get; }
    public Tile[,] Tiles { get; }

    private List<Enemy> Enemies { get; }

    public Chunk(Tile[,] tiles, double difficulty)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        Enemies = new List<Enemy>();
    }
    
    public void SuggestEnemySpawn()
    {
        // use AddNpc
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

    public IObjectOnMap GetMapObject(int x, int y)
    {
        IObjectOnMap objectOnMap = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        //Check other IObjectOnMap list here
        return objectOnMap;
    }
    
    public List<Npc> GetNpcs()
    {
        List<Npc> npcs = new();
        npcs.AddRange(Enemies);
        return npcs;
    }
    
    public void AddNpc(Npc npc)
    {
        if (npc is Enemy enemy) Enemies.Add(enemy);
        npc.Moved += MovedHandler;
        npc.Died += DiedHandler;
        npc.InvolvedInTurn += InvolvedInTurnHandler;
    }

    public void RemoveNpc(Npc npc)
    {
        if (npc is Enemy enemy) Enemies.Remove(enemy);
        npc.Moved -= MovedHandler;
        npc.Died -= DiedHandler;
        npc.InvolvedInTurn -= InvolvedInTurnHandler;
    }

    private void MovedHandler(object sender, MoveEventArgs e)
    {
        SomethingMoved?.Invoke(sender, e);
    }

    private void DiedHandler(object sender, EventArgs _)
    {
        RemoveNpc((Npc) sender);
        NpcDied?.Invoke(sender, EventArgs.Empty);
    }

    private void InvolvedInTurnHandler(object sender, EventArgs e)
    {
        NpcInvolvedInTurn?.Invoke(sender, e);
    }
}