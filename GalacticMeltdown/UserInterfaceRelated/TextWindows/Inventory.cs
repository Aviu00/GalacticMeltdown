using System.Collections.Generic;
using GalacticMeltdown.Items;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class Inventory : Menu
{
    private Dictionary<ItemCategory, List<Item>> _inventory;

    public Inventory(Dictionary<ItemCategory, List<Item>> inventory)
    {
        _inventory = inventory;
        LineView = new LineView();
    }
}