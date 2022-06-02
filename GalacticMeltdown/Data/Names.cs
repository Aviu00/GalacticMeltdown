using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.Data;

public static class Names
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

    public static readonly Dictionary<ItemCategory, string> ItemCategories = new()
    {
        {ItemCategory.Item, "Other"},
        {ItemCategory.ConsumableItem, "Consumables"},
        {ItemCategory.WeaponItem, "Weapons"},
        {ItemCategory.EquippableItem, "Armor"},
        {ItemCategory.RangedWeaponItem, "Guns"}
    };

    public static readonly Dictionary<BodyPart, string> BodyParts = new()
    {
        {BodyPart.Head, "Head"},
        {BodyPart.Torso, "Torso"},
        {BodyPart.Legs, "Legs"},
        {BodyPart.Feet, "Feet"},
        {BodyPart.Hands, "Hands"},
    };
}