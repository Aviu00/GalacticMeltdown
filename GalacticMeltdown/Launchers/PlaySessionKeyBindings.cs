using System;
using System.Collections.Generic;
using GalacticMeltdown.InputProcessing;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    // Inventory bindings supposed to be updated in the future
    private static readonly Dictionary<PlayerControl, Action> PlayerActions = new()
    {
        {PlayerControl.MoveUp, () => MoveControlled(0, 1)},
        {PlayerControl.MoveDown, () => MoveControlled(0, -1)},
        {PlayerControl.MoveRight, () => MoveControlled(1, 0)},
        {PlayerControl.MoveLeft, () => MoveControlled(-1, 0)},
        {PlayerControl.MoveNe, () => MoveControlled(1, 1)},
        {PlayerControl.MoveSe, () => MoveControlled(1, -1)},
        {PlayerControl.MoveSw, () => MoveControlled(-1, -1)},
        {PlayerControl.MoveNw, () => MoveControlled(-1, 1)},
        {PlayerControl.IncreaseViewRange, () => _player.ViewRange++},
        {PlayerControl.ReduceViewRange, () => _player.ViewRange--},
        {PlayerControl.ToggleNoClip, () => _player.NoClip = !_player.NoClip},
        {PlayerControl.ToggleXRay, () => _player.Xray = !_player.Xray},
        {PlayerControl.Quit, StopSession},
        //{PlayerAction.OpenCloseInventory, () => ChangeBindings()}
    };
}