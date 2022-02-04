using System;
using System.Collections.Generic;
using GalacticMeltdown.data;
using static GalacticMeltdown.Utility;

namespace GalacticMeltdown;

public class MapGenerator
{
    private Random _rng;
    private SubMapGenerator[,] _tempMap;
    private SubMap[,] _map;
    private List<(int min, int max)> _bars;
    private int _maxPoint;
    private int _minPoint;
    private SubMap _startPoint;
    private Tile[] _westernWall;
    private Tile[] _southernWall;
    private int _seed;
    private Dictionary<string, TileTypeData> _tileTypes;
    private List<(int rarity, int exitCount, RoomData.Room room)> _rooms;

    private const int MapOffset = 1; //amount of "layers" of rooms outside of main route
    private const int MapWidth = 20; //width is specified; height is random
    private const int ConnectionChance = 50; //room connection chance

    public MapGenerator(int seed, Dictionary<string, TileTypeData> tileTypes, 
        List<(int rarity, int exitCount, RoomData.Room room)> rooms)
    {
        _tileTypes = tileTypes;
        _rooms = rooms;
        ChangeSeed(seed);
    }

    public void ChangeSeed(int newSeed)
    {
        _rng = new Random(newSeed);
        _seed = newSeed;
    }
    
    public Map Generate()
    {
        GenerateBars();
        BuildMainRoute();
        FillMap();
        GenerateBorderWalls();
        return new Map(_map, _seed, _startPoint, _southernWall, _westernWall, _tileTypes);
    }

    private void GenerateBars()
    {
        //These 4 integers don't make much sense and probably should be left untouched
        int minSize = 6;
        int restrictionAddition = 7;
        int firstMaxValue = 9;
        int posMultiplierCeiling = 8;
        
        int newSize = _rng.Next(minSize, firstMaxValue);
        _bars = new() {(0, newSize)};
        _maxPoint = newSize;
        int lastMinVector = -1;
        int lastMaxVector = 1;
        for (int i = MapWidth; i > 0; i--)
        {
            int sizeRestriction = i + restrictionAddition;
            
            //Main route distribution formulas
            int dif = Chance(40, _rng) ? 0 : 
                Chance(newSize * 20 - 120 - Math.Abs(MapWidth / 2 - sizeRestriction) * 5, _rng) ? -1 : 1;
            int posMultiplier = Chance(50, _rng) ? Chance(40, _rng) ? 2 : 1 : 0;

            if (sizeRestriction <= posMultiplierCeiling)
            {
                posMultiplier = 0;
            }
            else
            {
                newSize = Math.Min(sizeRestriction, newSize + dif);
                newSize = Math.Max(newSize, minSize);
            }
            int newMin;
            if (lastMinVector >= 0)
            {
                if (lastMaxVector <= 0)
                {
                    newMin = _bars[^1].min + _rng.Next(-1,2) * posMultiplier;
                }
                else
                {
                    int newMax = _bars[^1].max + _rng.Next(0, 2) * posMultiplier;
                    newMin = newMax - newSize;
                }
            }
            else if(lastMaxVector <= 0)
            {
                newMin = _bars[^1].min + _rng.Next(-1, 0) * posMultiplier;
            }
            else
            {
                newMin = _bars[^1].min;
            }
            lastMinVector = newMin - _bars[^1].min;
            lastMaxVector = newMin + newSize - _bars[^1].max;
            _bars.Add((newMin, newMin + newSize));
            _maxPoint = Math.Max(_maxPoint, newSize + newMin);
            _minPoint = Math.Min(_minPoint, newMin);
        }
    }
    
    private void BuildMainRoute()
    {
        _tempMap = new SubMapGenerator[MapWidth + 1 + MapOffset*2, _maxPoint - _minPoint +1 + MapOffset*2];
        int lastMin = 0;
        int lastMax = 0;
        int startRoomPos = _rng.Next(0,_bars.Count);
        bool topStartRoom = Chance(50, _rng);
        int mainRouteRoomCount = 0;

        SubMapGenerator startPoint = null;
        for (int i = 0; i < _tempMap.GetLength(0); i++)
        {
            int x = i - MapOffset;
            if (x < 0 || x >= _bars.Count)
            {
                for (int y = 0; y < _tempMap.GetLength(1); y++)
                {
                    _tempMap[i, y] = new SubMapGenerator(i,y);
                }
                continue;
            }
            if (x == _bars.Count - 1)
            {
                lastMin = _bars[x].max - 1;
                lastMax = _bars[x].max;
            }
            int min1 = Math.Min(_bars[x].min, lastMin);
            int min2 = _bars[x].min + lastMin - min1;
            int max1 = Math.Min(_bars[x].max, lastMax);
            int max2 = _bars[x].max + lastMax - max1;
            for (int y = _minPoint-MapOffset, j = 0; y <= _maxPoint + MapOffset; j++, y++)
            {
                _tempMap[i, j] = new SubMapGenerator(i,j);
                if ((y < min1 || y > min2) && (y < max1 || y > max2)) continue;
                
                if (topStartRoom && x == startRoomPos && y == _bars[x].max || 
                    !topStartRoom && x == _bars.Count - startRoomPos - 1 && y == _bars[x].min)
                {
                    _tempMap[i, j].StartPoint = true;
                    startPoint = _tempMap[i, j];
                }

                mainRouteRoomCount++;
                _tempMap[i, j].MainRoute = true;
                _tempMap[i, j].HasAccessToMainRoute = true;
                if (x > 0 && (_bars[x-1].min == y || _bars[x-1].max == y))
                {
                    ConnectRooms(_tempMap[i, j], _tempMap[i-1,j]);
                }

                if (j > 0 && _tempMap[i, j - 1].MainRoute)
                {
                    ConnectRooms(_tempMap[i,j], _tempMap[i,j-1]);
                }
            }
            lastMax = _bars[x].max;
            lastMin = _bars[x].min;
        }

        int endRoomIndex = mainRouteRoomCount / 2;
        double addDifficulty = 1;
        double lastDifficulty = 0;
        SubMapGenerator previousSubMap = null;
        SubMapGenerator currentSubMap = startPoint!;
        for (int i = 0; i < mainRouteRoomCount; i++)
        {
            lastDifficulty += addDifficulty;
            currentSubMap.Difficulty = lastDifficulty;
            if (i == endRoomIndex)
            {
                currentSubMap.EndPoint = true;
                addDifficulty = -1;
            }
            SubMapGenerator newSubMap = currentSubMap.GetNextRoom(previousSubMap);
            previousSubMap = currentSubMap;
            currentSubMap = newSubMap;
        }
        
    }

    private void FillMap()
    {
        int width = _tempMap.GetLength(0);
        int height = _tempMap.GetLength(1);
        int yStep = Chance(50, _rng) ? 1 : -1;
        int lastX = width / 2 + 1;
        for (int x = 0; x < lastX; x++)
        {
            FillColumn(x, 1, yStep);
            yStep = -yStep;
        }
        
        for (int x = width-1; x >= lastX; x--)
        {
            FillColumn(x, -1, yStep);
            yStep = -yStep;
        }
        
        _map = new SubMap[width, height];
        for (int x = width - 1; x >= 0; x--)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (_tempMap[x, y].HasAccessToMainRoute)
                {
                    FinalizeRoom(x, y);
                    continue;
                }
                
                List<SubMapGenerator> adjAccessRooms = new();
                List<SubMapGenerator> adjNoAccessRooms = new();
                AddRoomToList(x + 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x - 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y + 1, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y - 1, adjAccessRooms, adjNoAccessRooms);
                if (adjAccessRooms.Count != 0)
                {
                    ConnectRooms(_tempMap[x,y], adjAccessRooms[_rng.Next(0, adjAccessRooms.Count)]);
                    FinalizeRoom(x, y);
                    continue;
                }

                foreach (var room in adjNoAccessRooms)
                {
                    ConnectRooms(_tempMap[x, y], room);
                }
                FinalizeRoom(x, y);
            }
        }
    }

    /// <summary>
    /// Work in progress method
    /// </summary>
    private void FinalizeRoom(int x, int y)
    {
        //matrix rotation: 90deg = transpose + rev rows; 270deg = transpose + rev cols; 180deg = rev rows + cols
        var rooms = GameManager.Rooms;
        var room = rooms[_rng.Next(0, rooms.Count)];
        TileTypeData[,] roomData = (TileTypeData[,]) room.room.Pattern.Clone();
        List<int> possibleRotations = new(){0, 90, 180, 270};
        switch (possibleRotations[_rng.Next(0, possibleRotations.Count)])
        {
            case 90:
                Algorithms.TransposeMatrix(roomData);
                Algorithms.ReverseMatrixRows(roomData);
                FlipPoles(roomData, ("north", "west"), ("south", "east"), ("west", "east"));
                break;
            case 180:
                Algorithms.ReverseMatrixRows(roomData);
                Algorithms.ReverseMatrixCols(roomData);
                FlipPoles(roomData, ("north", "south"), ("west", "east"));
                break;
            case 270:
                Algorithms.TransposeMatrix(roomData);
                Algorithms.ReverseMatrixCols(roomData);
                FlipPoles(roomData, ("north", "west"), ("south", "east"), ("north", "south"));
                break;
        }

        Tile[,] northernTileMap = y == _tempMap.GetLength(1) - 1 ? null : _tempMap[x, y + 1].Tiles;
        Tile[,] easternTileMap = x == _tempMap.GetLength(0) - 1 ? null : _tempMap[x + 1, y].Tiles;
        _map[x, y] = _tempMap[x, y].GenerateSubMap(roomData, northernTileMap, easternTileMap);
        if (_tempMap[x, y].StartPoint)
            _startPoint = _map[x, y];
    }

    private void FlipPoles(TileTypeData[,] roomData, params (string p1, string p2)[] poles)
    {
        for (int x = 0; x < roomData.GetLength(0); x++)
        {
            for (int y = 0; y < roomData.GetLength(1); y++)
            {
                string[] parsedId = roomData[x, y].Id.Split('_');
                if (parsedId.Length != 3 || parsedId[1] != "if")
                    continue;
                foreach (var (p1, p2) in poles)
                {
                    if (parsedId[2] == p1)
                        parsedId[2] = p2;
                    else if (parsedId[2] == p2)
                        parsedId[2] = p1;
                }

                roomData[x, y] = GameManager.TileTypes[$"{parsedId[0]}_if_{parsedId[2]}"];
            }
        }
    }
    
    private void GenerateBorderWalls()
    {
        _westernWall = new Tile[_map.GetLength(1) * 25];
        _southernWall = new Tile[_map.GetLength(0) * 25];
        for (int mapX = 0; mapX < _map.GetLength(0); mapX++)
        {
            for (int x = 0; x < 25; x++)
            {
                int tileX = mapX * 25 + x;
                string wallKey = x == 24 
                    ? "wall_nesw" : _map[mapX, 0].Tiles[x, 0].ConnectToWalls ? "wall_new" : "wall_ew";
                _southernWall[tileX] = new Tile(_tileTypes[wallKey]);
            }
        }
        for (int mapY = 0; mapY < _map.GetLength(1); mapY++)
        {
            for (int y = 0; y < 25; y++)
            {
                int tileY = mapY * 25 + y;
                string wallKey = y == 24 
                    ? "wall_nesw" : _map[0, mapY].Tiles[0, y].ConnectToWalls ? "wall_nes" : "wall_ns";
                _westernWall[tileY] = new Tile(_tileTypes[wallKey]);
            }
        }
    }
    
    private void AddRoomToList(int x, int y, List<SubMapGenerator> accessList, List<SubMapGenerator> noAccessList)
    {
        if (x < 0 || x >= _tempMap.GetLength(0) || y < 0 || y >= _tempMap.GetLength(1)) return;
        
        if (_tempMap[x, y].HasAccessToMainRoute)
        {
            accessList.Add(_tempMap[x,y]);
        }
        else
        {
            noAccessList.Add(_tempMap[x, y]);
        }
    }

    private void FillColumn(int x, int xStep, int yStep)
    {
        int startY = yStep == 1 ? 0 : _tempMap.GetLength(1) - 1;
        for (int i = 0, y = startY; i < _tempMap.GetLength(1); y += yStep, i++)
        {
            SubMapGenerator subMap = _tempMap[x, y];
            if (subMap.HasAccessToMainRoute)
            {
                continue;
            }
            
            SubMapGenerator yConnection = null;
            SubMapGenerator xConnection = _tempMap[x + xStep, y];
            int difY = y + yStep;
            if (difY >= 0 && difY < _tempMap.GetLength(1))
            {
                yConnection = _tempMap[x, difY];
            }

            if (subMap.StartPoint || subMap.EndPoint)
            {
                ConnectRooms(subMap, xConnection);
                if(yConnection != null)
                    ConnectRooms(subMap, yConnection);
                return;
            }

            if (Chance(ConnectionChance, _rng) && (!xConnection.HasAccessToMainRoute || !subMap.HasAccessToMainRoute))
            {
                ConnectRooms(subMap, xConnection);
            }
            if (yConnection != null 
                && Chance(ConnectionChance, _rng) && (!yConnection.HasAccessToMainRoute || !subMap.HasAccessToMainRoute))
            {
                ConnectRooms(subMap, yConnection);
            }
        }
    }

    private void ConnectRooms(SubMapGenerator room1, SubMapGenerator room2)
    {
        if (room1.MapX != room2.MapX)
        {
            int dif = room1.MapX - room2.MapX;
            switch (dif)
            {
                case 1:
                    room1.WestConnection = room2;
                    room2.EastConnection = room1;
                    break;
                case -1:
                    room1.EastConnection = room2;
                    room2.WestConnection = room1;
                    break;
                default:
                    return;
            }
        }
        else
        {
            int dif = room1.MapY - room2.MapY;
            switch (dif)
            {
                case 1:
                    room1.SouthConnection = room2;
                    room2.NorthConnection = room1;
                    break;
                case -1:
                    room1.NorthConnection = room2;
                    room2.SouthConnection = room1;
                    break;
                default:
                    return;
            }
        }

        if (room1.HasAccessToMainRoute)
            room2.AccessMainRoute(room1.Difficulty);
        if(room2.HasAccessToMainRoute)
            room1.AccessMainRoute(room2.Difficulty);
    }
}