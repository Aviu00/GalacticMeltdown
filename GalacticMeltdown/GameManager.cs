using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GalacticMeltdown.data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;


static partial class GameManager
{
    private static Player _player;
    private static IControllable _controlledObject;
    private static HashSet<IControllable> _updateOnMove;
    public static Renderer Renderer;
    public static Map Map;

    private static bool _stop;

    static void Main(string[] args)
    {
        var map = GenerateMap(args.Length > 0 ? args[0] : null);
        _player = Map.Player;
        _controlledObject = _player;
        _updateOnMove = new HashSet<IControllable> { _player };
        Renderer = new Renderer(_player);
        Renderer.AddView(new WorldView(map), 0, 0, 600, 1000);
        Renderer.AddView(new OverlayView(map), 601, 0, 1000, 1000);
        Console.CancelKeyPress += ExitEvent;
        AppDomain.CurrentDomain.ProcessExit += ExitEvent;
        // AppDomain.CurrentDomain.UnhandledException += ExitEvent; // Actually no, it should crash
        Renderer.Redraw();
        GameLoop();
    }
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
            Renderer.Redraw();  // TODO: Redraw should happen after a MoveMade event instead
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

    static Map GenerateMap(string seed = null)
    {
        if (seed == null || !int.TryParse(seed, out int mapSeed) || mapSeed < 0)
            mapSeed = Random.Shared.Next(0, 1000000000);
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(mapSeed, tileTypes, rooms);
        Map = mapGen.Generate();
        rooms = null;
        tileTypes = null;
        mapGen = null;
        GC.Collect();
        return Map;
    }
}
