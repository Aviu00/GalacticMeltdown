using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.data;

namespace GalacticMeltdown;

public class SubMapGenerator
{
    public bool HasAccessToMainRoute;
    public bool MainRoute;
    public SubMapGenerator NorthConnection = null;
    public SubMapGenerator EastConnection = null;
    public SubMapGenerator SouthConnection = null;
    public SubMapGenerator WestConnection = null;
    public bool IsStartPoint;
    public bool IsEndPoint;
    public double Difficulty = -1;
    private readonly Dictionary<string, TileTypeData> _tileTypes;

    public int ConnectionCount =>
        Convert.ToInt32(NorthConnection != null) + Convert.ToInt32(EastConnection != null) +
        Convert.ToInt32(SouthConnection != null) + Convert.ToInt32(WestConnection != null);

    public Tile[,] Tiles { get; private set; }
    public int MapX { get; }
    public int MapY { get; }

    public SubMapGenerator(int x, int y, Dictionary<string, TileTypeData> tileTypes)
    {
        _tileTypes = tileTypes;
        MapX = x;
        MapY = y;
    }

    public SubMapGenerator
        GetNextRoom(SubMapGenerator previousRoom) //used in map generation, for main route calculations
    {
        if (NorthConnection != null && NorthConnection != previousRoom)
            return NorthConnection;
        if (EastConnection != null && EastConnection != previousRoom)
            return EastConnection;
        if (SouthConnection != null && SouthConnection != previousRoom)
            return SouthConnection;
        return WestConnection!;
    }

    public void AccessMainRoute(double mainRouteDifficulty) //used in map generation
    {
        if (MainRoute)
            return;
        if (Difficulty < mainRouteDifficulty)
            Difficulty = mainRouteDifficulty;
        else
            return;
        HasAccessToMainRoute = true;
        NorthConnection?.AccessMainRoute(Difficulty);
        EastConnection?.AccessMainRoute(Difficulty);
        SouthConnection?.AccessMainRoute(Difficulty);
        WestConnection?.AccessMainRoute(Difficulty);
    }

    public SubMap GenerateSubMap
        (TileTypeData[,] roomData, Tile[,] northernTileMap = null, Tile[,] easternTileMap = null)
    {
        CalculateSymbol();
        Tiles = new Tile[25, 25];
        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                if (roomData[x, y].IsDependingOnRoomConnection)
                    ResolveRoomConnectionDependency(roomData, x, y);
            }
        }

        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                TileTypeData tileTypeData = roomData[x, y].IsConnectable
                    ? GetConnectableData(roomData, x, y)
                    : roomData[x, y];
                Tiles[x, y] = new Tile(tileTypeData);
            }
        }

        FillBorderWalls(roomData, northernTileMap, easternTileMap);
        return new SubMap(Tiles, Difficulty, MapX, MapY);
    }

    private void FillBorderWalls(TileTypeData[,] roomData, Tile[,] northernTileMap, Tile[,] easternTileMap)
    {
        for (int x = 0; x < 25; x++)
        {
            TileTypeData terrainObject = NorthConnection == null || x is not (11 or 12)
                ? GetConnectableData(roomData, x, 24, northernTileMap?[x, 0].ConnectToWalls, overrideId: "wall")
                : _tileTypes["floor"]; //will be changed to "door" later
            Tiles[x, 24] = new Tile(terrainObject);
        }

        for (int y = 0; y < 24; y++)
        {
            TileTypeData terrainObject = EastConnection == null || y is not (11 or 12)
                ? GetConnectableData(roomData, 24, y,
                    easternTileConnectable: easternTileMap?[0, y].ConnectToWalls, overrideId: "wall")
                : _tileTypes["floor"]; //will be changed to "door" later
            Tiles[24, y] = new Tile(terrainObject);
        }
    }

    private TileTypeData GetConnectableData(TileTypeData[,] roomData, int x, int y,
        bool? northernTileConnectable = null, bool? easternTileConnectable = null, string overrideId = null)
    {
        string id = overrideId ?? roomData[x, y].Id;
        string newId = id + "_";
        if ((x, y) is (24, 24))
            return _tileTypes[newId + "nesw"];
        StringBuilder wallKey = new StringBuilder(newId);
        if (northernTileConnectable ?? CheckForConnectionInTile(roomData, x, y + 1))
            wallKey.Append('n');
        if (easternTileConnectable ?? CheckForConnectionInTile(roomData, x + 1, y))
            wallKey.Append('e');
        if (CheckForConnectionInTile(roomData, x, y - 1))
            wallKey.Append('s');
        if (CheckForConnectionInTile(roomData, x - 1, y))
            wallKey.Append('w');
        string str = wallKey.ToString();
        if (str[^1] == '_')
            str = id;
        return _tileTypes[str];
    }

    private static bool CheckForConnectionInTile(TileTypeData[,] roomData, int x, int y)
    {
        if (x is < -1 or > 24 || y is < -1 or > 24)
            return false;
        return x is -1 or 24 || y is -1 or 24 || roomData[x, y].IsConnection;
    }

    private void ResolveRoomConnectionDependency(TileTypeData[,] roomData, int x, int y)
    {
        string[] parsedId = roomData[x, y].Id.Split('_');
        bool switchId = parsedId[2] switch
        {
            "north" => NorthConnection == null,
            "east" => EastConnection == null,
            "south" => SouthConnection == null,
            "west" => WestConnection == null,
            _ => false
        };

        string newId = !switchId ? parsedId[0] : parsedId[0] == "wall" ? "floor" : "wall";
        roomData[x, y] = _tileTypes[newId];
    }
    
    public char CalculateSymbol()
    {
        List<char> symbols = new()
            {'┼', '├', '┤', '┬', '┴', '┌', '└', '┐', '┘', '│', '─', '╴', '╶', '╷', '╵','0'};
        if (NorthConnection == null)
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
        if (SouthConnection == null)
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
        if (EastConnection == null)
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
        if (WestConnection == null)
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