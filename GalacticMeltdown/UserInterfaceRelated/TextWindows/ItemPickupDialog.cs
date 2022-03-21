using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemPickupDialog : ChoiceDialog<Item>
{
    private List<Item> _items;
    
    public ItemPickupDialog(List<Item> items, Action<Item> pickUp) 
        : base(items.Select(item => (item.Name, item)).ToList(), pickUp, "Choose an item to pickup")
    {
    }
}