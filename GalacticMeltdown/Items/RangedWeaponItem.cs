using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class RangedWeaponItem : WeaponItem
{
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