using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class RangedWeaponItem : WeaponItem
{
    public int Spread => _itemData.Spread;
    public int ShootEnergy => _itemData.ShootEnergy;

    public Dictionary<string, (int reloadAmount, int reloadEnergy, int minDamage, int maxDamage, ActorStateChangerData
        actorStateChangerData)> ammoTypes =>
        _itemData.AmmoTypes;

    public ActorStateChangerData StateChangerData => _itemData.ActorStateChangerData;
    
    private readonly RangedWeaponItemData _itemData;
    [JsonProperty] protected override string ItemType => "RangedWeapon";

    public RangedWeaponItem(RangedWeaponItemData data) : base(data)
    {
        _itemData = data;
    }

    public RangedWeaponItem(RangedWeaponItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }
    
    [JsonConstructor]
    private RangedWeaponItem(string id, LimitedNumber ammoAmount) : base(id, ammoAmount)
    {
        _itemData = (RangedWeaponItemData)DataHolder.ItemTypes[id];
    }
}