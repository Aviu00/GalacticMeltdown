using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

[JsonConverter(typeof(JsonSubtypes), "ItemType")]
[JsonSubtypes.KnownSubType(typeof(RangedWeaponItem), "RangedWeapon")]
[JsonSubtypes.KnownSubType(typeof(WeaponItem), "MeleeWeapon")]
[JsonSubtypes.KnownSubType(typeof(ConsumableItem), "Consumable")]
[JsonSubtypes.KnownSubType(typeof(EquippableItem), "Equippable")]
public class Item : IDrawable
{
    private readonly ItemData _itemData;
    [JsonIgnore] public bool Stackable => _itemData.Stackable;
    [JsonIgnore] public ItemCategory Category => _itemData.Category;
    public string Id => _itemData.Id;
    [JsonIgnore] public string Name => _itemData.Name;
    [JsonIgnore] public (char symbol, ConsoleColor color) SymbolData => (_itemData.Symbol, ConsoleColor.White);
    [JsonIgnore] public ConsoleColor? BgColor => ConsoleColor.Cyan;
    
    [JsonProperty] protected virtual string ItemType { get; }

    protected Item(ItemData data)
    {
        _itemData = data;
    }

    protected Item(Item item) : this(item._itemData)
    {
    }

    [JsonConstructor]
    private Item(string id) : this(DataHolder.ItemTypes[id])
    {
    }
    
    public static Item CreateItem(ItemData data)
    {
        return data switch
        {
            RangedWeaponItemData itemData => new RangedWeaponItem(itemData),
            WeaponItemData itemData => new WeaponItem(itemData),
            ConsumableItemData itemData => new ConsumableItem(itemData),
            WearableItemData itemData => new EquippableItem(itemData),
            _ => new Item(data)
        };
    }
    
    public static Item CreateItem(Item item)
    {
        return item switch
        {
            RangedWeaponItem itemObj => new RangedWeaponItem(itemObj),
            WeaponItem itemObj => new WeaponItem(itemObj),
            ConsumableItem itemObj => new ConsumableItem(itemObj),
            EquippableItem itemObj => new EquippableItem(itemObj),
            _ => new Item(item)
        };
    }
}