using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Items;

public class Item //Item is an object inside the inventory; ItemObject is an object on map
{
    private readonly ItemData _itemData;

    public string Name => _itemData.Name;

    public char Symbol => _itemData.Symbol;

    public LimitedNumber Amount;

    public Item(ItemData data, LimitedNumber amount)
    {
        Amount = amount;
        _itemData = data;
    }

    public Item(ItemData data, int amount) : this(data, new LimitedNumber(amount, 999, 0))
    {
    }
}