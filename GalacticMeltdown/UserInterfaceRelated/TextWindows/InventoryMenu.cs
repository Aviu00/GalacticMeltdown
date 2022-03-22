using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class InventoryMenu : TextWindow
{
    private Dictionary<ItemCategory, List<Item>> _inventory;
    private int _currentCategory;

    private readonly Player _player;

    public InventoryMenu(Player player)
    {
        _player = player;
        _inventory = _player.Inventory;
        _currentCategory = 0;
        LoadCategoryScreen(Enum.GetValues<ItemCategory>()[_currentCategory]);
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Back, Close},
            {SelectionControl.Select, LineView.PressCurrent},
            {SelectionControl.Down, LineView.SelectNext},
            {SelectionControl.Up, LineView.SelectPrev},
            {SelectionControl.Left, OpenPreviousCategory},
            {SelectionControl.Right, OpenNextCategory},
        }));
    }

    private void LoadCategoryScreen(ItemCategory category)
    {
        List<ListLine> lines = new List<ListLine>();
        lines.Add(new TextLine($"<--- {DataHolder.CategoryName[category]} --->"));
        List<Item> items = _inventory[category];
        if (!items.Any())
        {
            lines.Add(new TextLine("Nothing here yet"));
            LineView.SetLines(lines);
            return;
        }

        List<Item> itemLines = new List<Item>();
        Dictionary<string, int> stackableItemCounts = new();
        foreach (var item in items)
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

        lines.AddRange(itemLines.Select(itemLine => itemLine.Stackable
                ? new ItemButton(itemLine, OpenItemDialog, stackableItemCounts[itemLine.Id])
                : new ItemButton(itemLine, OpenItemDialog))
            .Cast<ListLine>()
            .ToList());
        LineView.SetLines(lines);
    }

    private void OpenItemDialog(Item item)
    {
        ItemDialog itemDialog = new(item, choice => ProcessChoice(item, choice));
        UserInterface.AddChild(this, itemDialog);
        itemDialog.Open();
    }

    private void ProcessChoice(Item item, ItemAction choice)
    {
        switch (choice)
        {
            case ItemAction.Drop:
                if (item.Stackable)
                {
                    foreach (Item listItem in _inventory[item.Category].FindAll(listItem => listItem.Id == item.Id).ToList())
                    {
                        _player.Drop(listItem);
                    }
                }
                else
                {
                    _player.Drop(item);
                }

                break;
            case ItemAction.Equip:
                _player.Equip((EquippableItem) item);
                break;
            case ItemAction.Consume:
                _player.Consume((ConsumableItem) item);
                break;
        }
        UpdateCurrentScreen();
    }

    private void OpenPreviousCategory()
    {
        _currentCategory -= 1;
        if (_currentCategory == -1) _currentCategory = Enum.GetValues<ItemCategory>().Length - 1;
        UpdateCurrentScreen();
    }
    
    private void OpenNextCategory()
    {
        _currentCategory = (_currentCategory + 1) % Enum.GetValues<ItemCategory>().Length;
        UpdateCurrentScreen();
    }

    private void UpdateCurrentScreen()
    {
        LoadCategoryScreen(Enum.GetValues<ItemCategory>()[_currentCategory]);
    }
}