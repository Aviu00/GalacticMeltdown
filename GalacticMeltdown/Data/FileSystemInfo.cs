using System;
using System.IO;

namespace GalacticMeltdown.Data;

public static class FileSystemInfo
{
    public static readonly string ProjectDirectory =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
}