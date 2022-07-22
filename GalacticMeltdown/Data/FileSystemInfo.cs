using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GalacticMeltdown.Data;

public static class FileSystemInfo
{
    public static readonly string ProjectDirectory =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));

    public static readonly string LevelDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"AppData\Roaming\galactic-meltdown\levels"
            : ".local/share/galactic-meltdown/levels");
}