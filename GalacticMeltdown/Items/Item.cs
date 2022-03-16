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

    protected Item(ItemData data)
    {
        _itemData = data;
    }

    protected Item(Item item) : this(item._itemData)
    {
    }
    
    public static Item CreateItem(ItemData data)
    {
        return data switch
        {
            RangedWeaponItemData itemData => new RangedWeaponItem(itemData),
            WeaponItemData itemData => new WeaponItem(itemData),
            UsableItemData itemData => new UsableItem(itemData),
            WearableItemData itemData => new WearableItem(itemData),
            _ => new Item(data)
        };
    }
    
    public static Item CreateItem(Item item)
    {
        return item switch
        {
            RangedWeaponItem itemObj => new RangedWeaponItem(itemObj),
            WeaponItem itemObj => new WeaponItem(itemObj),
            UsableItem itemObj => new UsableItem(itemObj),
            WearableItem itemObj => new WearableItem(itemObj),
            _ => new Item(item)
        };
    }
}