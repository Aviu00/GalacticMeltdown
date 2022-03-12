using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Items;

public class ItemObject : IObjectOnMap
{
    public readonly ItemData ItemData;
    public int X { get; }
    public int Y { get; }
    public (char symbol, ConsoleColor color) SymbolData => (ItemData.Symbol, ConsoleColor.White);
    public ConsoleColor? BgColor => ConsoleColor.Cyan;

    public string Name => ItemData.Name;

    public LimitedNumber Amount;

    public ItemObject(ItemData data, LimitedNumber amount, int x, int y)
    {
        Amount = amount;
        ItemData = data;
        X = x;
        Y = y;
    }

    public ItemObject(ItemData data, int amount, int x, int y)
        : this(data, new LimitedNumber(amount, 999, 0), x, y)
    {
    }
}