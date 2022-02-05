namespace GalacticMeltdown;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GalacticMeltdown.data;
static partial class GameManager // special class for bindings
{
    static void ChangeBindings(IDictionary ChangeTarget, IDictionary WhatToChange) // method to change binding when open/close inventory ot etc.
    {
        ChangeTarget = WhatToChange;
    }

    public enum ActionMove
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        IncreaseViewRange, //for fov testing
        ReduceViewRange, //for fov testing
        Stop,
        OpenCloseInventory
    }
    
    // Inventory bindings supposed to be updated in the future
    static private readonly IDictionary<ActionMove, Action> InventoryBinding =
        new Dictionary<ActionMove, Action>() { };
    static private readonly IDictionary<ActionMove, Action> ActionBinding = 
        new Dictionary<ActionMove, Action>
        {
            {ActionMove.MoveUp, () => Player.TryMove(0, 1)},
            {ActionMove.MoveDown, () => Player.TryMove(0, -1)},
            {ActionMove.MoveRight, () => Player.TryMove(1, 0)},
            {ActionMove.MoveLeft, () => Player.TryMove(-1, 0)},
            {ActionMove.IncreaseViewRange, () => Player.ViewRange++},
            {ActionMove.ReduceViewRange, () => Player.ViewRange--},
            {ActionMove.Stop, () => Stop()}
            //{ActionMove.OpenCloseInventory, () => ChangeBindings()}
        };
    
    static private readonly  IDictionary <ConsoleKey, ActionMove> KeyBinding = 
        new Dictionary<ConsoleKey, ActionMove>
        {
            {ConsoleKey.UpArrow, ActionMove.MoveUp},
            {ConsoleKey.DownArrow, ActionMove.MoveDown},
            {ConsoleKey.LeftArrow, ActionMove.MoveLeft},
            {ConsoleKey.RightArrow, ActionMove.MoveRight},
            {ConsoleKey.Multiply, ActionMove.IncreaseViewRange},
            {ConsoleKey.Subtract, ActionMove.ReduceViewRange},
            {ConsoleKey.Q, ActionMove.Stop},
            {ConsoleKey.Y, ActionMove.OpenCloseInventory}
        };
}