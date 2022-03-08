using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    public enum PlayerAction
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        MoveNe,
        MoveSe,
        MoveSw,
        MoveNw,
        StopTurn,
        IncreaseViewRange, //for fov testing
        ReduceViewRange, //for fov testing
        ActivateNoClip, //temporary cheat codes
        ActivateXRay, //temporary cheat codes
        Quit
    }

    // Inventory bindings supposed to be updated in the future
    private static readonly Dictionary<PlayerAction, Action> PlayerActions = new()
    {
        {PlayerAction.MoveUp, () => PlaySession.MoveControlled(0, 1)},
        {PlayerAction.MoveDown, () => PlaySession.MoveControlled(0, -1)},
        {PlayerAction.MoveRight, () => PlaySession.MoveControlled(1, 0)},
        {PlayerAction.MoveLeft, () => PlaySession.MoveControlled(-1, 0)},
        {PlayerAction.MoveNe, () => PlaySession.MoveControlled(1, 1)},
        {PlayerAction.MoveSe, () => PlaySession.MoveControlled(1, -1)},
        {PlayerAction.MoveSw, () => PlaySession.MoveControlled(-1, -1)},
        {PlayerAction.MoveNw, () => PlaySession.MoveControlled(-1, 1)},
        {PlayerAction.IncreaseViewRange, () => PlaySession._player.ViewRange++},
        {PlayerAction.ReduceViewRange, () => PlaySession._player.ViewRange--},
        {PlayerAction.ActivateNoClip, () => PlaySession._player.NoClip = !PlaySession._player.NoClip},
        {PlayerAction.ActivateXRay, () => PlaySession._player.Xray = !PlaySession._player.Xray},
        {PlayerAction.Quit, PlaySession.StopSession},
        //{PlayerAction.OpenCloseInventory, () => ChangeBindings()}
    };
}