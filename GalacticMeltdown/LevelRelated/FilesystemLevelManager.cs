using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
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

        RemoveLevel(path); //rewrite level
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "level.json"), levelStr);
        // returns false on failure
        return true;
    }

    public static bool RemoveLevel(string name)
    {
        if (!Directory.Exists(Path.Combine(GetSaveFolder(), name))) return false;
        
        var directory = new DirectoryInfo(name);
        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in directory.GetDirectories())
        {
            dir.Delete(true);
        }
        
        return true;
    }

    private static string GetSaveFolder()
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string home = Environment.GetEnvironmentVariable("HOME");
        return Path.Combine(home, isWindows
            ? @"\AppData\Roaming\galactic-meltdown\levels"
            : ".local/share/galactic-meltdown/levels");
    }
}