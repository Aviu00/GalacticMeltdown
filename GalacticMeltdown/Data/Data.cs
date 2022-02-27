using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Data;

public static class Data
{
    static Data()
    {
        
    }

    public struct CurrentBindings
    {
        public static Dictionary<ConsoleKey, GameManager.PlayerAction> Player = new()
        {
            {ConsoleKey.UpArrow, GameManager.PlayerAction.MoveUp},
            {ConsoleKey.DownArrow, GameManager.PlayerAction.MoveDown},
            {ConsoleKey.LeftArrow, GameManager.PlayerAction.MoveLeft},
            {ConsoleKey.RightArrow, GameManager.PlayerAction.MoveRight},
            {ConsoleKey.D8, GameManager.PlayerAction.MoveUp},
            {ConsoleKey.D9, GameManager.PlayerAction.MoveNe},
            {ConsoleKey.D6, GameManager.PlayerAction.MoveRight},
            {ConsoleKey.D3, GameManager.PlayerAction.MoveSe},
            {ConsoleKey.D2, GameManager.PlayerAction.MoveDown},
            {ConsoleKey.D1, GameManager.PlayerAction.MoveSw},
            {ConsoleKey.D4, GameManager.PlayerAction.MoveLeft},
            {ConsoleKey.D7, GameManager.PlayerAction.MoveNw},
            {ConsoleKey.Multiply, GameManager.PlayerAction.IncreaseViewRange},
            {ConsoleKey.Subtract, GameManager.PlayerAction.ReduceViewRange},
            {ConsoleKey.Z, GameManager.PlayerAction.ActivateNoClip},
            {ConsoleKey.X, GameManager.PlayerAction.ActivateXRay},
            {ConsoleKey.Q, GameManager.PlayerAction.Quit}
        };
    }
}