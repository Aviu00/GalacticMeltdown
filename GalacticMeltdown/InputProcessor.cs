using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public static class InputProcessor
{
    public static Dictionary<ConsoleKey, Action> Bindings { get; set; } = new Dictionary<ConsoleKey, Action>();
    private static bool _isActive = false;

    public static void StartProcessLoop()
    {
        _isActive = true;
        while (_isActive)
        {
            // Ignore the keypresses that happened before
            while (Console.KeyAvailable) Console.ReadKey(true);
            ConsoleKey key = Console.ReadKey(true).Key;
            if (Bindings.ContainsKey(key)) Bindings[key]();
        }
    }

    public static void StopProcessLoop()
    {
        _isActive = false;
    }
}