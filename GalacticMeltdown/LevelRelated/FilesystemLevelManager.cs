using System;
using System.Collections.Generic;
using System.IO;
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
        return JsonConvert.DeserializeObject<Level>(File.ReadAllText(path));
    }

    public static bool SaveLevel(Level level, string name)
    {
        string path = Path.Combine(GetSaveFolder(), name);
        string levelStr = JsonConvert.SerializeObject(level, Formatting.None, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        });


        if (Directory.Exists(path)) //rewrite level
        {
            var directory = new DirectoryInfo(path);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete(); 
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                dir.Delete(true); 
            }
        }
        else
            Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "level.json"), levelStr);
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