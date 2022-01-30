using System;
using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    static class GameManager
    {
        public static Player Player;
        public static ConsoleManager ConsoleManager;
        public static Map Map;
        public static TileTypesExtractor TileTypesExtractor;
        
        private static bool _stop;
        
        static void Main()
        {
            TileTypesExtractor = new TileTypesExtractor();
            Map = new Map();
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
                        Player.Move(0,1);
                        break;
                    case ConsoleKey.DownArrow:
                        Player.Move(0,-1);
                        break;
                    case ConsoleKey.RightArrow:
                        Player.Move(1,0);
                        break;
                    case ConsoleKey.LeftArrow:
                        Player.Move(-1,0);
                        break;
                    case ConsoleKey.Multiply://for fov testing
                        Player.ViewRange++;
                        break;
                    case ConsoleKey.Subtract:
                        Player.ViewRange--;
                        break;
                    case ConsoleKey.Q:
                        Stop();
                        return;
                }

                while (Console.KeyAvailable)//clear console key buffer
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
}