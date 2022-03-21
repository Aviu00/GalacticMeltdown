using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.Utility;
using ItemDictionary = Dictionary<(int x, int y), List<Item>>;

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

    public static bool ChanceRoll(int chance, int rolls, Random rng = null)
    {
        rng ??= Random.Shared;
        return rng.Next(1, 101) <= 100 - Math.Pow(100-chance, rolls) / Math.Pow(100, rolls-1);
    }

    public static int MultiChance(int[] chances, Random rng = null)
    {
        rng ??= Random.Shared;
        if (chances.Length == 0) throw new ArgumentException("chances array must have non zero length");
        int val = rng.Next(1, 101);
        int curChance = chances[0];
        for (int i = 0; i < chances.Length; i++)
        {
            if (val <= curChance) return i;
            if (i + 1 == chances.Length) break;
            curChance += chances[i + 1];
        }

        return chances.Length;
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

    public static double GetDistance(int x0, int y0, int x1, int y1)
    {
        int xDist = x1 - x0;
        int yDist = y1 - y0;
        return Math.Sqrt(xDist * xDist + yDist * yDist);
    }

    public static int CalculateChanceToHitForRangeEnemy(int distance, int spread)
    {
        if (spread < 0) return 0;
        return (int) (100 - 5 * distance * Math.Log(spread));
    }

    public static int CountDigits(int num) => num == 0 ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(num))) + 1;
}