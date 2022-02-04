using System;
using System.Collections.Generic;
using GalacticMeltdown.data;

namespace GalacticMeltdown;

static class GameManager
{
    public static Player Player;
    public static Renderer Renderer;
    public static Map Map;
    public static Dictionary<string, TileTypeData> TileTypes;
    private static List<(int rarity, int exitCount, RoomData.Room room)> _rooms;

    private static bool _stop;

    static void Main(string[] args)
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        _rooms = new RoomData(TileTypes).Rooms;
        GenerateMap(args.Length > 0 ? args[0] : null);
        Player = new Player(Map.StartPoint.MapX * 25 + 12, Map.StartPoint.MapY * 25 + 12, Map.GetTile);
        Renderer = new Renderer(Player);
        Console.CancelKeyPress += ExitEvent;
        AppDomain.CurrentDomain.ProcessExit += ExitEvent;
        AppDomain.CurrentDomain.UnhandledException += ExitEvent;
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
        {ActionMove.MoveUp, () => MovePlayer(0, 1)},
        {ActionMove.MoveDown, () => MovePlayer(0, -1)},
        {ActionMove.MoveRight, () => MovePlayer(1, 0)},
        {ActionMove.MoveLeft, () => MovePlayer(-1, 0)},
        {ActionMove.MoveNe, () => MovePlayer(1, 1)},
        {ActionMove.MoveSe, () => MovePlayer(1, -1)},
        {ActionMove.MoveSw, () => MovePlayer(-1, -1)},
        {ActionMove.MoveNw, () => MovePlayer(-1, 1)},
        {ActionMove.IncreaseViewRange, () => Player.ViewRange++},
        {ActionMove.ReduceViewRange, () => Player.ViewRange--},
        {ActionMove.ActivateNoClip, () => Player.NoClip = !Player.NoClip},
        {ActionMove.ActivateXRay, () => Player.Xray = !Player.Xray},
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
    
    private static void MovePlayer(int deltaX, int deltaY)
    {
        if (Player.TryMove(deltaX, deltaY))
        {
            Renderer.RedrawMap();
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
    }

    static void GenerateMap(string seed = null)
    {
        if (seed == null || !int.TryParse(seed, out int mapSeed) || mapSeed < 0)
            mapSeed = Random.Shared.Next(0, 1000000000);
        MapGenerator mapGen = new MapGenerator(mapSeed, TileTypes, _rooms);
        Map = mapGen.Generate();
        mapGen = null;
        GC.Collect();
    }
}