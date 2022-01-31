using System;
using GalacticMeltdown.data;

namespace GalacticMeltdown;
public class SubMap
{
    public bool HasAccessToMainRoute;
    public bool MainRoute;
    public SubMap NorthConnection = null;
    public SubMap EastConnection = null;
    public SubMap SouthConnection = null;
    public SubMap WestConnection = null;
    public bool StartPoint;
    public bool EndPoint;
    public double Difficulty = -1;

    public int ConnectionCount =>
        Convert.ToInt32(NorthConnection == null) + Convert.ToInt32(EastConnection == null) +
        Convert.ToInt32(SouthConnection == null) + Convert.ToInt32(WestConnection == null);

    public Tile[,] Tiles { get; private set; }
    public int MapX { get; }
    public int MapY { get; }

    public SubMap(int x, int y)
    {
        MapX = x;
        MapY = y;
    }
        
    public SubMap GetNextRoom(SubMap previousRoom)//used in map generation, for main route calculations
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
    
    public void Fill(TileTypeData[,] roomData)
    {
        //roomData = GameManager.RoomData.Rooms[0].room.Pattern;
        Tiles = new Tile[25, 25];
        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                int newX = MapX * 25 + x;
                int newY = MapY * 25 + y;
                Tiles[x, y] = new Tile(roomData[x, y], newX, newY);
            }
        }
        FillBorderWalls();
    }

    void FillBorderWalls()
    {
        for (int x = 0; x < 25; x++)
        {
            int newX = MapX * 25 + x;
            int newY = MapY * 25 + 24;
            TileTypeData terrainObject = NorthConnection == null || x is not (11 or 12)
                ? GameManager.TileTypesExtractor.TileTypes["wall"]
                : GameManager.TileTypesExtractor.TileTypes["floor"];
            Tiles[x, 24] = new Tile(terrainObject, newX, newY);
        }
        for (int y = 0; y < 24; y++)
        {
            int newX = MapX * 25 + 24;
            int newY = MapY * 25 + y;
            TileTypeData terrainObject = EastConnection == null || y is not (11 or 12)
                ? GameManager.TileTypesExtractor.TileTypes["wall"]
                : GameManager.TileTypesExtractor.TileTypes["floor"];
            Tiles[24, y] = new Tile(terrainObject, newX, newY);
        }
    }
}