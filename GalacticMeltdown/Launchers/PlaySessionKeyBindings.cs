using System;
using System.Collections.Generic;
using GalacticMeltdown.InputProcessing;
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
        {CursorControl.Back, () => {InputProcessor.RemoveLastBinding(); _levelView.RemoveCursor();}},
        {CursorControl.ToggleLine, () => { _levelView.DrawCursorLine = !_levelView.DrawCursorLine;}}
    };
}