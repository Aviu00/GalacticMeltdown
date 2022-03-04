using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public readonly record struct LevelInfo(string Path, int Seed, string Name); 

public static class FilesystemLevelManager
{
    public static List<LevelInfo> GetLevelButtonInfo()
    {
        return new List<LevelInfo> {new(".", 0, "ExampleName0"), 
            new("..", 5, "ExampleName1"),
            new("..", 5, "ExampleName2"),
            new("..", 5, "ExampleName3"),
            new("..", 5, "ExampleName4"),
            new("..", 5, "ExampleName5"),
            new("..", 5, "ExampleName6"),
            new("..", 5, "ExampleName7"),
            new("..", 5, "ExampleName8"),
            new("..", 5, "ExampleName9"),
            new("..", 5, "ExampleName10"),
            new("..", 5, "ExampleName11"),
            new("..", 5, "ExampleName12"),
            new("..", 5, "ExampleName13"),
            new("..", 5, "ExampleName14"),
            new("..", 5, "ExampleName15"),
            new("..", 5, "ExampleName16"),
            new("..", 5, "ExampleName17"),
            new("..", 5, "ExampleName18"),
            new("..", 5, "ExampleName19"),
            new("..", 5, "ExampleName20"),
            new("..", 5, "ExampleName21"),
            new("..", 5, "ExampleName22"),
        };
    }

    public static void RemoveLevel(string path)
    {
        
    }

    public static void SaveLevel(Level level)
    {
        
    }

    public static Level GetLevel(string path)
    {
        Level level = new MapGenerator(Random.Shared.Next(0, 1000000000)).Generate();
        return level;
    }
}