using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;
using ItemDictionary = Dictionary<(int x, int y), List<Item>>;

public class Chunk
{
    [JsonProperty] public readonly List<(int x, int y)> NeighborCoords;
    [JsonProperty] public readonly int MapX;
    [JsonProperty] public readonly int MapY;

    [JsonProperty] public readonly int Difficulty;
    [JsonProperty] public readonly int Seed;

    public bool WasActive;
    public bool WasVisitedByPlayer;
    [JsonProperty] public readonly bool IsFinalRoom;

    [JsonIgnore] public bool IsActive;
    
    [JsonProperty] private readonly ItemDictionary _items;
    [JsonProperty] public readonly Tile[,] Tiles;
    [JsonProperty] public List<Npc> Npcs { get; }

    [JsonProperty] public readonly char Symbol;

    [JsonConstructor]
    private Chunk()
    {

    }
    
    public Chunk(Tile[,] tiles, ItemDictionary items, List<(int x, int y)> neighborCoords, int difficulty, int seed,
        int x, int y, char symbol, bool isFinalRoom = false)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        MapX = x;
        MapY = y;
        NeighborCoords = neighborCoords;
        Seed = seed;
        _items = items;
        Symbol = symbol;
        IsFinalRoom = isFinalRoom;
        Npcs = new List<Npc>();
    }

    public List<(int, int)> GetFloorTileCoords(bool ignoreObjectsOnMap = true)
    {
        List<(int, int)> coords = new();
        for (int x = 0; x < ChunkConstants.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkConstants.ChunkSize; y++)
            {
                int xGlobal = x + MapX * ChunkConstants.ChunkSize, yGlobal = y + MapY * ChunkConstants.ChunkSize;
                if (Tiles[x, y].Id == "floor" && (ignoreObjectsOnMap || GetMapObject(xGlobal, yGlobal) is null))
                    coords.Add((xGlobal, yGlobal));
            }
        }

        return coords;
    }

    public IDrawable GetDrawable(int x, int y)
    {
        if (GetMapObject(x, y) is { } drawable) return drawable;
        return GetItems(x, y)?.First();
    }

    public IObjectOnMap GetMapObject(int x, int y)
    {
        IObjectOnMap objectOnMap = Npcs.FirstOrDefault(npc => npc.X == x && npc.Y == y);
        return objectOnMap;
    }

    public void AddItem(Item item, int x, int y)
    {
        if (!_items.ContainsKey((x, y)))
        {
            _items[(x, y)] = new List<Item>();
        }

        _items[(x, y)].Add(item);
    }

    public List<Item> GetItems(int x, int y)
    {
        if (!_items.TryGetValue((x, y), out List<Item> items))
            return null;
        return items.Count == 0 ? null : items;
    }
}