using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GalacticMeltdown.MapGeneration;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public readonly record struct LevelInfo(int Seed, string Name);

public static class FilesystemLevelManager
{
    public static List<LevelInfo> GetLevelInfo()
    {
        List<LevelInfo> levelInfos = new();
        string path = GetSaveFolder();
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path!);
        foreach (var dir in Directory.GetDirectories(path))
        {
            string name = Path.GetFileName(dir);
            int seed;
            try
            {
                seed = Convert.ToInt32(File.ReadAllText(Path.Combine(path, name, "seed.txt")));
            }
            catch (Exception)
            {
                seed = -1;
            }

            levelInfos.Add(new LevelInfo(seed, name));
        }

        return levelInfos;
    }

    public static Level CreateLevel(int seed, string name)
    {
        Level level = new MapGenerator(seed).Generate();
        // Save seed and name
        string path = Path.Combine(GetSaveFolder(), name);
        File.WriteAllText(Path.Combine(path, "seed.txt"), seed.ToString());
        SaveLevel(level, name);
        return level;
    }

    public static Level GetLevel(string name)
    {
        string path = Path.Combine(GetSaveFolder(), name, "level.json");
        return JsonConvert.DeserializeObject<Level>(File.ReadAllText(path));
    }

    public static void SaveLevel(Level level, string name)
    {
        string path = Path.Combine(GetSaveFolder(), name);
        string levelStr = JsonConvert.SerializeObject(level, Formatting.None, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });

        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "level.json"), levelStr);
    }

    public static void RemoveLevel(string name)
    {
        Directory.Delete(Path.Combine(GetSaveFolder(), name), true);
    }

    private static string GetSaveFolder()
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, isWindows
            ? @"AppData\Roaming\galactic-meltdown\levels" : ".local/share/galactic-meltdown/levels");
    }
}