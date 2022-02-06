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
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(mapSeed, tileTypes, rooms);
        Map = mapGen.Generate();
        mapGen = null;
        GC.Collect();
    }
}