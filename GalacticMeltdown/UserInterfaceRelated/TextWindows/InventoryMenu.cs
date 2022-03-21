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
    public Item Item { get; }

    public ItemButton(Item item, Action<Item> openItemScreen) : base(item.Name, "", () => openItemScreen(item))
    {
        Item = item;
    }
}

public class InventoryMenu : TextWindow
{
    private const string Drop = "Drop";
    private const string Equip = "Equip";
    private const string Consume = "Consume";
    
    private Dictionary<ItemCategory, List<Item>> _inventory;
    private int _currentCategory;

    private readonly Player _player;

    public InventoryMenu(Dictionary<ItemCategory, List<Item>> inventory, Player player)
    {
        _player = player;
        _inventory = inventory;
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
        LineView.SetLines(items.Select(item => new ItemButton(item, OpenItemDialog)).Cast<ListLine>().ToList());
    }

    private void OpenItemDialog(Item item)
    {
        ItemDialog itemDialog = new(item, choice => ProcessChoice(item, choice));
        UserInterface.AddChild(this, itemDialog);
        itemDialog.Open();
    }

    private void ProcessChoice(Item item, string choice)
    {
        switch (choice)
        {
            case Drop:
                _player.Drop(item);
                break;
            case Equip:
                _player.Equip((EquippableItem) item);
                break;
            case Consume:
                _player.Consume((ConsumableItem) item);
                break;
        }
    }

    private void OpenPreviousCategory()
    {
        _currentCategory -= 1;
        if (_currentCategory == -1) _currentCategory = Enum.GetValues<ItemCategory>().Length - 1;
        LoadCategoryScreen(Enum.GetValues<ItemCategory>()[_currentCategory]);
    }
    
    private void OpenNextCategory()
    {
        _currentCategory = (_currentCategory + 1) % Enum.GetValues<ItemCategory>().Length;
        LoadCategoryScreen(Enum.GetValues<ItemCategory>()[_currentCategory]);
    }
}