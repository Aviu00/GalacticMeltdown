using System;
using System.Collections.Generic;

namespace GalacticMeltdown
{
    public static class Utility
    {
        public static readonly Dictionary<string, ConsoleColor> Colors = new()
        {
            {"white", ConsoleColor.White},
            {"black", ConsoleColor.Black},
            {"blue", ConsoleColor.Blue},
            {"cyan", ConsoleColor.Cyan},
            {"green", ConsoleColor.Green},
            {"gray", ConsoleColor.Gray},
            {"magenta", ConsoleColor.Magenta},
            {"red", ConsoleColor.Red},
            {"yellow", ConsoleColor.Yellow},
            {"dark_gray", ConsoleColor.DarkGray},
            {"dark_blue", ConsoleColor.DarkBlue},
            {"dark_cyan", ConsoleColor.DarkCyan},
            {"dark_green", ConsoleColor.DarkGreen},
            {"dark_magenta", ConsoleColor.DarkMagenta},
            {"dark_red", ConsoleColor.DarkRed},
            {"dark_yellow", ConsoleColor.DarkYellow},
        };
        public static readonly ConsoleColor OutOfVisionTileColor = ConsoleColor.Black;
        
        
        public static (int x, int y) ConvertGlobalToRelativeCords(int x, int y, int relObjX, int relObjY)
        {
            return (x - relObjX, y - relObjY);
        }
        public static (int x, int y) ConvertRelativeToGlobalCords(int x, int y, int relObjX, int relObjY)
        {
            return (x + relObjX, y + relObjY);
        }
    }
}