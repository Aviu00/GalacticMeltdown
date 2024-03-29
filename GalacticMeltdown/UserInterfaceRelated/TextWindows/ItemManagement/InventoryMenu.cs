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
    private Dictionary<InventoryFilterType, Func<Item, bool>> _inventoryFilters;

    private readonly Player _player;

    public InventoryMenu(Player player)
    {
        _player = player;
        _player.Inventory.CollectionChanged += OnInventoryChange;
        _inventoryFilters = new Dictionary<InventoryFilterType, Func<Item, bool>>();
        UpdateLines();
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(KeyBindings.InventoryMenu,
            new Dictionary<InventoryControl, Action>
            {
                {InventoryControl.Back, Close},
                {InventoryControl.Select, LineView.PressCurrent},
                {InventoryControl.Down, LineView.SelectNext},
                {InventoryControl.Up, LineView.SelectPrev},
                {InventoryControl.OpenEquipmentMenu, OpenEquipmentMenu},
                {InventoryControl.OpenCategorySelection, OpenCategoryDialog},
                {InventoryControl.OpenSearchBox, OpenSearchBox}
            }));
    }

    public override void Close()
    {
        _player.Inventory.CollectionChanged -= OnInventoryChange;
        base.Close();
    }

    private void UpdateLines()
    {
        List<Item> itemLines = new();
        Dictionary<string, int> stackableItemCounts = new();
        foreach (Item item in _player.Inventory.Where(item => _inventoryFilters.Values.All(cond => cond(item))))
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
                case ItemAction.Info:
                        InfoWindow infoWindow = new(item.GetDescription());
                        UserInterface.AddChild(this, infoWindow);
                        infoWindow.Open();
                    break;
            }
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

    private void OpenSearchBox()
    {
        InputBox searchBox = new(SetTextFilter);
        UserInterface.AddChild(this, searchBox);
        searchBox.Open();

        void SetTextFilter(string query)
        {
            if (query == "")
                _inventoryFilters.Remove(InventoryFilterType.Search);
            else
                _inventoryFilters[InventoryFilterType.Search] = item =>
                    item.Name.ToLower().Contains(query.ToLower());
            UpdateLines();
        }
    }

    private void OnInventoryChange(object sender, EventArgs e) => UpdateLines();
}