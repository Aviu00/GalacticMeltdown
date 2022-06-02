using System.Collections.Generic;
using System.Linq;
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
    private ConsumableItem(string id) : this((ConsumableItemData) MapData.ItemTypes[id])
    {
    }

    public void Consume(Actor actor)
    {
        foreach (var stateChanger in _itemData.ActorStateChangerData)
        {
            StateChangerData.StateChangers[stateChanger.Type](actor, stateChanger.Power, stateChanger.Duration);
        }
    }
    
    public override List<string> GetDescription()
    {
        List<string> description = new() {Name};
        if (_itemData.ActorStateChangerData.Count <= 0) return description;
        description.Add("");
        description.Add("On consume:");
        description.AddRange(_itemData.ActorStateChangerData.Select(data =>
            StateChangerData.StateChangerDescriptions[data.Type](data.Power, data.Duration)));
        return description;
    }
}