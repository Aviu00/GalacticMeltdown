using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Data;

public static class DataHolder
{
    static DataHolder()
    {
        
    }

    public struct CurrentBindings
    {
        public static Dictionary<ConsoleKey, PlaySession.PlayerAction> Player = new()
        {
            {ConsoleKey.UpArrow, PlaySession.PlayerAction.MoveUp},
            {ConsoleKey.DownArrow, PlaySession.PlayerAction.MoveDown},
            {ConsoleKey.LeftArrow, PlaySession.PlayerAction.MoveLeft},
            {ConsoleKey.RightArrow, PlaySession.PlayerAction.MoveRight},
            {ConsoleKey.D8, PlaySession.PlayerAction.MoveUp},
            {ConsoleKey.D9, PlaySession.PlayerAction.MoveNe},
            {ConsoleKey.D6, PlaySession.PlayerAction.MoveRight},
            {ConsoleKey.D3, PlaySession.PlayerAction.MoveSe},
            {ConsoleKey.D2, PlaySession.PlayerAction.MoveDown},
            {ConsoleKey.D1, PlaySession.PlayerAction.MoveSw},
            {ConsoleKey.D4, PlaySession.PlayerAction.MoveLeft},
            {ConsoleKey.D7, PlaySession.PlayerAction.MoveNw},
            {ConsoleKey.Multiply, PlaySession.PlayerAction.IncreaseViewRange},
            {ConsoleKey.Subtract, PlaySession.PlayerAction.ReduceViewRange},
            {ConsoleKey.Z, PlaySession.PlayerAction.ActivateNoClip},
            {ConsoleKey.X, PlaySession.PlayerAction.ActivateXRay},
            {ConsoleKey.Q, PlaySession.PlayerAction.Quit}
        };

        public static Dictionary<ConsoleKey, SelectionControl> Selection = new()
        {
            {ConsoleKey.UpArrow, SelectionControl.Up},
            {ConsoleKey.DownArrow, SelectionControl.Down},
            {ConsoleKey.LeftArrow, SelectionControl.Left},
            {ConsoleKey.RightArrow, SelectionControl.Right},
            {ConsoleKey.Enter, SelectionControl.Select},
            {ConsoleKey.Escape, SelectionControl.Back},
            {ConsoleKey.Tab, SelectionControl.SwitchButtonGroup},
        };
    }
}