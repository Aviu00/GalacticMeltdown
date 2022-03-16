using GalacticMeltdown.Data;

namespace GalacticMeltdown.Items;

public class UsableItem : Item
{
    private readonly UsableItemData _itemData;
    public UsableItem(UsableItemData data) : base(data)
    {
        _itemData = data;
    }

    public UsableItem(UsableItem item) : base(item._itemData)
    {
        _itemData = item._itemData;
    }
}