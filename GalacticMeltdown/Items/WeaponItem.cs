using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Items;
using AmmoDictionary = Dictionary<string, (int reloadAmount, int reloadEnergy, int minDamage, int maxDamage)>;

public class WeaponItem : Item
{
    private readonly WeaponItemData _itemData;
    public int MinHitDamage => _itemData.MinHitDamage;
    public int MaxHitDamage => _itemData.MaxHitDamage;
    public int HitEnergy => _itemData.HitEnergy;
    public int AmmoCapacity => _itemData.AmmoCapacity;
    public AmmoDictionary AmmoTypes => _itemData.AmmoTypes;
    public LimitedNumber AmmoAmount;
    public readonly bool isRanged;

    public WeaponItem(WeaponItemData data) : base(data)
    {
        if (data is RangedWeaponItemData) isRanged = true;
        _itemData = data;
        AmmoAmount = new LimitedNumber(data.AmmoCapacity, data.AmmoCapacity, 0);
    }

    public WeaponItem(WeaponItem item) : base(item._itemData)
    {
        if (item is RangedWeaponItem) isRanged = true;
        _itemData = item._itemData;
        AmmoAmount = new LimitedNumber(item.AmmoAmount.Value, _itemData.AmmoCapacity, 0);
    }
}