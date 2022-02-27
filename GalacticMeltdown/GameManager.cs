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
        Dictionary<ConsoleKey, Action> bindings = new();
        foreach (var (key, action) in KeyBinding)
        {
            bindings.Add(key, ActionBinding[action]);
        }
        bindings.Add(ConsoleKey.Q, InputProcessor.StopProcessLoop);
        InputProcessor.Bindings = bindings;
        InputProcessor.StartProcessLoop();
        CleanUp();
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
        CleanUp();
    }

    private static void CleanUp()
    {
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
