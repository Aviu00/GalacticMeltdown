using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Input;
using GalacticMeltdown.data;

namespace GalacticMeltdown;

static class GameManager
{
    public static Player Player;
    public static ConsoleManager ConsoleManager;
    public static Map Map;
    public static TileTypesExtractor TileTypesExtractor;
    public static RoomData RoomData;

    private static bool _stop;

    private static int _mapSeed = -1;

    static void Main()
    {
        if (_mapSeed < 0)
            _mapSeed = Random.Shared.Next(0, 1000000);
        TileTypesExtractor = new TileTypesExtractor();
        RoomData = new RoomData();
        MapGenerator mapGen = new MapGenerator(_mapSeed);
        Map = mapGen.Generate();
        Player = new Player();
        ConsoleManager = new ConsoleManager();
        Console.CancelKeyPress += ConsoleCancelEvent;
        Player.ResetVisibleObjects();
        GameLoop();
    }

    public enum ActionMove
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        IncreaseViewRange, //for fov testing
        ReduceViewRange, //for fov testing
        Stop
    }
    static private readonly  IDictionary <ConsoleKey, ActionMove> KeyBinding = 
        new Dictionary<ConsoleKey, ActionMove>
        {
            {ConsoleKey.UpArrow, ActionMove.MoveUp},
            {ConsoleKey.DownArrow, ActionMove.MoveDown},
            {ConsoleKey.LeftArrow, ActionMove.MoveLeft},
            {ConsoleKey.RightArrow, ActionMove.MoveRight},
            {ConsoleKey.Multiply, ActionMove.IncreaseViewRange},
            {ConsoleKey.Subtract, ActionMove.ReduceViewRange},
            {ConsoleKey.Q, ActionMove.Stop}
        };

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
    };

    static void GameLoop()
    {
       
        while (!_stop)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (KeyBinding.ContainsKey(key.Key))
            {
                ActionBinding[KeyBinding[key.Key]].Invoke();
            }
            while (Console.KeyAvailable) //clear console key buffer
            {
                Console.ReadKey(true);
            }
        }
    }

    private static void ConsoleCancelEvent(object sender, ConsoleCancelEventArgs e)
    {
        Stop();
    }

    static void Stop()
    {
        _stop = true;
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
    }
}