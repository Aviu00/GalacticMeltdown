namespace GalacticMeltdown;
using System;
using System.Collections.Generic;
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
        IncreaseViewRange, //for fov testing
        ReduceViewRange, //for fov testing
        ActivateNoClip, //temporary cheat codes
        ActivateXRay, //temporary cheat codes
        Quit
    }
    
    // Inventory bindings supposed to be updated in the future
    private static readonly Dictionary<PlayerAction, Action> PlayerActions = 
        new()
        {
            {PlayerAction.MoveUp, () => MoveControlled(0, 1)},
            {PlayerAction.MoveDown, () => MoveControlled(0, -1)},
            {PlayerAction.MoveRight, () => MoveControlled(1, 0)},
            {PlayerAction.MoveLeft, () => MoveControlled(-1, 0)},
            {PlayerAction.MoveNe, () => MoveControlled(1, 1)},
            {PlayerAction.MoveSe, () => MoveControlled(1, -1)},
            {PlayerAction.MoveSw, () => MoveControlled(-1, -1)},
            {PlayerAction.MoveNw, () => MoveControlled(-1, 1)},
            {PlayerAction.IncreaseViewRange, () => _player.ViewRadius++},
            {PlayerAction.ReduceViewRange, () => _player.ViewRadius--},
            {PlayerAction.ActivateNoClip, () => _player.NoClip = !_player.NoClip},
            {PlayerAction.ActivateXRay, () => _player.Xray = !_player.Xray},
            {PlayerAction.Quit, StopSession},
            //{PlayerAction.OpenCloseInventory, () => ChangeBindings()}
        };
}