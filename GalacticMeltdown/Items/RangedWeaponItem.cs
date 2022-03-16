using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Items;

public class RangedWeaponItem : WeaponItem
{
    private readonly RangedWeaponItemData _itemData;
    public int MinShootDamage => _itemData.MinShootDamage;
    public int MaxShootDamage => _itemData.MaxShootDamage;
    public string AmmoId => _itemData.AmmoId;
    public int AmmoCapacity => _itemData.AmmoCapacity;
    public int ReloadEnergy => _itemData.ReloadEnergy;

    public LimitedNumber Ammo;
    public RangedWeaponItem(RangedWeaponItemData data) : base(data)
    {
        _itemData = data;
        Ammo = new LimitedNumber(data.AmmoCapacity, data.AmmoCapacity, 0);
    }

    public RangedWeaponItem(RangedWeaponItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
        Ammo = new LimitedNumber(item.Ammo.Value, _itemData.AmmoCapacity, 0);
    }
}