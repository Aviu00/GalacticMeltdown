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

    public ItemButton(Item item, Action<Item> openItemScreen) : base(item.Name, "", () => openItemScreen(item))
    {
        StoredItem = item;
    }

    public override ViewCellData this[int x] 
    {
        get
        {
            if (x == 0) return new ViewCellData(StoredItem.SymbolData, StoredItem.BgColor ?? BgColor);
            if (x == 1) return new ViewCellData(null, BgColor);
            return new ViewCellData((RenderedText[x - 2], TextColor), BgColor);
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
        LineView.SetLines(items.Select(item => new ItemButton(item, OpenItemDialog)).Cast<ListLine>().ToList());
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