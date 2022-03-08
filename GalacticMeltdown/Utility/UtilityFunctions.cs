using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticMeltdown.Utility;

public static class UtilityFunctions
{
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
        return rng.Next(1, 101) <= chance;
    }

    public static int MultiChance(Random rng = null, params int[] chances)
    {
        rng ??= Random.Shared;
        if (chances.Length == 0) throw new ArgumentException();
        int val = rng.Next(1, 101);
        int curChance = chances[0];
        for (int i = 0; i < chances.Length; i++)
        {
            if (val <= curChance) return i;
            if (i + 1 == chances.Length) break;
            curChance += chances[i + 1];
        }

        throw new ArgumentException();
    }

    public static Dictionary<TKey, TVal> JoinDictionaries<TKey, TJoin, TVal>(Dictionary<TKey, TJoin> dict1,
        Dictionary<TJoin, TVal> dict2)
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
            builder.Append((char) Random.Shared.Next('A', 'Z' + 1));
        }

        return builder.ToString();
    }
}