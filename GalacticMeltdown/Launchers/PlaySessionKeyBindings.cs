using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    private void SetControlDicts()
    {
        MainActions = new Dictionary<MainControl, Action>
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
            MainControl.UseCursor, () =>
            {
                _controlledObject = _levelView.Cursor;
                UserInterface.SetController(this, new ActionHandler(
                    UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Cursor, CursorActions)));
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
                    UserInterface.SetController(this, new ActionHandler(
                        UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, MainActions)));
                    if(_level.InteractWithDoor(x, y, _player)) UserInterface.YieldControl(this);
                };  
                UserInterface.SetController(this, new ActionHandler(
                    UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Cursor, CursorActions)));
            }
        },
        {MainControl.IncreaseViewRange, () => _player.ViewRange++},
        {MainControl.ReduceViewRange, () => _player.ViewRange--},
        {MainControl.ToggleNoClip, () => _player.NoClip = !_player.NoClip},
        {MainControl.ToggleXRay, () => _player.Xray = !_player.Xray},
        {MainControl.Quit, StopSession},
    };

    CursorActions = new Dictionary<CursorControl, Action>
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
                    UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, MainActions)));
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