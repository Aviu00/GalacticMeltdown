using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Items;

public class UsableItem : Item
{
    private readonly UsableItemData _itemData;
    [JsonProperty] protected override string ItemType => "Usable";
    public UsableItem(UsableItemData data) : base(data)
    {
        _itemData = data;
    }

    public UsableItem(UsableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }

    [JsonConstructor]
    private UsableItem(string id) : this((UsableItemData) DataHolder.ItemTypes[id])
    {
    }
}