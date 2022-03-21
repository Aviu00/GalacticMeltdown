using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Cursor;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.TextWindows;
using GalacticMeltdown.Utility;

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
                cursor.Action = (_, _, x, y) =>
                {
                    List<Item> items = _level.GetItems(x, y);
                    if (items is null || items.Count == 0) return;
                    if (items.Count == 1)
                    {
                        PickUp(items[0]);
                    }
                    else
                    {
                        ItemPickupDialog dialog = new(items, PickUp);
                        UserInterface.AddChild(this, dialog);
                        dialog.Open();
                    }
                    
                    void PickUp(Item item)
                    {
                        items.Remove(item);
                        _player.AddToInventory(item);
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
                cursor.Action = (_, _, x, y) =>
                {
                    cursor.Close();
                    if(_level.InteractWithDoor(x, y, _player)) UserInterface.YieldControl(this);
                };
                UserInterface.AddChild(this, cursor);
                cursor.Start();
            }
        },
        {MainControl.IncreaseViewRange, () => _player.ViewRange++},
        {MainControl.ReduceViewRange, () => _player.ViewRange--},
        {MainControl.ToggleNoClip, () => _player.NoClip = !_player.NoClip},
        {MainControl.ToggleXRay, () => _player.Xray = !_player.Xray},
        {MainControl.OpenPauseMenu, OpenPauseMenu},
    };

    _cursorActions = new Dictionary<CursorControl, Action>
    {
        {CursorControl.MoveUp, () => MoveControlled(0, 1)},
        {CursorControl.MoveDown, () => MoveControlled(0, -1)},
        {CursorControl.MoveRight, () => MoveControlled(1, 0)},
        {CursorControl.MoveLeft, () => MoveControlled(-1, 0)},
        {CursorControl.MoveNe, () => MoveControlled(1, 1)},
        {CursorControl.MoveSe, () => MoveControlled(1, -1)},
        {CursorControl.MoveSw, () => MoveControlled(-1, -1)},
        {CursorControl.MoveNw, () => MoveControlled(-1, 1)},
        {CursorControl.Interact, () => ((Cursor) _controlledObject).Interact()},
        {CursorControl.Back, () =>
            {
                _controlledObject = _player;
                _levelView.SetFocus(_player);
                _levelView.RemoveCursor();
                UserInterface.SetController(this, new ActionHandler(
                    UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, _mainActions)));
            }
        },
        {CursorControl.ToggleLine, () => { _levelView.DrawCursorLine = !_levelView.DrawCursorLine;}},
        {
            CursorControl.ToggleFocus,
            () => _levelView.SetFocus(((Cursor) _controlledObject).InFocus ? _player : _levelView.Cursor)
        }
    };
    }
}