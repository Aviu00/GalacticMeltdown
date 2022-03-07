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
    
    public double Difficulty = -1;

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

    public ChunkGenerator GetNextRoom(ChunkGenerator previousRoom)
    {
        if (NorthConnection is not null && NorthConnection != previousRoom) return NorthConnection;
        if (EastConnection is not null && EastConnection != previousRoom) return EastConnection;
        if (SouthConnection is not null && SouthConnection != previousRoom) return SouthConnection;
        return WestConnection!;
    }

    public void AccessMainRoute(double mainRouteDifficulty)
    {
        if (MainRoute || Difficulty >= mainRouteDifficulty) return;

        HasAccessToMainRoute = true;
        Difficulty = mainRouteDifficulty;
        NorthConnection?.AccessMainRoute(Difficulty);
        EastConnection?.AccessMainRoute(Difficulty);
        SouthConnection?.AccessMainRoute(Difficulty);
        WestConnection?.AccessMainRoute(Difficulty);
    }

    public Chunk GenerateChunk(TileTypeData[,] roomData, Tile[,] northernTileMap = null, Tile[,] easternTileMap = null)
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
        return new Chunk(Tiles, Difficulty);
    }

    private void FillBorderWalls(TileTypeData[,] roomData, Tile[,] northernTileMap, Tile[,] easternTileMap)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            TileTypeData terrainObject = NorthConnection is null || x is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, x, ChunkSize - 1, northernTileMap?[x, 0].ConnectToWalls, overrideId: "wall")
                : DataHolder.TileTypes["floor"]; //will be changed to "door" later
            Tiles[x, ChunkSize - 1] = new Tile(terrainObject);
        }

        for (int y = 0; y < ChunkSize - 1; y++)
        {
            TileTypeData terrainObject = EastConnection is null || y is not (ChunkSize / 2 - 1 or ChunkSize / 2)
                ? GetConnectableData(roomData, ChunkSize - 1, y, easternTileConnectable: easternTileMap?[0, y].ConnectToWalls,
                    overrideId: "wall")
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

    private static bool CheckForConnectionInTile(TileTypeData[,] roomData, int x, int y)
    {
        if (!(x is >= -1 and < ChunkSize && y is >= -1 and < ChunkSize)) return false;
        return x is -1 or ChunkSize - 1 || y is -1 or ChunkSize - 1 || roomData[x, y].IsConnection;
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

    public char CalculateSymbol()
    {
        List<char> symbols = new()
        {
            '┼',
            '├',
            '┤',
            '┬',
            '┴',
            '┌',
            '└',
            '┐',
            '┘',
            '│',
            '─',
            '╴',
            '╶',
            '╷',
            '╵',
            '0'
        };
        if (NorthConnection is null)
        {
            symbols.Remove('┼');
            symbols.Remove('├');
            symbols.Remove('┤');
            symbols.Remove('┴');
            symbols.Remove('└');
            symbols.Remove('┘');
            symbols.Remove('│');
            symbols.Remove('╵');
        }

        if (SouthConnection is null)
        {
            symbols.Remove('┼');
            symbols.Remove('├');
            symbols.Remove('┤');
            symbols.Remove('┬');
            symbols.Remove('┌');
            symbols.Remove('┐');
            symbols.Remove('│');
            symbols.Remove('╷');
        }

        if (EastConnection is null)
        {
            symbols.Remove('┼');
            symbols.Remove('├');
            symbols.Remove('┬');
            symbols.Remove('┴');
            symbols.Remove('┌');
            symbols.Remove('└');
            symbols.Remove('─');
            symbols.Remove('╶');
        }

        if (WestConnection is null)
        {
            symbols.Remove('┼');
            symbols.Remove('┤');
            symbols.Remove('┬');
            symbols.Remove('┴');
            symbols.Remove('┐');
            symbols.Remove('┘');
            symbols.Remove('─');
            symbols.Remove('╴');
        }

        return symbols[0];
    }
}