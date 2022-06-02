using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

using AmmoDictionary = Dictionary<string, (int minDamage, int maxDamage,
    LinkedList<ActorStateChangerData> actorStateChangerData)>;

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
    protected WeaponItem(string id) : base((WeaponItemData) MapData.ItemTypes[id])
    {
        _itemData = (WeaponItemData) MapData.ItemTypes[id];
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
            $"Attack energy cost: {HitEnergy}"
        };
        if (StateChangers is not null)
        {
            description.Add("Applies effects on target when hit:");
            description.AddRange(StateChangers.Select(data =>
                Data.StateChangerData.StateChangerDescriptions[data.Type](data.Power, data.Duration)));
        }

        if (AmmoTypes is not null) description.AddRange(GetAmmoDescription());
        return description;
    }

    protected IEnumerable<string> GetAmmoDescription()
    {
        List<string> description = new() {"Possible ammo types:"};
        foreach (var pair in AmmoTypes)
        {
            description.Add("");
            description.Add(MapData.ItemTypes[pair.Key].Name);
            description.Add($"Damage: {pair.Value.minDamage}-{pair.Value.maxDamage}");
            if (pair.Value.actorStateChangerData is null) continue;
            description.Add("Applies effects on target:");
            description.AddRange(pair.Value.actorStateChangerData.Select(data => 
                Data.StateChangerData.StateChangerDescriptions[data.Type](data.Power, data.Duration)));
        }

        return description;
    }
}