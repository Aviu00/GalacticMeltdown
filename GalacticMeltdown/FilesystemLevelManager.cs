using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;

namespace GalacticMeltdown;

public readonly record struct LevelInfo(string Path, int Seed, string Name); 

public class FilesystemLevelManager
{
    public List<LevelInfo> GetLevels()
    {
        return new List<LevelInfo> {new(".", 0, "ExampleName")};
    }

    public void RemoveLevel(string path)
    {
        
    }

    public void SaveLevel(Map level)
    {
        
    }

    public Map GetLevel(string path)
    {
        Map map;
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(1, tileTypes, rooms);
        map = mapGen.Generate();
        rooms = null;
        tileTypes = null;
        mapGen = null;
        GC.Collect();
        return map;
    }
}