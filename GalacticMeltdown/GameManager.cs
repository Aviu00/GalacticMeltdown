using System;
using System.Collections.Generic;
using GalacticMeltdown.data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

static class GameManager
{
    private static Player _player;
    private static IControllable _controlledObject;
    private static HashSet<IControllable> _updateOnMove;
    public static Renderer Renderer;
    public static Map Map;

    private static bool _stop;

    static void Main(string[] args)
    {
        GenerateMap(args.Length > 0 ? args[0] : null);
        _player = Map.Player;
        _controlledObject = _player;
        _updateOnMove = new HashSet<IControllable> { _player };
        Renderer = new Renderer(_player);
        Console.CancelKeyPress += ExitEvent;
        AppDomain.CurrentDomain.ProcessExit += ExitEvent;
        // AppDomain.CurrentDomain.UnhandledException += ExitEvent; // Actually no, it should crash
        Renderer.RedrawMap();
        GameLoop();
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
    private static readonly  IDictionary <ConsoleKey, ActionMove> KeyBinding = 
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
            {ConsoleKey.Q, ActionMove.Stop}
        };

    private static readonly IDictionary<ActionMove, Action> ActionBinding = 
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
        {ActionMove.IncreaseViewRange, () => _player.ViewRange++},
        {ActionMove.ReduceViewRange, () => _player.ViewRange--},
        {ActionMove.ActivateNoClip, () => _player.NoClip = !_player.NoClip},
        {ActionMove.ActivateXRay, () => _player.Xray = !_player.Xray},
        {ActionMove.Stop, Stop}
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
    
    private static void MoveControlled(int deltaX, int deltaY)
    {
        if (_updateOnMove.Contains(_controlledObject) && _controlledObject.TryMove(deltaX, deltaY))
        {
            Renderer.RedrawMap();  // TODO: Redraw should happen after a MoveMade event instead
            Console.SetCursorPosition(0, 1);
            Renderer.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
            Console.WriteLine($"X: {_controlledObject.X} Y: {_controlledObject.Y}");
        }
    }

    private static void ExitEvent(object sender, EventArgs e)
    {
        Stop();
    }

    static void Stop()
    {
        _stop = true;
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Seed: {Map.MapSeed}");
        Console.WriteLine($"X: {_player.X} Y: {_player.Y}");
        Console.WriteLine(Map.MapString);
    }

    static void GenerateMap(string seed = null)
    {
        if (seed == null || !int.TryParse(seed, out int mapSeed) || mapSeed < 0)
            mapSeed = Random.Shared.Next(0, 1000000000);
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(mapSeed, tileTypes, rooms);
        Map = mapGen.Generate();
        mapGen = null;
        GC.Collect();
    }
}
