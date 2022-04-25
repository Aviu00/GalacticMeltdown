using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.MapGeneration;
using ItemDictionary = Dictionary<(int x, int y), List<Item>>;

public class ChunkGenerator
{
    private const double RandomFuncNorm = 500;
        
    private Random _rng;
    private const int ChunkSize = DataHolder.ChunkSize;

    public bool HasAccessToMainRoute;
    public bool MainRoute;

    public ChunkGenerator NorthConnection = null;
    public ChunkGenerator EastConnection = null;
    public ChunkGenerator SouthConnection = null;
    public ChunkGenerator WestConnection = null;

    public bool IsStartPoint;
    public bool IsEndPoint;

    public int Difficulty = -1;

    public int ConnectionCount =>
        Convert.ToInt32(NorthConnection is not null)
        + Convert.ToInt32(EastConnection is not null)
        + Convert.ToInt32(SouthConnection is not null)
        + Convert.ToInt32(WestConnection is not null);

    public Tile[,] Tiles { get; private set; }
    public int MapX { get; }
    public int MapY { get; }

    public ChunkGenerator(int x, int y)
    {
        MapX = x;
        MapY = y;
    }

    public Chunk GenerateChunk(TileInformation[,] roomData, int seed,
        Tile[,] northernTileMap = null, Tile[,] easternTileMap = null)
    {
        _rng = new Random(seed);
        ItemDictionary items = new();
        Tiles = new Tile[ChunkSize, ChunkSize];
        for (int y = 0; y < ChunkSize - 1; y++)
        {
            for (int x = 0; x < ChunkSize - 1; x++)
            {
                if (roomData[x, y].TileTypeData.IsDependingOnRoomConnection)
                    ResolveRoomConnectionDependency(roomData, x, y);
            }
        }

        for (int y = 0; y < ChunkSize - 1; y++)
        {
            for (int x = 0; x < ChunkSize - 1; x++)
            {
                TileTypeData tileTypeData = roomData[x, y].TileTypeData.IsConnectable
                    ? GetConnectableData(roomData, x, y)
                    : roomData[x, y].TileTypeData;
                Tiles[x, y] = new Tile(tileTypeData);
                CalculateLoot(items, roomData, x, y);
            }
        }

        FillBorderWalls(roomData, northernTileMap, easternTileMap);
        return new Chunk(Tiles, items, GetNeighborCoords(), Difficulty, seed, MapX, MapY, CalculateSymbol(), IsEndPoint);
    }

    private List<(int x, int y)> GetNeighborCoords()
    {
        var neighborCoords = new List<(int x, int y)>();
        if (NorthConnection != null) neighborCoords.Add((NorthConnection.MapX, NorthConnection.MapY));
        if (EastConnection != null) neighborCoords.Add((EastConnection.MapX, EastConnection.MapY));
        if (SouthConnection != null) neighborCoords.Add((SouthConnection.MapX, SouthConnection.MapY));
        if (WestConnection != null) neighborCoords.Add((WestConnection.MapX, WestConnection.MapY));
        return neighborCoords;
    }

    public void AccessMainRoute(int mainRouteDifficulty)
    {
        if (MainRoute || Difficulty >= mainRouteDifficulty) return;

        HasAccessToMainRoute = true;
        Difficulty = mainRouteDifficulty;
        NorthConnection?.AccessMainRoute(Difficulty);
        EastConnection?.AccessMainRoute(Difficulty);
        SouthConnection?.AccessMainRoute(Difficulty);
        WestConnection?.AccessMainRoute(Difficulty);
    }

    public ChunkGenerator GetNextRoom(ChunkGenerator previousRoom)
    {
        if (NorthConnection is not null && NorthConnection != previousRoom) return NorthConnection;
        if (EastConnection is not null && EastConnection != previousRoom) return EastConnection;
        if (SouthConnection is not null && SouthConnection != previousRoom) return SouthConnection;
        return WestConnection!;
    }

    public char CalculateSymbol() //will be used for minimap
    {
        return (NorthConnection, EastConnection, SouthConnection, WestConnection) switch
        {
            (not null, not null, not null, not null) => '┼',
            (not null, not null, not null, null) => '├',
            (not null, null, not null, not null) => '┤',
            (null, not null, not null, not null) => '┬',
            (not null, not null, null, not null) => '┴',
            (null, not null, not null, null) => '┌',
            (not null, not null, null, null) => '└',
            (null, null, not null, not null) => '┐',
            (not null, null, null, not null) => '┘',
            (not null, null, not null, null) => '│',
            (null, not null, null, not null) => '─',
            (null, null, null, not null) => '╴',
            (null, not null, null, null) => '╶',
            (null, null, not null, null) => '╷',
            (not null, null, null, null) => '╵',
            _ => ' '
        };
    }

    private void CalculateLoot(ItemDictionary items, TileInformation[,] roomData, int localX, int localY)
    {
        string id = roomData[localX, localY].LootId;
        if (id == null ||
            !UtilityFunctions.Chance(ChanceFunction(roomData[localX, localY].LootChance, roomData[localX, localY].Gain,
                roomData[localX, localY].Limit), _rng))
            return;
        int newX = localX + MapX * DataHolder.ChunkSize;
        int newY = localY + MapY * DataHolder.ChunkSize;
        SpawnItems(items, id, newX, newY);
    }

    private void SpawnItems(ItemDictionary items, string id, int x, int y)
    {
        if (DataHolder.ItemTypes.ContainsKey(id))
        {
            AddItem(items, DataHolder.ItemTypes[id], x, y);
            return;
        }

        if (DataHolder.LootTables[id] is ItemLoot)
        {
            ItemLoot itemLoot = (ItemLoot) DataHolder.LootTables[id];
            int amount = GetLimitedValue(itemLoot.Limit, 
                _rng.Next(itemLoot.Min, itemLoot.Max + (int) (Difficulty * itemLoot.Gain) + 1));
            if (amount <= 0)
                return;
            for (; amount > 0; amount--)
            {
                AddItem(items, DataHolder.ItemTypes[itemLoot.ItemId], x, y);
            }
            return;
        }

        LootTable table = (LootTable) DataHolder.LootTables[id];
        if (!table.IsCollection)
        {
            int[] chances =
                table.Items.Select(item => ChanceFunction(item.chance, item.gain, item.limit)).ToArray();
            int index = UtilityFunctions.MultiChance(chances, _rng);
            if (index == chances.Length) return;
            SpawnItems(items, table.Items[index].lootId, x, y);
            return;
        }

        foreach (var itemObj in table.Items)
        {
            if (!UtilityFunctions.Chance(ChanceFunction(itemObj.chance, itemObj.gain, itemObj.limit), _rng)) continue;
            SpawnItems(items, itemObj.lootId, x, y);
        }
    }
    
    private static void AddItem(ItemDictionary items, ItemData data, int x, int y)
    {
        if (!items.ContainsKey((x, y)))
        {
            items[(x, y)] = new List<Item>();
        }

        items[(x, y)].Add(Item.CreateItem(data));
    }

    private int GetLimitedValue(int limit, int value)
    {
        return limit == -1 ? value : Math.Min(limit, value);
    }
    private int ChanceFunction(int chance, double gain, int limit)
    {
        if (chance == limit || gain == 0) return chance;
        return (int) (limit - 1 / (gain * Difficulty / RandomFuncNorm + 1 / (double)(limit - chance)));
    }

    private void FillBorderWalls(TileInformation[,] roomData, Tile[,] northernTileMap,
        Tile[,] easternTileMap)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            TileTypeData terrainObject = NorthConnection is null || x is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, x, ChunkSize - 1, northernTileMap?[x, 0].ConnectToWalls,
                    overrideId: "wall")
                : DataHolder.TileTypes["door-closed"];
            Tiles[x, ChunkSize - 1] = new Tile(terrainObject);
        }

        for (int y = 0; y < ChunkSize - 1; y++)
        {
            TileTypeData terrainObject = EastConnection is null || y is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, ChunkSize - 1, y,
                    easternTileConnectable: easternTileMap?[0, y].ConnectToWalls, overrideId: "wall")
                : DataHolder.TileTypes["door-closed"];
            Tiles[ChunkSize - 1, y] = new Tile(terrainObject);
        }
    }

    private TileTypeData GetConnectableData(TileInformation[,] roomData, int x, int y,
        bool? northernTileConnectable = null, bool? easternTileConnectable = null, string overrideId = null)
    {
        string id = overrideId ?? roomData[x, y].TileTypeData.Id;
        string newId = id + "_";
        if ((x, y) is (ChunkSize - 1, ChunkSize - 1)) return DataHolder.TileTypes[newId + "nesw"];
        StringBuilder wallKey = new StringBuilder(newId);
        if (northernTileConnectable ?? CheckForConnectionInTile(roomData, x, y + 1)) wallKey.Append('n');
        if (easternTileConnectable ?? CheckForConnectionInTile(roomData, x + 1, y)) wallKey.Append('e');
        if (CheckForConnectionInTile(roomData, x, y - 1)) wallKey.Append('s');
        if (CheckForConnectionInTile(roomData, x - 1, y)) wallKey.Append('w');
        string str = wallKey.ToString();
        if (str[^1] == '_') str = id;
        return DataHolder.TileTypes[str];
    }

    private void ResolveRoomConnectionDependency(TileInformation[,] tileInfo, int x, int y)
    {
        string[] parsedId = tileInfo[x, y].TileTypeData.Id.Split('_');
        bool switchId = parsedId[2] switch
        {
            "north" => NorthConnection is null,
            "east" => EastConnection is null,
            "south" => SouthConnection is null,
            "west" => WestConnection is null,
            _ => false
        };

        string newId = !switchId ? parsedId[0] : parsedId[1];
        tileInfo[x, y].TileTypeData = DataHolder.TileTypes[newId];
    }

    private static bool CheckForConnectionInTile(TileInformation[,] roomData, int x, int y)
    {
        if (!(x is >= -1 and < ChunkSize && y is >= -1 and < ChunkSize)) return false;
        return x is -1 or ChunkSize - 1 || y is -1 or ChunkSize - 1 || roomData[x, y].TileTypeData.IsConnection;
    }
}