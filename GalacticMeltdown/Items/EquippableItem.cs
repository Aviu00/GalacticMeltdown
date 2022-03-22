using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class EquippableItem : Item
{
    [JsonIgnore] public BodyPart BodyPart => _itemData.BodyPart;
    
    private readonly WearableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Equippable";
    public EquippableItem(WearableItemData data) : base(data)
    {
        _itemData = data;
    }

    public EquippableItem(EquippableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }
    
    [JsonConstructor]
    private EquippableItem(string id) : this((WearableItemData) DataHolder.ItemTypes[id])
    {
    }
}