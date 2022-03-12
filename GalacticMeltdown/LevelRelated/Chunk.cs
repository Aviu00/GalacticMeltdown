using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.LevelRelated;

public class Chunk
{
    public event EventHandler<MoveEventArgs> SomethingMoved;
    public event EventHandler NpcDied;
    public event EventHandler NpcInvolvedInTurn;

    public readonly int MapX;
    public readonly int MapY;

    public readonly int Difficulty;
    public readonly Tile[,] Tiles;
    public readonly Random Rng;

    private List<Enemy> Enemies { get; }

    private List<ItemObject> _items;

    public bool WasActiveBefore;

    public readonly List<(int x, int y)> NeighborCoords;

    public Chunk(Tile[,] tiles, List<ItemObject> items, List<(int x, int y)> neighborCoords, int difficulty, Random rng,
        int x, int y)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        MapX = x;
        MapY = y;
        NeighborCoords = neighborCoords;
        Rng = rng;
        _items = items;
        Enemies = new List<Enemy>();
    }

    public List<(int, int)> GetFloorTileCoords(bool ignoreObjectsOnMap = true)
    {
        List<(int, int)> coords = new();
        for (int x = 0; x < DataHolder.ChunkSize; x++)
        {
            for (int y = 0; y < DataHolder.ChunkSize; y++)
            {
                if (Tiles[x, y].Id == "floor" && (ignoreObjectsOnMap || GetMapObject(x, y) is null))
                    coords.Add((x + MapX * DataHolder.ChunkSize, y + MapY * DataHolder.ChunkSize));
            }
        }

        return coords;
    }

    public IObjectOnMap GetMapObject(int x, int y)
    {
        IObjectOnMap objectOnMap = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        if (objectOnMap is not null) return objectOnMap;
        objectOnMap = _items.FirstOrDefault(item => item.X == x && item.Y == y);
        //Check other IObjectOnMap list here
        return objectOnMap;
    }

    public void AddItem(ItemData data, int amount, int x, int y)
    {
        ItemObject itemObject =
            _items.FirstOrDefault(item => item.X == x && item.Y == y && item.ItemData.Id == data.Id);
        if (itemObject != null)
        {
            itemObject.Amount.Value += amount;
            return;
        }

        _items.Add(new ItemObject(data, amount, x, y));
    }

    /// <summary>
    /// This method is not final!!!
    /// </summary>
    public Item GetItem(ItemData data, int amount, int x, int y)
    {
        ItemObject itemObject = _items.FirstOrDefault(item => item.X == x && item.Y == y && item.ItemData == data);
        if (itemObject == null) return null;
        int newAmount = itemObject.Amount.Value - amount;
        int returnAmount;
        if (newAmount < 0)
        {
            returnAmount = itemObject.Amount.Value;
            _items = _items.Where(item => item.ItemData != data || item.X != x && item.Y != y).ToList(); //remove item
        }
        else
        {
            returnAmount = amount;
            if (newAmount == 0) //remove item
                _items = _items.Where(item => item.ItemData != data || item.X != x && item.Y != y).ToList();
            else
                itemObject.Amount.Value = newAmount;
        }

        return new Item(data, returnAmount);
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