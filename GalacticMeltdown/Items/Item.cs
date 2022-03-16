using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Items;

public class Item : IDrawable
{
    private readonly ItemData _itemData;
    public string Id => _itemData.Id;
    public string Name => _itemData.Name;
    public (char symbol, ConsoleColor color) SymbolData => (_itemData.Symbol, ConsoleColor.White);
    public ConsoleColor? BgColor => ConsoleColor.Cyan;

    public Item(ItemData data)
    {
        _itemData = data;
    }
}