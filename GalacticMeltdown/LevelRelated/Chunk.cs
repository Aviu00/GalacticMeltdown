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
                if (Tiles[x, y].Id == "floor" && (ignoreObjectsOnMap || GetMapObject(x, y) is null))
                    coords.Add((x + MapX * ChunkConstants.ChunkSize, y + MapY * ChunkConstants.ChunkSize));
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

    public void AddItem(Item item, int localX, int localY)
    {
        if (!_items.ContainsKey((localX, localY)))
        {
            _items[(localX, localY)] = new List<Item>();
        }

        _items[(localX, localY)].Add(item);
    }

    public List<Item> GetItems(int localX, int localY)
    {
        if (!_items.TryGetValue((localX, localY), out List<Item> items))
            return null;
        return items.Count == 0 ? null : items;
    }
}