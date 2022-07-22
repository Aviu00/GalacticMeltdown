using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using GalacticMeltdown.MapGeneration;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public readonly record struct LevelInfo(string Path, string Name, int Seed);

public static class FilesystemLevelManager
{
    private const string LevelFileName = "level.json";
    private const string InfoFileName = "info.txt";
    
    public static List<LevelInfo> GetLevelInfo()
    {
        List<LevelInfo> levelInfos = new();
        string saveDirPath = Data.FileSystemInfo.LevelDirectory;
        if (!Directory.Exists(saveDirPath))
            Directory.CreateDirectory(saveDirPath);
        foreach (string dir in Directory.GetDirectories(saveDirPath))
        {
            if (!File.Exists(Path.Combine(dir, LevelFileName))) continue;
            try
            {
                string[] fileLines = File.ReadAllText(Path.Combine(dir, InfoFileName)).Split('\n');
                string name = fileLines.Length < 2 ? "noname" : fileLines[1];

                if (fileLines.Length == 0 || !int.TryParse(fileLines[0], out int seed))
                {
                    seed = -1;
                }

                levelInfos.Add(new LevelInfo(dir, name, seed));
            }
            catch (FileNotFoundException) { }
            catch (IOException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }
            catch (NotSupportedException) { }
        }

        return levelInfos;
    }

    public static Level CreateLevel(int seed, string name, out string path)
    {
        Level level = new MapGenerator(seed).Generate();
        while (Directory.Exists(path = Path.Combine(Data.FileSystemInfo.LevelDirectory,
                   Utility.UtilityFunctions.RandomString(32))))
        {
        }

        try
        {
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, InfoFileName), seed.ToString() + '\n' + name);
        }
        catch (FileNotFoundException) { }
        catch (IOException) { }
        catch (SecurityException) { }
        catch (UnauthorizedAccessException) { }
        catch (NotSupportedException) { }
        
        SaveLevel(level, path);
        return level;
    }

    public static Level GetLevel(string path)
    {
        try
        {
            return JsonConvert.DeserializeObject<Level>(File.ReadAllText(Path.Combine(path, LevelFileName)));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void SaveLevel(Level level, string path)
    {
        string levelStr = JsonConvert.SerializeObject(level, Formatting.None, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });

        try
        {
            File.WriteAllText(Path.Combine(path, LevelFileName), levelStr);
        }
        catch (FileNotFoundException) { }
        catch (IOException) { }
        catch (SecurityException) { }
        catch (UnauthorizedAccessException) { }
        catch (NotSupportedException) { }
    }

    public static void RemoveLevel(string path)
    {
        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}