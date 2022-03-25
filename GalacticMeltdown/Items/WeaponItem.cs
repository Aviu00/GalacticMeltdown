using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;
using AmmoDictionary = Dictionary<string, (int reloadAmount, int minDamage, int maxDamage,
     ActorStateChangerData actorStateChangerData)>;

public class WeaponItem : EquippableItem
{
    private readonly WeaponItemData _itemData;
    [JsonProperty] protected override string ItemType => "MeleeWeapon";
    [JsonIgnore] public int MinHitDamage => _itemData.MinHitDamage;
    [JsonIgnore] public int MaxHitDamage => _itemData.MaxHitDamage;
    [JsonIgnore] public int HitEnergy => _itemData.HitEnergy;
    [JsonIgnore] public int AmmoCapacity => _itemData.AmmoCapacity;
    [JsonIgnore] public AmmoDictionary AmmoTypes => _itemData.AmmoTypes;
    [JsonProperty] public readonly LimitedNumber AmmoAmount;

    public WeaponItem(WeaponItemData data) : base(data)
    {
        _itemData = data;
        AmmoAmount = new LimitedNumber(data.AmmoCapacity, data.AmmoCapacity, 0);
    }

    public WeaponItem(WeaponItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
        AmmoAmount = new LimitedNumber(item.AmmoAmount.Value, _itemData.AmmoCapacity, 0);
    }

    [JsonConstructor]
    protected WeaponItem(string id, LimitedNumber ammoAmount) : base((WeaponItemData) DataHolder.ItemTypes[id])
    {
        _itemData = (WeaponItemData)DataHolder.ItemTypes[id];
        AmmoAmount = ammoAmount;
    }

    public override void Equip(Actor actor)
    {
    }

    public override void Unequip(Actor actor)
    {
    }
}