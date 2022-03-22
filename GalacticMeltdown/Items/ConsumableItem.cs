using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class ConsumableItem : Item
{
    public Action<Player> Consume { get; } 

    private readonly ConsumableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Usable";
    public ConsumableItem(ConsumableItemData data) : base(data)
    {
        _itemData = data;
    }

    public ConsumableItem(ConsumableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }

    [JsonConstructor]
    private ConsumableItem(string id) : this((ConsumableItemData) DataHolder.ItemTypes[id])
    {
    }
}