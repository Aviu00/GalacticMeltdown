using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ItemPickupDialog : TextWindow
{
    private Action<Item> _pickUp;
    private List<Item> _items;
    
    public ItemPickupDialog(List<Item> items, Action<Item> pickUp)
    {
        _pickUp = pickUp;
        _items = items;
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Back, Close},
            {SelectionControl.Select, LineView.PressCurrent},
            {SelectionControl.Down, LineView.SelectNext},
            {SelectionControl.Up, LineView.SelectPrev},
        }));
        UpdateLines();
    }

    private void UpdateLines()
    {
        List<Item> itemLines = new List<Item>();
        Dictionary<string, int> stackableItemCounts = new();
        foreach (var item in _items)
        {
            if (item.Stackable)
            {
                if (!stackableItemCounts.ContainsKey(item.Id))
                {
                    stackableItemCounts[item.Id] = 0;
                    itemLines.Add(item);
                }

                stackableItemCounts[item.Id] += 1;
            }
            else
            {
                itemLines.Add(item);
            }
        }

        List<ListLine> lines = itemLines.Select(itemLine => itemLine.Stackable
                ? new ItemButton(itemLine, PickUp, stackableItemCounts[itemLine.Id])
                : new ItemButton(itemLine, PickUp))
            .Cast<ListLine>()
            .ToList();
        if (!lines.Any())
        {
            Close();
            return;
        }
        LineView.SetLines(lines);
    }

    private void PickUp(Item item)
    {
        _pickUp(item);
        UpdateLines();
    }
}