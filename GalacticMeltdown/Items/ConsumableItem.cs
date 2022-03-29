using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class ConsumableItem : Item
{
    private readonly ConsumableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Consumable";
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

    public void Consume(Actor actor)
    {
        foreach (var stateChanger in _itemData.ActorStateChangerData)
        {
            DataHolder.ActorStateChangers[stateChanger.Type](actor, stateChanger.Power, stateChanger.Duration);
        }
    }
}