using System;
using System.Collections.Generic;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemDialog : ChoiceDialog<string>
{
    private const string Drop = "Drop";
    private const string Equip = "Equip";
    private const string Consume = "Consume";

    private static LinkedList<(string text, string choice)> GetChoices(Item item)
    {
        var choices = new LinkedList<(string text, string choice)>();
        choices.AddFirst(("Drop", Drop));
        switch (item)
        {
            case EquippableItem:
                choices.AddFirst(("Equip", Equip));
                break;
            case ConsumableItem:
                choices.AddFirst(("Consume", Consume));
                break;
        }

        return choices;
    }

    public ItemDialog(Item item, Action<string> send) : base(GetChoices(item), send)
    {
    }
}