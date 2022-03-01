using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticMeltdown;

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

    public static readonly ConsoleColor OutOfVisionTileColor = ConsoleColor.DarkGray;

    public static (int x, int y) ConvertAbsoluteToRelativeCoords(int x, int y, int relObjX, int relObjY)
    {
        return (x - relObjX, y - relObjY);
    }

    public static (int x, int y) ConvertRelativeToAbsoluteCoords(int x, int y, int relObjX, int relObjY)
    {
        return (x + relObjX, y + relObjY);
    }
    
    public static bool Chance(int chance, Random rng = null)
    {
        rng ??= Random.Shared;
        int val = rng.Next(1, 101);
        return val <= chance;
    }
    
    public static int MultiChance(Random rng = null, params int[] chances)
    {
        rng ??= Random.Shared;
        if (chances.Length == 0)
            throw new ArgumentException();
        int val = rng.Next(1, 101);
        int curChance = chances[0];
        for (int i = 0; i < chances.Length; i++)
        {
            if (val <= curChance)
                return i;
            if (i+1 == chances.Length)
                break;
            curChance += chances[i+1];
        }
        throw new ArgumentException();
    }

    public static Dictionary<TKey, TVal> JoinDictionaries<TKey, TJoin, TVal>
        (Dictionary<TKey, TJoin> dict1, Dictionary<TJoin, TVal> dict2)
    {
        Dictionary<TKey, TVal> dictionary = new();
        foreach (var (key, value) in dict1)
        {
            if (dict2.ContainsKey(value)) dictionary.Add(key, dict2[value]);
        }

        return dictionary;
    }

    public static string RandomString(int size)
    {
        StringBuilder builder = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            builder.Append((char) Random.Shared.Next('A', 'Z'));
        }

        return builder.ToString();
    }
}