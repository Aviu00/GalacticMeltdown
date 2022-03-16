using GalacticMeltdown.Data;

namespace GalacticMeltdown.Items;

public class WeaponItem : Item
{
    private readonly WeaponItemData _itemData;
    public int MinHitDamage => _itemData.MinHitDamage;
    public int MaxHitDamage => _itemData.MaxHitDamage;
    public int HitEnergy => _itemData.HitEnergy;
    public WeaponItem(WeaponItemData data) : base(data)
    {
        _itemData = data;
    }

    public WeaponItem(WeaponItem item) : this(item._itemData)
    {
        _itemData = item._itemData;
    }
}