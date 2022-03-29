using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows.ItemManagement;

public class CategoryDialog : ChoiceDialog<ItemCategory?>
{
    public CategoryDialog(Action<ItemCategory?> send) : base(GetChoices(), send)
    {
    }

    private static List<(string text, ItemCategory? choice)> GetChoices()
    {
        var choices = new List<(string text, ItemCategory? choice)> { ("All", null) };
        choices.AddRange(Enum.GetValues<ItemCategory>()
            .Select(category => (DataHolder.CategoryName[category], (ItemCategory?) category)));
        return choices;
    }
}