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
        _map = new SubMap[_mapWidth + 1 + mapOffset*2, _maxPoint - _minPoint +1 + mapOffset*2];
        int lastMin = 0;
        int lastMax = 0;
        int startRoomPos = _rng.Next(0,_bars.Count);
        bool topStartRoom = Chance(50, _rng);
        int mainRouteRoomCount = 0;
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            int x = i - mapOffset;
            if (x < 0 || x >= _bars.Count)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    _map[i, y] = new SubMap(i,y);
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
                _map[i, j] = new SubMap(i,j);
                if (y >= min1 && y <= min2 || y >= max1 && y <= max2)//main route
                {

                    if (topStartRoom && x == startRoomPos && y == _bars[x].max || 
                        !topStartRoom && x == _bars.Count - startRoomPos - 1 && y == _bars[x].min)
                    {
                        _map[i, j].StartPoint = true;
                        StartPoint = _map[i, j];
                    }

                    mainRouteRoomCount++;
                    _map[i, j].MainRoute = true;
                    _map[i, j].HasAccessToMainRoute = true;
                    if (x > 0 && (_bars[x-1].min == y || _bars[x-1].max == y))
                    {
                        ConnectRooms(_map[i, j], _map[i-1,j]);
                    }

                    if (j > 0 && _map[i, j - 1].MainRoute)
                    {
                        ConnectRooms(_map[i,j], _map[i,j-1]);
                    }
                }
            }
            lastMax = _bars[x].max;
            lastMin = _bars[x].min;
        }

        int endRoomIndex = mainRouteRoomCount / 2;
        double addDifficulty = 1;
        double lastDifficulty = 0;
        SubMap previousSubMap = null;
        SubMap currentSubMap = StartPoint!;
        for (int i = 0; i < mainRouteRoomCount; i++)
        {
            lastDifficulty += addDifficulty;
            currentSubMap.Difficulty = lastDifficulty;
            if (lastDifficulty <= 0)
                throw new Exception();
            if (i == endRoomIndex)
            {
                currentSubMap.EndPoint = true;
                addDifficulty = -1;
            }
            SubMap newSubMap = currentSubMap.GetNextRoom(previousSubMap);
            previousSubMap = currentSubMap;
            currentSubMap = newSubMap;
        }
        
    }

    private void FillMap()
    {
        int yStep = Chance(50, _rng) ? 1 : -1;
        int lastX = _map.GetLength(0) / 2 + 1;
        for (int x = 0; x < lastX; x++)
        {
            FillColumn(x, 1, yStep);
            yStep = -yStep;
        }
        
        for (int x = _map.GetLength(0)-1; x >= lastX; x--)
        {
            FillColumn(x, -1, yStep);
            yStep = -yStep;
        }
        
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if (!_map[x, y].HasAccessToMainRoute)
                {
                    List<SubMap> adjRooms = new();
                    List<SubMap> adjNoAccessRooms = new();
                    AddRoomToList(x + 1, y, adjRooms, adjNoAccessRooms);
                    AddRoomToList(x - 1, y, adjRooms, adjNoAccessRooms);
                    AddRoomToList(x, y + 1, adjRooms, adjNoAccessRooms);
                    AddRoomToList(x, y - 1, adjRooms, adjNoAccessRooms);
                    if (adjRooms.Count != 0)
                    {
                        ConnectRooms(_map[x,y], adjRooms[_rng.Next(0, adjRooms.Count)]);
                        continue;
                    }

                    foreach (var room in adjNoAccessRooms)
                    {
                        ConnectRooms(_map[x, y], room);
                    }
                }
            }
        }
    }

    private void FillSubMaps()
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                FinalizeRoom(_map[i,j]);
            }
        }
    }

    /// <summary>
    /// Work in progress method
    /// </summary>
    private void FinalizeRoom(SubMap subMap)
    {
        var rooms = GameManager.RoomData.Rooms;
        subMap.Fill(rooms[_rng.Next(0, rooms.Count)].room.Pattern);
    }

    private void AddRoomToList(int x, int y, List<SubMap> list, List<SubMap> noAccessList)
    {
        if (x < 0 || x >= _map.GetLength(0) || y < 0 || y >= _map.GetLength(1)) return;
        
        if (_map[x, y].HasAccessToMainRoute)
        {
            list.Add(_map[x,y]);
        }
        else
        {
            noAccessList.Add(_map[x, y]);
        }
    }

    private void FillColumn(int x, int xStep, int yStep)
    {
        int connectionChance = 50;
        
        int startY = yStep == 1 ? 0 : _map.GetLength(1) - 1;
        for (int i = 0, y = startY; i < _map.GetLength(1); y += yStep, i++)
        {
            SubMap subMap = _map[x, y];
            if (subMap.HasAccessToMainRoute)
            {
                continue;
            }
            
            SubMap yConnection = null;
            SubMap xConnection = _map[x + xStep, y];
            int difY = y + yStep;
            if (difY >= 0 && difY < _map.GetLength(1))
            {
                yConnection = _map[x, difY];
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

    private void ConnectRooms(SubMap room1, SubMap room2)
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