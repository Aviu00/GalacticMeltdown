using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

[JsonConverter(typeof(JsonSubtypes), "ItemType")]
[JsonSubtypes.KnownSubType(typeof(RangedWeaponItem), "RangedWeapon")]
[JsonSubtypes.KnownSubType(typeof(WeaponItem), "MeleeWeapon")]
[JsonSubtypes.KnownSubType(typeof(EquippableItem), "Equippable")]
public class EquippableItem : Item
{
    [JsonIgnore] public BodyPart BodyPart => _itemData.BodyPart;

    [JsonIgnore] public LinkedList<ActorStateChangerData> StateChangers => _itemData.ActorStateChangerData;

    private readonly WearableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Equippable";

    public EquippableItem(WearableItemData data) : base(data)
    {
        _itemData = data;
    }

    public EquippableItem(EquippableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }

    [JsonConstructor]
    private EquippableItem(string id) : this((WearableItemData) MapData.ItemTypes[id])
    {
    }

    public virtual void Equip(Actor actor)
    {
        foreach (var stateChanger in StateChangers)
        {
            DataHolder.ActorStateChangers[stateChanger.Type](actor, stateChanger.Power, 0);
        }
    }

    public virtual void Unequip(Actor actor)
    {
        foreach (var stateChanger in StateChangers)
        {
            DataHolder.ActorStateChangers[stateChanger.Type](actor, -stateChanger.Power, 0);
        }
    }

    public override List<string> GetDescription()
    {
        List<string> description = new() {Name};
        if (StateChangers.Count <= 0) return description;
        description.Add("");
        description.Add("On equip:");
        description.AddRange(StateChangers.Select(data =>
            DataHolder.StateChangerDescriptions[data.Type](data.Power, data.Duration)));
        return description;
    }
}