using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemPickupDialog : ChoiceDialog<Item>
{
    public ItemPickupDialog(List<Item> items, Action<Item> pickUp) 
        : base(items.Select(item => (item.Name, item)).ToList(), pickUp, "Choose an item to pickup", false)
    {
    }

    protected override void SendChoice(Item choice)
    {
        for (var i = 0; i < Options.Count; i++)
        {
            if (Options[i].choice == choice)
            {
                Options.RemoveAt(i);
                break;
            }
        }
        UpdateLines();
        base.SendChoice(choice);
        if (Options.Count == 0) Close();
    }
}