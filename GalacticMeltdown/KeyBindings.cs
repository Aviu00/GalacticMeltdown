namespace GalacticMeltdown;
using System;
using System.Collections;
using System.Collections.Generic;
partial class GameManager
{
    static void ChangeBindings(IDictionary ChangeTarget, IDictionary WhatToChange) // method to change binding when open/close inventory ot etc.
    {
        ChangeTarget = WhatToChange;
    }

    private enum PlayerAction
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
        Stop
    }
    
    // Inventory bindings supposed to be updated in the future
    static private readonly IDictionary<PlayerAction, Action> InventoryBinding =
        new Dictionary<PlayerAction, Action>() { };
    static private readonly IDictionary<PlayerAction, Action> ActionBinding = 
        new Dictionary<PlayerAction, Action>
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
            //{PlayerAction.OpenCloseInventory, () => ChangeBindings()}
        };
    
    static private readonly  IDictionary <ConsoleKey, PlayerAction> KeyBinding = 
        new Dictionary<ConsoleKey, PlayerAction>
        {
            {ConsoleKey.UpArrow, PlayerAction.MoveUp},
            {ConsoleKey.DownArrow, PlayerAction.MoveDown},
            {ConsoleKey.LeftArrow, PlayerAction.MoveLeft},
            {ConsoleKey.RightArrow, PlayerAction.MoveRight},
            {ConsoleKey.D8, PlayerAction.MoveUp},
            {ConsoleKey.D9, PlayerAction.MoveNe},
            {ConsoleKey.D6, PlayerAction.MoveRight},
            {ConsoleKey.D3, PlayerAction.MoveSe},
            {ConsoleKey.D2, PlayerAction.MoveDown},
            {ConsoleKey.D1, PlayerAction.MoveSw},
            {ConsoleKey.D4, PlayerAction.MoveLeft},
            {ConsoleKey.D7, PlayerAction.MoveNw},
            {ConsoleKey.Multiply, PlayerAction.IncreaseViewRange},
            {ConsoleKey.Subtract, PlayerAction.ReduceViewRange},
            {ConsoleKey.Z, PlayerAction.ActivateNoClip},
            {ConsoleKey.X, PlayerAction.ActivateXRay},
            //{ConsoleKey.Y, PlayerAction.OpenCloseInventory}
        };
}