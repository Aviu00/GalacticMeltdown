using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Items;

public class RangedWeaponItem : WeaponItem
{
    private readonly RangedWeaponItemData _itemData;

    public RangedWeaponItem(RangedWeaponItemData data) : base(data)
    {
        _itemData = data;
    }

    public RangedWeaponItem(RangedWeaponItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }
}