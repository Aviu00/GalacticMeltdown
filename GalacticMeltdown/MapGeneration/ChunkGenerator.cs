using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.MapGeneration;

public class ChunkGenerator
{
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

    public Chunk GenerateChunk(TileTypeData[,] roomData, int seed, 
        Tile[,] northernTileMap = null, Tile[,] easternTileMap = null)
    {
        CalculateSymbol();
        Tiles = new Tile[ChunkSize, ChunkSize];
        for (int y = 0; y < ChunkSize - 1; y++)
        {
            for (int x = 0; x < ChunkSize - 1; x++)
            {
                if (roomData[x, y].IsDependingOnRoomConnection) ResolveRoomConnectionDependency(roomData, x, y);
            }
        }

        for (int y = 0; y < ChunkSize - 1; y++)
        {
            for (int x = 0; x < ChunkSize - 1; x++)
            {
                TileTypeData tileTypeData = roomData[x, y].IsConnectable
                    ? GetConnectableData(roomData, x, y)
                    : roomData[x, y];
                Tiles[x, y] = new Tile(tileTypeData);
            }
        }

        FillBorderWalls(roomData, northernTileMap, easternTileMap);
        var neighborCoords = new List<(int x, int y)>();
        if(NorthConnection != null) neighborCoords.Add((NorthConnection.MapX, NorthConnection.MapY));
        if(EastConnection != null) neighborCoords.Add((EastConnection.MapX, EastConnection.MapY));
        if(SouthConnection != null) neighborCoords.Add((SouthConnection.MapX, SouthConnection.MapY));
        if(WestConnection != null) neighborCoords.Add((WestConnection.MapX, WestConnection.MapY));
        return new Chunk(Tiles, neighborCoords, Difficulty, seed, MapX, MapY);
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

    public char CalculateSymbol()//will be used for minimap
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

    private void FillBorderWalls(TileTypeData[,] roomData, Tile[,] northernTileMap, Tile[,] easternTileMap)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            TileTypeData terrainObject = NorthConnection is null || x is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, x, ChunkSize - 1, northernTileMap?[x, 0].ConnectToWalls,
                    overrideId: "wall")
                : DataHolder.TileTypes["floor"]; //will be changed to "door" later
            Tiles[x, ChunkSize - 1] = new Tile(terrainObject);
        }

        for (int y = 0; y < ChunkSize - 1; y++)
        {
            TileTypeData terrainObject = EastConnection is null || y is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, ChunkSize - 1, y,
                    easternTileConnectable: easternTileMap?[0, y].ConnectToWalls, overrideId: "wall")
                : DataHolder.TileTypes["floor"]; //will be changed to "door" later
            Tiles[ChunkSize - 1, y] = new Tile(terrainObject);
        }
    }

    private TileTypeData GetConnectableData(TileTypeData[,] roomData, int x, int y,
        bool? northernTileConnectable = null, bool? easternTileConnectable = null, string overrideId = null)
    {
        string id = overrideId ?? roomData[x, y].Id;
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

    private void ResolveRoomConnectionDependency(TileTypeData[,] roomData, int x, int y)
    {
        string[] parsedId = roomData[x, y].Id.Split('_');
        bool switchId = parsedId[2] switch
        {
            "north" => NorthConnection is null,
            "east" => EastConnection is null,
            "south" => SouthConnection is null,
            "west" => WestConnection is null,
            _ => false
        };

        string newId = !switchId ? parsedId[0] : parsedId[1];
        roomData[x, y] = DataHolder.TileTypes[newId];
    }

    private static bool CheckForConnectionInTile(TileTypeData[,] roomData, int x, int y)
    {
        if (!(x is >= -1 and < ChunkSize && y is >= -1 and < ChunkSize)) return false;
        return x is -1 or ChunkSize - 1 || y is -1 or ChunkSize - 1 || roomData[x, y].IsConnection;
    }
}