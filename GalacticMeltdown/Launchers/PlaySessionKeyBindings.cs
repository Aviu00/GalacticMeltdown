using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    private static readonly Dictionary<MainControl, Action> MainActions = new()
    {
        {MainControl.MoveUp, () => MoveControlled(0, 1)},
        {MainControl.MoveDown, () => MoveControlled(0, -1)},
        {MainControl.MoveRight, () => MoveControlled(1, 0)},
        {MainControl.MoveLeft, () => MoveControlled(-1, 0)},
        {MainControl.MoveNe, () => MoveControlled(1, 1)},
        {MainControl.MoveSe, () => MoveControlled(1, -1)},
        {MainControl.MoveSw, () => MoveControlled(-1, -1)},
        {MainControl.MoveNw, () => MoveControlled(-1, 1)},
        {MainControl.StopTurn, () => {_player.StopTurn(); GiveBackControl();}},
        {MainControl.DoNothing, GiveBackControl},
        {
            MainControl.UseCursor, () =>
            {
                _controlledObject = _levelView.Cursor;
                InputProcessor.AddBinding(DataHolder.CurrentBindings.Cursor, CursorActions);
            }
        },
        {
            MainControl.InteractWithDoors, () =>
            {
                _controlledObject = _levelView.Cursor;
                _levelView.Cursor.LevelBounds = (_player.X - 1, _player.Y - 1, _player.X + 1, _player.Y + 1);
                _levelView.Cursor.Action = (_, _, x, y) =>
                {
                    _controlledObject = _player;
                    _levelView.SetFocus(_player);
                    _levelView.RemoveCursor();
                    if (_level.InteractWithDoor(x, y, _player))
                        GiveBackControl();
                    else
                        InputProcessor.RemoveLastBinding();
                };  
                InputProcessor.AddBinding(DataHolder.CurrentBindings.Cursor, CursorActions);
            }
        },
        {MainControl.IncreaseViewRange, () => _player.ViewRange++},
        {MainControl.ReduceViewRange, () => _player.ViewRange--},
        {MainControl.ToggleNoClip, () => _player.NoClip = !_player.NoClip},
        {MainControl.ToggleXRay, () => _player.Xray = !_player.Xray},
        {MainControl.Quit, StopSession},
    };

    private static readonly Dictionary<CursorControl, Action> CursorActions = new()
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
                InputProcessor.RemoveLastBinding();
                _controlledObject = _player;
                _levelView.SetFocus(_player);
                _levelView.RemoveCursor();
            }
        },
        {CursorControl.ToggleLine, () => { _levelView.DrawCursorLine = !_levelView.DrawCursorLine;}},
        {
            CursorControl.ToggleFocus,
            () => _levelView.SetFocus(((Cursor) _controlledObject).InFocus ? _player : _levelView.Cursor)
        }
    };
}