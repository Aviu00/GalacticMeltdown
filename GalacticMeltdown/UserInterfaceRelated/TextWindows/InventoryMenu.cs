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
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

internal class ItemButton : Button
{
    public Item StoredItem { get; }

    public ItemButton(Item item, Action<Item> openItemScreen, int? count = null) : base(item.Name,
        count is null ? "" : $"{count}", () => openItemScreen(item))
    {
        StoredItem = item;
    }

    public override ViewCellData this[int x] 
    {
        get
        {
            return x switch
            {
                0 => new ViewCellData(StoredItem.SymbolData, StoredItem.BgColor ?? BgColor),
                1 => new ViewCellData(null, BgColor),
                _ => new ViewCellData((RenderedText[x - 2], TextColor), BgColor)
            };
        }
    }

    public override void SetWidth(int width)
    {
        base.SetWidth(width);
        RenderedText = RenderText(width - 2);
    }
}

public class InventoryMenu : TextWindow
{
    private Dictionary<ItemCategory, List<Item>> _inventory;
    private int _currentCategory;

    private readonly Player _player;

    public InventoryMenu(Player player)
    {
        _player = player;
        _inventory = _player.Inventory;
        LineView = new LineView();
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
        List<Item> items = _inventory[category];
        if (!items.Any())
        {
            LineView.SetLines(new List<ListLine> {new TextLine("Nothing here yet")});
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

        List<ListLine> lines = itemLines.Select(itemLine => itemLine.Stackable
                ? new ItemButton(itemLine, OpenItemDialog, stackableItemCounts[itemLine.Id])
                : new ItemButton(itemLine, OpenItemDialog))
            .Cast<ListLine>()
            .ToList();
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
                _player.Drop(item);
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