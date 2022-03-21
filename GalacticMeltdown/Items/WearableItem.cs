using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class WearableItem : Item
{
    public BodyPart BodyPart => _itemData.BodyPart;
    
    private readonly WearableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Wearable";
    public WearableItem(WearableItemData data) : base(data)
    {
        _itemData = data;
    }

    public WearableItem(WearableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }
    
    [JsonConstructor]
    private WearableItem(string id) : this((WearableItemData)DataHolder.ItemTypes[id])
    {
    }
}