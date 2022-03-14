using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.InputProcessing;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    // Inventory bindings supposed to be updated in the future
    private static readonly Dictionary<MainControl, Action> PlayerActions = new()
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
        //{PlayerAction.OpenCloseInventory, () => ChangeBindings()}
    };
}