using System;
using System.Collections.Generic;
using static GalacticMeltdown.Utility;

namespace GalacticMeltdown;

public class MapGenerator
{
    private Random _rng;
    private readonly int mapOffset = 3;
    private readonly int minSize = 6;
    private readonly int _mapWidth = 20;
    
    private SubMapGenerator[,] _tempMap;
    private SubMap[,] _map;
    private List<(int min, int max)> _bars;
    private int _maxPoint;
    private int _minPoint;
    private SubMap StartPoint;
    
    public int Seed { get; private set; }

    public MapGenerator(int seed)
    {
        ChangeSeed(seed);
    }

    public void ChangeSeed(int newSeed)
    {
        _rng = new Random(newSeed);
        Seed = newSeed;
    }
    
    public Map Generate()
    {
        GenerateBars();
        BuildMainRoute();
        FillMap();
        FillSubMaps();
        return new Map(_map, Seed, StartPoint);
    }

    private void GenerateBars()
    {
        int newSize = _rng.Next(minSize, 9);
        _bars = new() {(0, newSize)};
        _maxPoint = newSize;
        int lastMinVector = -1;
        int lastMaxVector = 1;
        for (int i = _mapWidth; i > 0; i--)
        {
            int sizeRestriction = i + 7;
            int dif = Chance(40, _rng) ? 0 : 
                Chance(newSize * 20 - 120 - Math.Abs(_mapWidth / 2 - sizeRestriction) * 5, _rng) ? -1 : 1;
            int posMultiplier = Chance(50, _rng) ? Chance(40, _rng) ? 2 : 1 : 0;
            if (sizeRestriction <= 8)
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
        _tempMap = new SubMapGenerator[_mapWidth + 1 + mapOffset*2, _maxPoint - _minPoint +1 + mapOffset*2];
        int lastMin = 0;
        int lastMax = 0;
        int startRoomPos = _rng.Next(0,_bars.Count);
        bool topStartRoom = Chance(50, _rng);
        int mainRouteRoomCount = 0;

        SubMapGenerator startPoint = null;
        for (int i = 0; i < _tempMap.GetLength(0); i++)
        {
            int x = i - mapOffset;
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
            for (int y = _minPoint-mapOffset, j = 0; y <= _maxPoint + mapOffset; j++, y++)
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
        int yStep = Chance(50, _rng) ? 1 : -1;
        int lastX = _tempMap.GetLength(0) / 2 + 1;
        for (int x = 0; x < lastX; x++)
        {
            FillColumn(x, 1, yStep);
            yStep = -yStep;
        }
        
        for (int x = _tempMap.GetLength(0)-1; x >= lastX; x--)
        {
            FillColumn(x, -1, yStep);
            yStep = -yStep;
        }
        
        for (int x = 0; x < _tempMap.GetLength(0); x++)
        {
            for (int y = 0; y < _tempMap.GetLength(1); y++)
            {
                if (_tempMap[x, y].HasAccessToMainRoute) continue;
                
                List<SubMapGenerator> adjAccessRooms = new();
                List<SubMapGenerator> adjNoAccessRooms = new();
                AddRoomToList(x + 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x - 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y + 1, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y - 1, adjAccessRooms, adjNoAccessRooms);
                if (adjAccessRooms.Count != 0)
                {
                    ConnectRooms(_tempMap[x,y], adjAccessRooms[_rng.Next(0, adjAccessRooms.Count)]);
                    continue;
                }

                foreach (var room in adjNoAccessRooms)
                {
                    ConnectRooms(_tempMap[x, y], room);
                }
            }
        }
    }

    private void FillSubMaps()
    {
        int width = _tempMap.GetLength(0);
        int height = _tempMap.GetLength(1);
        _map = new SubMap[width, height];
        for (int i = width - 1; i >= 0; i--)
        {
            for (int j = height - 1; j >= 0; j--)
            {
                _map[i,j] = FinalizeRoom(i, j);
                if (_tempMap[i, j].StartPoint)
                    StartPoint = _map[i, j];
            }
        }
    }

    /// <summary>
    /// Work in progress method
    /// </summary>
    private SubMap FinalizeRoom(int x, int y)
    {
        Tile[,] northernTileMap = y == _tempMap.GetLength(1) - 1 ? null : _tempMap[x, y + 1].Tiles;
        Tile[,] easternTileMap = x == _tempMap.GetLength(0) - 1 ? null : _tempMap[x + 1, y].Tiles;
        var rooms = GameManager.RoomData.Rooms;
        return _tempMap[x,y].GenerateSubMap
            (rooms[_rng.Next(0, rooms.Count)].room.Pattern, northernTileMap, easternTileMap);
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
        const int connectionChance = 50;
        
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

            if (Chance(connectionChance, _rng) && (!xConnection.HasAccessToMainRoute || !subMap.HasAccessToMainRoute))
            {
                ConnectRooms(subMap, xConnection);
            }
            if (yConnection != null 
                && Chance(connectionChance, _rng) && (!yConnection.HasAccessToMainRoute || !subMap.HasAccessToMainRoute))
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