using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.LevelRelated;

public class Chunk
{
    public event EventHandler<MoveEventArgs> SomethingMoved;
    public event EventHandler NpcDied;
    public event EventHandler NpcInvolvedInTurn;
    
    public int MapX { get; }
    public int MapY { get; }
    
    private double Difficulty { get; }
    public Tile[,] Tiles { get; }

    private List<Enemy> Enemies { get; }
    
    private bool _wasActiveBefore;

    public List<(int x, int y)> NeighborCoords { get; }

    public bool WasActiveBefore
    {
        get => _wasActiveBefore;
        set
        {
            if(!_wasActiveBefore && value) SpawnEnemies();
            _wasActiveBefore = value;
        }
    }

    public Chunk(Tile[,] tiles, List<(int x, int y)> neighborCoords, double difficulty, int x, int y)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        MapX = x;
        MapY = y;
        NeighborCoords = neighborCoords;
        Enemies = new List<Enemy>();
    }

    public List<(int, int)> GetFloorTileCoords()
    {
        List<(int, int)> coords = new();
        for (int x = 0; x < DataHolder.ChunkSize; x++)
        {
            for (int y = 0; y < DataHolder.ChunkSize; y++)
            {
                if (Tiles[x, y].Id == "floor")
                    coords.Add((x + MapX * DataHolder.ChunkSize, y + MapY * DataHolder.ChunkSize));
            }
        }
        return coords;
    }

    public void SpawnEnemies()
    {
        
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