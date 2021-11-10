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


        //convert relative cords to global and vice versa
        public static (int, int) GetRelativeCoordinates(int x, int y, int relX, int relY)
        {
            return (x - relX, y - relY);
        }
        public static (int, int) GetRelativeCoordinates((int, int) cords, int relX, int relY)
        {
            return (cords.Item1 - relX, cords.Item2 - relY);
        }
        public static (int, int) GetGlobalCoordinates(int x, int y, int relX, int relY)
        {
            return (x + relX, y + relY);
        }
        public static (int, int) GetGlobalCoordinates((int, int) cords, int relX, int relY)
        {
            return (cords.Item1 + relX, cords.Item2 + relY);
        }
    }
}