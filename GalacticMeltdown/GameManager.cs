﻿using System;
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

    static void Main(string[] args)
    {
        if (args.Length == 0 || int.TryParse(args[0], out _mapSeed) && _mapSeed < 0)
            _mapSeed = Random.Shared.Next(0, 1000000000);
        else if (_mapSeed < 0)
            _mapSeed = args[0].GetHashCode();
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
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    Player.TryMove(0, 1);
                    break;
                case ConsoleKey.DownArrow:
                    Player.TryMove(0, -1);
                    break;
                case ConsoleKey.RightArrow:
                    Player.TryMove(1, 0);
                    break;
                case ConsoleKey.LeftArrow:
                    Player.TryMove(-1, 0);
                    break;
                case ConsoleKey.Multiply: //for fov testing
                    Player.ViewRange++;
                    break;
                case ConsoleKey.Subtract:
                    Player.ViewRange--;
                    break;
                case ConsoleKey.X:
                    Player.Xray = !Player.Xray;
                    break;
                case ConsoleKey.Z:
                    Player.NoClip = !Player.NoClip;
                    break;
                case ConsoleKey.Q:
                    Stop();
                    return;
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