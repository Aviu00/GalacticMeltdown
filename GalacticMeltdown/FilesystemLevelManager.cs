using System.Collections.Generic;

namespace GalacticMeltdown;

public readonly record struct LevelInfo(string Path, int Seed, string Name); 

public static class FilesystemLevelManager
{
    public static List<LevelInfo> GetLevelInfo()
    {
        return new List<LevelInfo> {new(".", 0, "ExampleName")};
    }

    public static void RemoveLevel(string path)
    {
        
    }

    public static void SaveLevel(Map level)
    {
        
    }

    public static Map GetLevel(string path)
    {
        Map map = new MapGenerator(1).Generate();
        return map;
    }
}