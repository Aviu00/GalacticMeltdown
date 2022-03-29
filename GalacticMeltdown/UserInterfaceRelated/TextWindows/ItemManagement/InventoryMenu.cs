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

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows.ItemManagement;

public class InventoryMenu : TextWindow
{
    private List<Item> _inventory;
    private Dictionary<InventoryFilterType, Func<Item, bool>> _inventoryFilters;

    private readonly Player _player;

    public InventoryMenu(Player player)
    {
        _player = player;
        _inventory = _player.Inventory;
        _inventoryFilters = new Dictionary<InventoryFilterType, Func<Item, bool>>();
        UpdateLines();
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.InventoryMenu,
            new Dictionary<InventoryControl, Action>
            {
                {InventoryControl.Back, Close},
                {InventoryControl.Select, LineView.PressCurrent},
                {InventoryControl.Down, LineView.SelectNext},
                {InventoryControl.Up, LineView.SelectPrev},
                {InventoryControl.OpenEquipmentMenu, OpenEquipmentMenu},
                {InventoryControl.OpenCategorySelection, OpenCategoryDialog},
            }));
    }

    private void UpdateLines()
    {
        List<Item> itemLines = new();
        Dictionary<string, int> stackableItemCounts = new();
        foreach (Item item in _inventory.Where(item => _inventoryFilters.Values.All(cond => cond(item))))
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

        if (!itemLines.Any())
        {
            LineView.SetLines(new List<ListLine> {new TextLine("No items")});
            return;
        }

        LineView.SetLines(
            itemLines.OrderBy(item => item.Name)
                .Select(itemLine =>
                    itemLine.Stackable
                        ? new ItemButton(itemLine, OpenItemDialog, stackableItemCounts[itemLine.Id])
                        : new ItemButton(itemLine, OpenItemDialog))
                .Cast<ListLine>()
                .ToList(), true);
    }

    private void OpenEquipmentMenu()
    {
        EquipmentMenu equipmentMenu = new(_player);
        UserInterface.AddChild(this, equipmentMenu);
        equipmentMenu.Open();
    }

    private void OpenItemDialog(Item item)
    {
        ItemDialog itemDialog = new(item, ProcessChoice);
        UserInterface.AddChild(this, itemDialog);
        itemDialog.Open();
        
        void ProcessChoice(ItemAction choice)
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
            UpdateLines();
        }
    }

    private void OpenCategoryDialog()
    {
        CategoryDialog dialog = new(SetCategory);
        UserInterface.AddChild(this, dialog);
        dialog.Open();

        void SetCategory(ItemCategory? category)
        {
            if (category is null) _inventoryFilters.Remove(InventoryFilterType.Category);
            else _inventoryFilters[InventoryFilterType.Category] = item => item.Category == category;
            UpdateLines();
        }
    }
}