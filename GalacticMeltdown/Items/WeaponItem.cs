using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;
using AmmoDictionary = Dictionary<string, (int reloadAmount, int minDamage, int maxDamage,
     IEnumerable<ActorStateChangerData> actorStateChangerData)>;

public class WeaponItem : EquippableItem
{
    private readonly WeaponItemData _itemData;
    [JsonProperty] protected override string ItemType => "MeleeWeapon";
    [JsonIgnore] public int MinHitDamage => _itemData.MinHitDamage;
    [JsonIgnore] public int MaxHitDamage => _itemData.MaxHitDamage;
    [JsonIgnore] public int HitEnergy => _itemData.HitEnergy;
    [JsonIgnore] public AmmoDictionary AmmoTypes => _itemData.AmmoTypes;

    public WeaponItem(WeaponItemData data) : base(data)
    {
        _itemData = data;
    }

    public WeaponItem(WeaponItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }

    [JsonConstructor]
    protected WeaponItem(string id) : base((WeaponItemData) DataHolder.ItemTypes[id])
    {
        _itemData = (WeaponItemData)DataHolder.ItemTypes[id];
    }

    public override void Equip(Actor actor)
    {
    }

    public override void UnEquip(Actor actor)
    {
    }
}