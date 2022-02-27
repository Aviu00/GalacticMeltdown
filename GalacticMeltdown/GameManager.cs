using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

partial class GameManager
{
    private static Player _player;
    private static IControllable _controlledObject;
    private static Renderer _renderer;
    private static Map _map;
    private static WorldView _worldView;

    private static bool _stop;

    static void Main(string[] args)
    {
        var map = GenerateMap(args.Length > 0 ? args[0] : null);
        _player = _map.Player;
        _controlledObject = _player;
        _renderer = new Renderer();
        _worldView = new WorldView(map);
        _worldView.AddTileRevealingObject(_player);
        _worldView.SetFocus(_player);
        _renderer.AddView(_worldView, 0, 0, 600, 1000);
        _renderer.AddView(new OverlayView(map), 601, 0, 1000, 1000);
        Console.CancelKeyPress += ExitEvent;
        AppDomain.CurrentDomain.ProcessExit += ExitEvent;
        // AppDomain.CurrentDomain.UnhandledException += ExitEvent; // Actually no, it should crash
        _renderer.Redraw();
        Dictionary<ConsoleKey, Action> Bindings = new();
        foreach (var (key, action) in KeyBinding)
        {
            Bindings.Add(key, ActionBinding[action]);
        }
        Bindings.Add(ConsoleKey.Q, InputProcessor.StopProcessLoop);
        InputProcessor.StartProcessLoop();
    }

    private static void GameLoop()
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
        if (_controlledObject.TryMove(deltaX, deltaY))  // Temporary, keeps screen up to date
        {
            _renderer.Redraw();  // TODO: Redraw should happen after a MoveMade event instead
        }
    }

    private static void ExitEvent(object sender, EventArgs e)
    {
        Stop();
    }

    private static void Stop()
    {
        _stop = true;
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Seed: {_map.MapSeed}");
        Console.WriteLine($"X: {_player.X} Y: {_player.Y}");
        Console.WriteLine(_map.MapString);
    }

    private static Map GenerateMap(string seed = null)
    {
        if (seed == null || !int.TryParse(seed, out int mapSeed) || mapSeed < 0)
            mapSeed = Random.Shared.Next(0, 1000000000);
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(mapSeed, tileTypes, rooms);
        _map = mapGen.Generate();
        rooms = null;
        tileTypes = null;
        mapGen = null;
        GC.Collect();
        return _map;
    }
}
