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
    [JsonProperty] private readonly List<(int x, int y)> _floorTileCoords;
    
    [JsonProperty] public readonly Tile[,] Tiles;
    [JsonProperty] public readonly List<Npc> Npcs;

    [JsonProperty] public readonly char Symbol;

    [JsonConstructor]
    private Chunk()
    {

    }
    
    public Chunk(Tile[,] tiles, ItemDictionary items, List<(int x, int y)> neighborCoords, int difficulty, int seed,
        int mapX, int mapY, char symbol, bool isFinalRoom = false)
    {
        Tiles = tiles;
        Difficulty = difficulty;
        MapX = mapX;
        MapY = mapY;
        NeighborCoords = neighborCoords;
        Seed = seed;
        _items = items;
        Symbol = symbol;
        IsFinalRoom = isFinalRoom;
        Npcs = new List<Npc>();
        
        List<(int, int)> floorTileCoords = new();
        for (int x = 0; x < ChunkConstants.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkConstants.ChunkSize; y++)
            {
                if (Tiles[x, y].Id == "floor")
                    floorTileCoords.Add((x + MapX * ChunkConstants.ChunkSize, y + MapY * ChunkConstants.ChunkSize));
            }
        }

        _floorTileCoords = floorTileCoords;
    }

    public List<(int, int)> GetFloorTileCoords(bool ignoreObjectsOnMap = true)
    {
        return ignoreObjectsOnMap
            ? _floorTileCoords
            : _floorTileCoords.Where(coords => GetMapObject(coords.x, coords.y) is null).ToList();
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