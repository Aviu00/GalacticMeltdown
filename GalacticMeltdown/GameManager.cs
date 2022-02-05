using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GalacticMeltdown.data;

namespace GalacticMeltdown;


static partial class GameManager
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