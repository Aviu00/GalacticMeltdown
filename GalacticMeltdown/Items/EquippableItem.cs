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

    [JsonIgnore] public ActorStateChangerData StateChanger => _itemData.ActorStateChangerData;
    
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
    private EquippableItem(string id) : this((WearableItemData) DataHolder.ItemTypes[id])
    {
    }

    public virtual void Equip(Actor actor)
    {
        DataHolder.ActorStateChangers[StateChanger.Type](actor, StateChanger.Power, 0);
    }

    public virtual void Unequip(Actor actor)
    {
        DataHolder.ActorStateChangers[StateChanger.Type](actor, -StateChanger.Power, 0);
    }
}