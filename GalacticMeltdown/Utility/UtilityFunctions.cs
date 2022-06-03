using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.Utility;
using ItemDictionary = Dictionary<(int x, int y), List<Item>>;

public static class UtilityFunctions
{
    public static string RenderText(int width, string textLeft, string textRight)
    {
        const string ellipsis = "...";
        const string separator = "  ";
        const string noSpaceForRightText = $"{separator}{ellipsis}";
        int maxLeftStringLength = width - noSpaceForRightText.Length;
        string text;
        
        if (width < ellipsis.Length)
        {
            return new string(' ', width);
        }
        
        if (textRight.Length == 0)
        {
            text = textLeft.Length > width
                ? textLeft.Substring(0, width - ellipsis.Length) + ellipsis
                : textLeft.PadRight(width);
        }
        else if (textLeft.Length == 0)
        {
            text = textRight.Length > width
                ? ellipsis + textRight.Substring(textRight.Length - (width - ellipsis.Length))
                : textRight.PadLeft(width);
        }
        else if (textLeft.Length >= maxLeftStringLength)
        {
            if (maxLeftStringLength < 0) text = new string(' ', width);
            else text = textLeft.Substring(0, maxLeftStringLength) + noSpaceForRightText;
        }
        else
        {
            text = textLeft;
            text += separator;
            int spaceLeft = width - text.Length;
            text += textRight.Length > spaceLeft
                ? textRight.Substring(0, spaceLeft - ellipsis.Length) + ellipsis
                : textRight.PadLeft(spaceLeft);
        }

        return text;
    }

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

    public static double GetDistance(int x0, int y0, int x1, int y1)
    {
        int xDist = x1 - x0;
        int yDist = y1 - y0;
        return Math.Sqrt(xDist * xDist + yDist * yDist);
    }

    public static int RangeAttackHitChance(double distance, int spread)
    {
        if (spread < 0) return 0;
        return  100 - (int) (5 * distance * Math.Log10(spread + 1));
    }

    public static int CalculateMeleeDamage(int min, int max, int strength)
    {
        int damage = Random.Shared.Next(min, max + 1);
        if(strength < Actor.ActorStr)
            return damage / (1 + (Actor.ActorStr - strength) / 10);
        return damage + 5 * (strength - Actor.ActorStr);
    }

    public static bool ObjectInSquareArea(int x0, int y0, int x1, int y1, int radius)
    {
        return Math.Abs(x0 - x1) <= radius && Math.Abs(y0 - y1) <= radius;
    }

    public static void ApplyStateChangers(IEnumerable<ActorStateChangerData> stateChangers, Actor target)
    {
        if (stateChangers is null) return;
        foreach (var changer in stateChangers)
        {
            StateChangerData.StateChangers[changer.Type](target, changer.Power, changer.Duration);
        }
    }

    public static int CountDigits(int num) => num == 0 ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(num))) + 1;
}