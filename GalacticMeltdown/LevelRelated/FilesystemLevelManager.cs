using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GalacticMeltdown.MapGeneration;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public readonly record struct LevelInfo(string Path, string Name, int Seed);

public static class FilesystemLevelManager
{
    public static List<LevelInfo> GetLevelInfo()
    {
        List<LevelInfo> levelInfos = new();
        string saveFolderPath = GetSaveFolder();
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath!);
        foreach (string dir in Directory.GetDirectories(saveFolderPath))
        {
            string[] fileLines = File.ReadAllText(Path.Combine(dir, "info.txt")).Split('\n');
            if (fileLines.Length < 2) continue;
            if (!int.TryParse(fileLines[0], out int seed))
            {
                seed = -1;
            }
            string name = fileLines[1];

            levelInfos.Add(new LevelInfo(dir, name, seed));
        }

        return levelInfos;
    }

    public static Level CreateLevel(int seed, string name, out string path)
    {
        Level level = new MapGenerator(seed).Generate();
        // Save seed and name
        while (Directory.Exists(path = Path.Combine(GetSaveFolder(), Utility.UtilityFunctions.RandomString(32)))) {}
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "info.txt"), seed.ToString() + '\n' + name);
        SaveLevel(level, path);
        return level;
    }

    public static Level GetLevel(string path)
    {
        return JsonConvert.DeserializeObject<Level>(File.ReadAllText(Path.Combine(path, "level.json")));
    }

    public static void SaveLevel(Level level, string path)
    {
        string levelStr = JsonConvert.SerializeObject(level, Formatting.None, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });
        
        File.WriteAllText(Path.Combine(path, "level.json"), levelStr);
    }

    public static void RemoveLevel(string path)
    {
        Directory.Delete(path, true);
    }

    private static string GetSaveFolder()
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, isWindows
            ? @"AppData\Roaming\galactic-meltdown\levels" : ".local/share/galactic-meltdown/levels");
    }
}