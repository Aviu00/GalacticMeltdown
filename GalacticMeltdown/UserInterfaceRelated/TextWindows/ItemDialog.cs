using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemDialog : ChoiceDialog<ItemAction>
{
    private static LinkedList<(string text, ItemAction choice)> GetChoices(Item item)
    {
        var choices = new LinkedList<(string text, ItemAction choice)>();
        choices.AddFirst(("Drop", ItemAction.Drop));
        switch (item)
        {
            case EquippableItem:
                choices.AddFirst(("Equip", ItemAction.Equip));
                break;
            case ConsumableItem:
                choices.AddFirst(("Consume", ItemAction.Consume));
                break;
        }

        return choices;
    }

    public ItemDialog(Item item, Action<ItemAction> send) : base(GetChoices(item).ToList(), send)
    {
    }
}