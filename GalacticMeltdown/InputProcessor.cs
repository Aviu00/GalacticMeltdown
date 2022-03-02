using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public static class InputProcessor
{
    private static Stack<Dictionary<ConsoleKey, Action>> Bindings { get; } = new();
    private static bool _isActive;

    public static void StartProcessLoop()
    {
        _isActive = true;
        while (_isActive)
        {
            // Ignore the keypresses that happened before
            while (Console.KeyAvailable) Console.ReadKey(true);
            ConsoleKey key = Console.ReadKey(true).Key;
            if (Bindings.Peek().ContainsKey(key)) Bindings.Peek()[key]();
        }
    }

    public static void StopProcessLoop() => _isActive = false;

    public static void AddBinding<TEnum>(Dictionary<ConsoleKey, TEnum> controlMode,
        Dictionary<TEnum, Action> actions) =>
        Bindings.Push(Utility.JoinDictionaries(controlMode, actions));

    public static void RemoveLastBinding() => Bindings.Pop();

    public static void ClearBindings() => Bindings.Clear();
}