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
    private List<Item> _inventory;
    private List<Func<Item, bool>> _conditions;

    private readonly Player _player;

    public InventoryMenu(Player player)
    {
        _player = player;
        _inventory = _player.Inventory;
        UpdateLines();
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.InventoryMenu,
            new Dictionary<InventoryControl, Action>
            {
                {InventoryControl.Back, Close},
                {InventoryControl.Select, LineView.PressCurrent},
                {InventoryControl.Down, LineView.SelectNext},
                {InventoryControl.Up, LineView.SelectPrev},
                {InventoryControl.Left, OpenPreviousCategory},
                {InventoryControl.Right, OpenNextCategory},
                {InventoryControl.OpenEquipmentMenu, OpenEquipmentMenu}
            }));
    }

    private void UpdateLines()
    {
        List<ListLine> lines = new();
        if (!_inventory.Any())
        {
            lines.Add(new TextLine("No items"));
            LineView.SetLines(lines);
            return;
        }

        List<Item> itemLines = new();
        Dictionary<string, int> stackableItemCounts = new();
        foreach (Item item in _inventory.Where(item => _conditions.All(cond => cond(item))))
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

        lines.AddRange(itemLines.OrderBy(item => item.Name)
            .Select(itemLine =>
                itemLine.Stackable
                    ? new ItemButton(itemLine, OpenItemDialog, stackableItemCounts[itemLine.Id])
                    : new ItemButton(itemLine, OpenItemDialog))
            .Cast<ListLine>()
            .ToList());
        LineView.SetLines(lines, true);
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

    private void OpenPreviousCategory()
    {
        UpdateLines();
    }
    
    private void OpenNextCategory()
    {
        UpdateLines();
    }
}