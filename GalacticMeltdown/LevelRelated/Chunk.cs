using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;
using ItemDictionary = Dictionary<(int x, int y), Dictionary<Item, int>>;

public class Chunk
{
    public event EventHandler<MoveEventArgs> SomethingMoved;
    public event EventHandler NpcDied;
    public event EventHandler NpcInvolvedInTurn;

    [JsonProperty] public readonly List<(int x, int y)> NeighborCoords;
    [JsonProperty] public readonly int MapX;
    [JsonProperty] public readonly int MapY;

    [JsonProperty] public readonly int Difficulty;
    [JsonProperty] public readonly int Seed;

    public bool WasActiveBefore;


    [JsonIgnore] public bool IsActive;
    
    [JsonProperty] private readonly ItemDictionary _items;
    [JsonProperty] public readonly Tile[,] Tiles;
    [JsonProperty] public readonly List<Enemy> Enemies;

    [JsonConstructor]
    private Chunk()
    {
    }
    
    public Chunk(Tile[,] tiles, ItemDictionary items, List<(int x, int y)> neighborCoords, int difficulty, int seed,
        int x, int y)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        MapX = x;
        MapY = y;
        NeighborCoords = neighborCoords;
        Seed = seed;
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

    public IDrawable GetDrawable(int x, int y)
    {
        IDrawable drawable = GetMapObject(x, y);
        if (drawable is not null) return drawable;
        drawable = FindItem(x, y);
        return drawable;
    }

    public IObjectOnMap GetMapObject(int x, int y)
    {
        IObjectOnMap objectOnMap = Enemies.FirstOrDefault(enemy => enemy.X == x && enemy.Y == y);
        return objectOnMap;
    }

    public void AddItem(ItemData data, int amount, int x, int y)
    {
        UtilityFunctions.AddItemOnMap(_items, data, amount, x, y);
    }
    public void AddItem(Item item, int amount, int x, int y, bool copy)
    {
        UtilityFunctions.AddItemOnMap(_items, item, amount, x, y, copy);
    }
    public Item FindItem(int x, int y)
    {
        if (!_items.ContainsKey((x, y))) return null;
        Dictionary<Item, int> itemList = _items[(x, y)];
        if (itemList == null || itemList.Count == 0) return null;
        return itemList.Keys.First();
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