using System;
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
    public bool StartPoint;
    public bool EndPoint;
    public double Difficulty = -1;

    public int ConnectionCount =>
        Convert.ToInt32(NorthConnection == null) + Convert.ToInt32(EastConnection == null) +
        Convert.ToInt32(SouthConnection == null) + Convert.ToInt32(WestConnection == null);

    public Tile[,] Tiles { get; private set; }
    public int MapX { get; }
    public int MapY { get; }

    public SubMapGenerator(int x, int y)
    {
        MapX = x;
        MapY = y;
    }
        
    public SubMapGenerator GetNextRoom(SubMapGenerator previousRoom)//used in map generation, for main route calculations
    {
        if (NorthConnection != null && NorthConnection != previousRoom)
            return NorthConnection;
        if (EastConnection != null && EastConnection != previousRoom)
            return EastConnection;
        if (SouthConnection != null && SouthConnection != previousRoom)
            return SouthConnection;
        return WestConnection!;
    }
        
    public void AccessMainRoute(double mainRouteDifficulty)//used in map generation
    {
        if (MainRoute)
            return;
        if(Difficulty < mainRouteDifficulty)
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
        //roomData = GameManager.RoomData.Rooms[0].room.Pattern;
        Tiles = new Tile[25, 25];
        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                TileTypeData tileTypeData = roomData[x, y].Name == "wall"
                    ? GetWallData(roomData, x, y)
                    : roomData[x, y];
                Tiles[x, y] = new Tile(tileTypeData);
            }
        }
        FillBorderWalls(roomData, northernTileMap, easternTileMap);
        return new SubMap(Tiles, Difficulty, MapX, MapY);
    }

    void FillBorderWalls(TileTypeData[,] roomData, Tile[,] northernTileMap, Tile[,] easternTileMap)
    {
        var tileTypes = GameManager.TileTypesExtractor.TileTypes;
        for (int x = 0; x < 25; x++)
        {
            TileTypeData terrainObject = NorthConnection == null || x is not (11 or 12)
                ? GetWallData(roomData, x, 24, northernTileMap?[x,0].ConnectToWalls)
                : tileTypes["floor"];
            Tiles[x, 24] = new Tile(terrainObject);
        }
        for (int y = 0; y < 24; y++)
        {
            TileTypeData terrainObject = EastConnection == null || y is not (11 or 12)
                ? GetWallData(roomData, 24, y, easternTileConnectable: easternTileMap?[0,y].ConnectToWalls)
                : tileTypes["floor"];
            Tiles[24, y] = new Tile(terrainObject);
        }
    }

    TileTypeData GetWallData(TileTypeData[,] roomData, int x, int y, 
        bool? northernTileConnectable = null, bool? easternTileConnectable = null)
    {
        if ((x, y) is (24, 24))
            return GameManager.TileTypesExtractor.TileTypes["wall_nesw"];
        StringBuilder wallKey = new StringBuilder("wall_");
        if (northernTileConnectable ?? CheckForWallInTile(roomData, x, y + 1))
            wallKey.Append('n');
        if (easternTileConnectable ?? CheckForWallInTile(roomData, x + 1, y))
            wallKey.Append('e');
        if (CheckForWallInTile(roomData, x, y - 1))
            wallKey.Append('s');
        if (CheckForWallInTile(roomData, x - 1, y))
            wallKey.Append('w');
        string str = wallKey.ToString();
        if (str[^1] == '_')
            str = "wall";
        return GameManager.TileTypesExtractor.TileTypes[str];
    }

    bool CheckForWallInTile(TileTypeData[,] roomData, int x, int y)
    {
        if (x is < -1 or > 24 || y is < -1 or > 24)
            return false;
        return x is -1 or 24 || y is -1 or 24 || roomData[x, y].ConnectToWalls;
    }
}