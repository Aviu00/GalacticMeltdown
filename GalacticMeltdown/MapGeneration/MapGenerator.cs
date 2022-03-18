using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Data;
using static GalacticMeltdown.Utility.UtilityFunctions;

namespace GalacticMeltdown.MapGeneration;

public class MapGenerator
{
    private const int MapOffset = 1; //amount of "layers" of rooms outside of main route
    private const int MapWidth = 20; //width is specified; height is random
    private const int ConnectionChance = 60; //room connection chance(this probably shouldn't be touched)
    private const int ChunkSize = DataHolder.ChunkSize;
    private List<RoomTypes> _roomTypes;

    private readonly Random _rng;
    
    private ChunkGenerator[,] _tempMap;
    private Chunk[,] _map;
    
    private List<(int min, int max)> _bars;
    
    private int _maxPoint;
    private int _minPoint;

    private Tile[] _westernWall;
    private Tile[] _southernWall;
    
    private (int x, int y) _playerStartPoint;
    private (int x, int y) _endPoint;

    public MapGenerator(int seed)
    {
        _rng = new Random(seed);
        _roomTypes = DataHolder.GetRooms();
    }

    public Level Generate()
    {
        GenerateBars();
        BuildMainRoute();
        FillMap();
        FinalizeRooms();
        GenerateBorderWalls();
        return new Level(_map, _playerStartPoint, _southernWall, _westernWall, _endPoint);
    }

    private void GenerateBars()
    {
        //These 4 integers don't make much sense and probably should be left untouched
        const int minSize = 6;
        const int restrictionAddition = 7;
        const int firstMaxValue = 9;
        const int posMultiplierCeiling = 8;

        int newSize = _rng.Next(minSize, firstMaxValue);
        _bars = new List<(int min, int max)> {(0, newSize)};
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
                    newMin = _bars[^1].min + _rng.Next(-1, 2) * posMultiplier;
                }
                else
                {
                    int newMax = _bars[^1].max + _rng.Next(0, 2) * posMultiplier;
                    newMin = newMax - newSize;
                }
            }
            else if (lastMaxVector <= 0)
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
        _tempMap = new ChunkGenerator[MapWidth + 1 + MapOffset * 2, _maxPoint - _minPoint + 1 + MapOffset * 2];
        int lastMin = 0;
        int lastMax = 0;
        int startRoomPos = _rng.Next(0, _bars.Count);
        bool topStartRoom = Chance(50, _rng);
        int mainRouteRoomCount = 0;

        ChunkGenerator startPoint = null;
        for (int i = 0; i < _tempMap.GetLength(0); i++)
        {
            int x = i - MapOffset;
            if (x < 0 || x >= _bars.Count)
            {
                for (int y = 0; y < _tempMap.GetLength(1); y++)
                {
                    _tempMap[i, y] = new ChunkGenerator(i, y);
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
            for (int y = _minPoint - MapOffset, j = 0; y <= _maxPoint + MapOffset; j++, y++)
            {
                _tempMap[i, j] = new ChunkGenerator(i, j);
                if ((y < min1 || y > min2) && (y < max1 || y > max2)) continue;

                if (topStartRoom && x == startRoomPos && y == _bars[x].max
                    || !topStartRoom && x == _bars.Count - startRoomPos - 1 && y == _bars[x].min)
                {
                    _tempMap[i, j].IsStartPoint = true;
                    startPoint = _tempMap[i, j];
                }

                mainRouteRoomCount++;
                _tempMap[i, j].MainRoute = true;
                _tempMap[i, j].HasAccessToMainRoute = true;
                if (x > 0 && (_bars[x - 1].min == y || _bars[x - 1].max == y))
                {
                    ConnectRooms(_tempMap[i, j], _tempMap[i - 1, j]);
                }

                if (j > 0 && _tempMap[i, j - 1].MainRoute)
                {
                    ConnectRooms(_tempMap[i, j], _tempMap[i, j - 1]);
                }
            }

            lastMax = _bars[x].max;
            lastMin = _bars[x].min;
        }

        int endRoomIndex = mainRouteRoomCount / 2;
        int addDifficulty = 1;
        int difficulty = 0;
        ChunkGenerator previousChunk = null;
        ChunkGenerator currentChunk = startPoint!;
        for (int i = 0; i < mainRouteRoomCount; i++)
        {
            difficulty += addDifficulty;
            currentChunk.Difficulty = difficulty;
            if (i == endRoomIndex)
            {
                currentChunk.IsEndPoint = true;
                _endPoint = (currentChunk.MapX * ChunkSize + ChunkSize / 2,
                    currentChunk.MapY * ChunkSize + ChunkSize / 2);
                addDifficulty = -1;
            }

            (previousChunk, currentChunk) = (currentChunk, currentChunk.GetNextRoom(previousChunk));
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

        for (int x = width - 1; x >= lastX; x--)
        {
            FillColumn(x, -1, yStep);
            yStep = -yStep;
        }

        _map = new Chunk[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_tempMap[x, y].HasAccessToMainRoute) continue;

                List<ChunkGenerator> adjAccessRooms = new();
                List<ChunkGenerator> adjNoAccessRooms = new();
                AddRoomToList(x + 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x - 1, y, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y + 1, adjAccessRooms, adjNoAccessRooms);
                AddRoomToList(x, y - 1, adjAccessRooms, adjNoAccessRooms);
                if (adjAccessRooms.Count != 0)
                {
                    ConnectRooms(_tempMap[x, y], adjAccessRooms[_rng.Next(0, adjAccessRooms.Count)]);
                    continue;
                }

                foreach (var room in adjNoAccessRooms)
                {
                    ConnectRooms(_tempMap[x, y], room);
                }
            }
        }
    }

    private void FinalizeRooms()
    {
        for (int x = _map.GetLength(0) - 1; x >= 0; x--)
        {
            for (int y = _map.GetLength(1) - 1; y >= 0; y--)
            {
                FinalizeRoom(x, y);
            }
        }
    }

    private void FinalizeRoom(int x, int y)
    {
        int roomType = CalculateRoomType(x, y);
        var matchingRooms =
            _roomTypes.Where(room => ValidateRoomType(room.Type, roomType) && Chance(room.Chance, _rng)).ToArray();
        RoomTypes room = matchingRooms[_rng.Next(0, matchingRooms.Length)];
        var roomData = (TileInformation[,]) room.RoomInterior.Clone();
        RotateRoomPattern(x, y, roomData, room);
        Tile[,] northernTileMap = y == _tempMap.GetLength(1) - 1 ? null : _tempMap[x, y + 1].Tiles;
        Tile[,] easternTileMap = x == _tempMap.GetLength(0) - 1 ? null : _tempMap[x + 1, y].Tiles;
        _map[x, y] = _tempMap[x, y].GenerateChunk(roomData, _rng.Next(0, 1000000), northernTileMap, easternTileMap);
        if (_tempMap[x, y].IsStartPoint)
            _playerStartPoint = (x * ChunkSize + ChunkSize / 2, y * ChunkSize + ChunkSize / 2);
    }

    private int CalculateRoomType(int x, int y)
    {
        int connections = _tempMap[x, y].ConnectionCount;
        switch (connections)
        {
            case 1:
                return 0;
            case 2:
                if ((_tempMap[x, y].NorthConnection is null || _tempMap[x, y].SouthConnection is null)
                    && (_tempMap[x, y].WestConnection is null || _tempMap[x, y].EastConnection is null))
                {
                    return 2;
                }
                else
                    return 1;
            default:
                return connections;
        }
    }

    private bool ValidateRoomType(int roomType, int validator)
    {
        return roomType >= validator && (roomType, validator) is not (1, 2) and not (2, 1);
    }

    private void RotateRoomPattern(int x, int y, TileInformation[,] roomData, RoomTypes room)
    {
        if (room.RotationalSymmetry) return;
        List<int> possibleRotations = new() {0, 90, 180, 270};
        var mapRoom = _tempMap[x, y];
        switch (room.Type)
        {
            case 0:
                if (mapRoom.SouthConnection is null) possibleRotations.Remove(0);
                if (mapRoom.EastConnection is null) possibleRotations.Remove(90);
                if (mapRoom.NorthConnection is null) possibleRotations.Remove(180);
                if (mapRoom.WestConnection is null) possibleRotations.Remove(270);
                break;
            case 1:
                if (mapRoom.NorthConnection is null && mapRoom.SouthConnection is null)
                {
                    possibleRotations.Remove(0);
                    possibleRotations.Remove(180);
                }
                else if (mapRoom.WestConnection is null && mapRoom.EastConnection is null)
                {
                    possibleRotations.Remove(90);
                    possibleRotations.Remove(270);
                }

                break;
            case 2:
                if (mapRoom.SouthConnection is not null)
                {
                    possibleRotations.Remove(180);
                    possibleRotations.Remove(90);
                }

                if (mapRoom.WestConnection is not null)
                {
                    possibleRotations.Remove(0);
                    possibleRotations.Remove(90);
                }

                if (mapRoom.NorthConnection is not null)
                {
                    possibleRotations.Remove(0);
                    possibleRotations.Remove(270);
                }

                if (mapRoom.EastConnection is not null)
                {
                    possibleRotations.Remove(270);
                    possibleRotations.Remove(180);
                }

                break;
            case 3:
                if (mapRoom.SouthConnection is not null) possibleRotations.Remove(180);
                if (mapRoom.EastConnection is not null) possibleRotations.Remove(270);
                if (mapRoom.NorthConnection is not null) possibleRotations.Remove(0);
                if (mapRoom.WestConnection is not null) possibleRotations.Remove(90);
                break;
        }

        if (room.CentralSymmetry)
        {
            if (possibleRotations.Contains(0)) possibleRotations.Remove(180);
            if (possibleRotations.Contains(90)) possibleRotations.Remove(270);
        }

        //matrix rotation: 90deg = transpose + rev rows; 270deg = transpose + rev cols; 180deg = rev rows + cols
        switch (possibleRotations[_rng.Next(0, possibleRotations.Count)])
        {
            case 90:
                Algorithms.TransposeMatrix(roomData);
                Algorithms.ReverseMatrixRows(roomData);
                FlipPoles(roomData, ("north", "west"), ("south", "east"), ("north", "south"));
                break;
            case 180:
                Algorithms.ReverseMatrixRows(roomData);
                Algorithms.ReverseMatrixCols(roomData);
                FlipPoles(roomData, ("north", "south"), ("west", "east"));
                break;
            case 270:
                Algorithms.TransposeMatrix(roomData);
                Algorithms.ReverseMatrixCols(roomData);
                FlipPoles(roomData, ("north", "west"), ("south", "east"), ("west", "east"));
                break;
        }
    }

    private void FlipPoles(TileInformation[,] roomData, params (string p1, string p2)[] poles)
    {
        for (int x = 0; x < roomData.GetLength(0); x++)
        {
            for (int y = 0; y < roomData.GetLength(1); y++)
            {
                if (!roomData[x, y].TileTypeData.IsDependingOnRoomConnection) continue;
                string[] parsedId = roomData[x, y].TileTypeData.Id.Split('_');
                foreach (var (p1, p2) in poles)
                {
                    if (parsedId[2] == p1) parsedId[2] = p2;
                    else if (parsedId[2] == p2) parsedId[2] = p1;
                }

                roomData[x, y].TileTypeData = DataHolder.TileTypes[$"{parsedId[0]}_{parsedId[1]}_{parsedId[2]}"];
            }
        }
    }

    private void GenerateBorderWalls()
    {
        _westernWall = new Tile[_map.GetLength(1) * ChunkSize];
        _southernWall = new Tile[_map.GetLength(0) * ChunkSize];
        for (int mapX = 0; mapX < _map.GetLength(0); mapX++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                int tileX = mapX * ChunkSize + x;
                string wallKey = x == ChunkSize - 1 ? "wall_nesw" :
                    _map[mapX, 0].Tiles[x, 0].ConnectToWalls ? "wall_new" : "wall_ew";
                _southernWall[tileX] = new Tile(DataHolder.TileTypes[wallKey]);
            }
        }

        for (int mapY = 0; mapY < _map.GetLength(1); mapY++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                int tileY = mapY * ChunkSize + y;
                string wallKey = y == ChunkSize - 1 ? "wall_nesw" :
                    _map[0, mapY].Tiles[0, y].ConnectToWalls ? "wall_nes" : "wall_ns";
                _westernWall[tileY] = new Tile(DataHolder.TileTypes[wallKey]);
            }
        }
    }

    private void AddRoomToList(int x, int y, List<ChunkGenerator> accessList, List<ChunkGenerator> noAccessList)
    {
        if (x < 0 || x >= _tempMap.GetLength(0) || y < 0 || y >= _tempMap.GetLength(1)) return;

        if (_tempMap[x, y].HasAccessToMainRoute) accessList.Add(_tempMap[x, y]);
        else noAccessList.Add(_tempMap[x, y]);
    }

    private void FillColumn(int x, int xStep, int yStep)
    {
        int startY = yStep == 1 ? 0 : _tempMap.GetLength(1) - 1;
        for (int i = 0, y = startY; i < _tempMap.GetLength(1); y += yStep, i++)
        {
            ChunkGenerator chunk = _tempMap[x, y];
            if (chunk.HasAccessToMainRoute)
            {
                continue;
            }

            ChunkGenerator yConnection = null;
            ChunkGenerator xConnection = _tempMap[x + xStep, y];
            int difY = y + yStep;
            if (difY >= 0 && difY < _tempMap.GetLength(1))
            {
                yConnection = _tempMap[x, difY];
            }

            if (chunk.IsStartPoint || chunk.IsEndPoint)
            {
                ConnectRooms(chunk, xConnection);
                if (yConnection is not null) ConnectRooms(chunk, yConnection);
                return;
            }

            if (Chance(ConnectionChance, _rng) && (!xConnection.HasAccessToMainRoute || !chunk.HasAccessToMainRoute))
            {
                ConnectRooms(chunk, xConnection);
            }

            if (yConnection is not null && Chance(ConnectionChance, _rng)
                && (!yConnection.HasAccessToMainRoute || !chunk.HasAccessToMainRoute))
            {
                ConnectRooms(chunk, yConnection);
            }
        }
    }

    private void ConnectRooms(ChunkGenerator room1, ChunkGenerator room2)
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

        if (room1.HasAccessToMainRoute) room2.AccessMainRoute(room1.Difficulty);
        if (room2.HasAccessToMainRoute) room1.AccessMainRoute(room2.Difficulty);
    }

    /// <summary>
    /// Debug method
    /// </summary>
    private string CalculateMapString()
    {
        int width = _tempMap.GetLength(0);
        int height = _tempMap.GetLength(1);
        StringBuilder sb = new StringBuilder((_tempMap.GetLength(0) + 1) * _tempMap.GetLength(1));
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (_tempMap[x, y].IsStartPoint)
                {
                    sb.Append('S');
                    continue;
                }

                sb.Append(_tempMap[x, y].CalculateSymbol());
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }
}