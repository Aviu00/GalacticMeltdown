using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Cursor;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.TextWindows.ItemManagement;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    private void SetControlDicts()
    {
        _mainActions = new Dictionary<MainControl, Action>
        {
        {MainControl.MoveUp, () => MoveControlled(0, 1)},
        {MainControl.MoveDown, () => MoveControlled(0, -1)},
        {MainControl.MoveRight, () => MoveControlled(1, 0)},
        {MainControl.MoveLeft, () => MoveControlled(-1, 0)},
        {MainControl.MoveNe, () => MoveControlled(1, 1)},
        {MainControl.MoveSe, () => MoveControlled(1, -1)},
        {MainControl.MoveSw, () => MoveControlled(-1, -1)},
        {MainControl.MoveNw, () => MoveControlled(-1, 1)},
        {MainControl.StopTurn, () => {_player.StopTurn(); UserInterface.YieldControl(this);}},
        {MainControl.DoNothing, () => UserInterface.YieldControl(this)},
        {
            MainControl.PickUpItem, () =>
            {
                Cursor cursor = _levelView.Cursor;
                cursor.LevelBounds = (_player.X - 1, _player.Y - 1, _player.X + 1, _player.Y + 1);
                cursor.Action = (x, y) =>
                {
                    List<Item> items = _level.GetItems(x, y);
                    if (items is null || items.Count == 0) return;
                    ItemPickupDialog dialog = new(items, PickUp);
                    UserInterface.AddChild(this, dialog);
                    dialog.Open();
                    
                    void PickUp(Item item)
                    {
                        if (item.Stackable)
                        {
                            foreach (Item listItem in items.FindAll(listItem => listItem.Id == item.Id).ToList())
                            {
                                items.Remove(listItem);
                                _player.AddToInventory(item);
                            }
                        }
                        else
                        {
                            items.Remove(item);
                            _player.AddToInventory(item);
                        }
                    }
                };
                UserInterface.AddChild(this, cursor);
                cursor.Start();
            }
        },
        {
            MainControl.UseCursor, () =>
            {
                Cursor cursor = _levelView.Cursor;
                UserInterface.AddChild(this, cursor);
                cursor.Start();
            }
        },
        {
            MainControl.InteractWithDoors, () =>
            {
                Cursor cursor = _levelView.Cursor;
                cursor.LevelBounds = (_player.X - 1, _player.Y - 1, _player.X + 1, _player.Y + 1);
                cursor.Action = (x, y) =>
                {
                    cursor.Close();
                    if (_level.InteractWithDoor(x, y, _player)) UserInterface.YieldControl(this);
                };
                UserInterface.AddChild(this, cursor);
                cursor.Start();
            }
        },
        {
            MainControl.Shoot, () =>
            {
                if (_player.Equipment[BodyPart.Hands] is not RangedWeaponItem || _player.ChosenAmmoId is null) return;
                Cursor cursor = _levelView.Cursor;
                _levelView.ToggleCursorLine();
                cursor.Action = (x, y) =>
                {
                    cursor.Close();
                    if (_player.Shoot(x, y))
                        UserInterface.YieldControl(this);
                };
                UserInterface.AddChild(this, cursor);
                cursor.Start();
            }
        },
        {MainControl.IncreaseViewRange, () =>
            {
                if (_cheatsEnabled) _player.ViewRange++;
            }
        },
        {MainControl.ReduceViewRange, () =>
            {
                if (_cheatsEnabled) _player.ViewRange--;
            }
        },
        {MainControl.OpenPauseMenu, OpenPauseMenu},
        {MainControl.OpenInventory, OpenInventory},
        {MainControl.OpenAmmoSelection, OpenAmmoSelectionDialog},
        {MainControl.OpenConsole, OpenConsole},
        };
    }
}