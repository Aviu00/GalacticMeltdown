using System;
using System.Collections.Generic;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemDialog : ChoiceDialog
{
    private const string Drop = "Drop";
    private const string Equip = "Equip";
    private const string Use = "Use";

    private static LinkedList<string> GetChoices(Item item)
    {
        var choices = new LinkedList<string>();
        choices.AddFirst(Drop);
        switch (item)
        {
            case EquippableItem:
                choices.AddFirst(Equip);
                break;
            case ConsumableItem:
                choices.AddFirst(Use);
                break;
        }

        return choices;
    }

    public ItemDialog(Item item, Action<string> send) : base(GetChoices(item), send)
    {
    }
}