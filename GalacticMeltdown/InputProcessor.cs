using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public class InputProcessor
{
    public Dictionary<ConsoleKey, Action> Bindings { get; set; } = new Dictionary<ConsoleKey, Action>();
    private bool _isActive = false;

    public void StartProcessLoop()
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

    public void StopProcessLoop()
    {
        _isActive = false;
    }
}