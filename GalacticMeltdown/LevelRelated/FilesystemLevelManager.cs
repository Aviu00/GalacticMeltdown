using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GalacticMeltdown.MapGeneration;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public readonly record struct LevelInfo(int Seed, string Name);

public static class FilesystemLevelManager
{
    public static List<LevelInfo> GetLevelInfo()
    {
        List<LevelInfo> levelInfos = new();
        string path = GetSaveFolder();
        foreach (var dir in Directory.GetDirectories(path))
        {
            levelInfos.Add(new LevelInfo(0, Path.GetFileName(dir)));
        }
        return levelInfos;
    }
    
    public static Level CreateLevel(int seed, string name)
    {
        Level level = new MapGenerator(seed).Generate();
        // Save seed and name
        SaveLevel(level, name);
        return level;
    }
    
    public static Level GetLevel(string name)
    {
        string path = Path.Combine(GetSaveFolder(), name, "level.json");
        try
        {
            return JsonConvert.DeserializeObject<Level>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static bool SaveLevel(Level level, string name)
    {
        string path = Path.Combine(GetSaveFolder(), name, "level.json");
        string levelStr = JsonConvert.SerializeObject(level, Formatting.None, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        });

    File.WriteAllText(path, levelStr);
        // returns false on failure
        return true;
    }

    public static bool RemoveLevel(string name)
    {
        // returns false on failure
        return true;
    }

    private static string GetSaveFolder()
    {
        return Path.Combine(Environment.GetEnvironmentVariable("HOME") + "/.local/share/galactic-meltdown/levels");
    }
}