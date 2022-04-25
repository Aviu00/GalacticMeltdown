using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class RangedWeaponItem : WeaponItem
{
    [JsonIgnore] public int Spread => _itemData.Spread;
    [JsonIgnore] public int ShootEnergy => _itemData.ShootEnergy;

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
    private RangedWeaponItem(string id) : base(id)
    {
        _itemData = (RangedWeaponItemData) DataHolder.ItemTypes[id];
    }

    public override void Equip(Actor actor)
    {
    }

    public override void Unequip(Actor actor)
    {
    }
    
    public override List<string> GetDescription()
    {
        List<string> description = new()
        {
            Name,
            "",
            $"Deals {MinHitDamage}-{MaxHitDamage} damage on hit",
            $"Melee attack energy cost: {HitEnergy}",
            $"Shoot energy cost: {ShootEnergy}",
            $"Spread: {Spread}"
        };
        if (StateChangers is not null)
        {
            description.Add("Applies effects on target when shot:");
            description.AddRange(StateChangers.Select(data =>
                DataHolder.StateChangerDescriptions[data.Type](data.Power, data.Duration)));
        }
        if(AmmoTypes is not null) description.AddRange(GetAmmoDescription());
        return description;
    }
}