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

    private enum ActionMove
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
    static private readonly IDictionary<ActionMove, Action> InventoryBinding =
        new Dictionary<ActionMove, Action>() { };
    static private readonly IDictionary<ActionMove, Action> ActionBinding = 
        new Dictionary<ActionMove, Action>
        {
            {ActionMove.MoveUp, () => MoveControlled(0, 1)},
            {ActionMove.MoveDown, () => MoveControlled(0, -1)},
            {ActionMove.MoveRight, () => MoveControlled(1, 0)},
            {ActionMove.MoveLeft, () => MoveControlled(-1, 0)},
            {ActionMove.MoveNe, () => MoveControlled(1, 1)},
            {ActionMove.MoveSe, () => MoveControlled(1, -1)},
            {ActionMove.MoveSw, () => MoveControlled(-1, -1)},
            {ActionMove.MoveNw, () => MoveControlled(-1, 1)},
            {ActionMove.IncreaseViewRange, () => _player.ViewRadius++},
            {ActionMove.ReduceViewRange, () => _player.ViewRadius--},
            {ActionMove.ActivateNoClip, () => _player.NoClip = !_player.NoClip},
            {ActionMove.ActivateXRay, () => _player.Xray = !_player.Xray},
            //{ActionMove.OpenCloseInventory, () => ChangeBindings()}
        };
    
    static private readonly  IDictionary <ConsoleKey, ActionMove> KeyBinding = 
        new Dictionary<ConsoleKey, ActionMove>
        {
            {ConsoleKey.UpArrow, ActionMove.MoveUp},
            {ConsoleKey.DownArrow, ActionMove.MoveDown},
            {ConsoleKey.LeftArrow, ActionMove.MoveLeft},
            {ConsoleKey.RightArrow, ActionMove.MoveRight},
            {ConsoleKey.D8, ActionMove.MoveUp},
            {ConsoleKey.D9, ActionMove.MoveNe},
            {ConsoleKey.D6, ActionMove.MoveRight},
            {ConsoleKey.D3, ActionMove.MoveSe},
            {ConsoleKey.D2, ActionMove.MoveDown},
            {ConsoleKey.D1, ActionMove.MoveSw},
            {ConsoleKey.D4, ActionMove.MoveLeft},
            {ConsoleKey.D7, ActionMove.MoveNw},
            {ConsoleKey.Multiply, ActionMove.IncreaseViewRange},
            {ConsoleKey.Subtract, ActionMove.ReduceViewRange},
            {ConsoleKey.Z, ActionMove.ActivateNoClip},
            {ConsoleKey.X, ActionMove.ActivateXRay},
            //{ConsoleKey.Y, ActionMove.OpenCloseInventory}
        };
}