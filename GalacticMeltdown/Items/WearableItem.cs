using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class WearableItem : Item
{
    private readonly WearableItemData _itemData;
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