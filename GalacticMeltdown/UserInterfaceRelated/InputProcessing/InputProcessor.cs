using System;
using System.Collections.Generic;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public static class InputProcessor
{
    private static Dictionary<object, KeyHandler> _objectHandlers;
    private static List<KeyHandler> _dormantHandlers;
    private static List<KeyHandler> _activeHandlers;
    private static KeyHandler _controllingHandler;

    private static Dictionary<object, (object parent, HashSet<object> children)> _children;

    private static Stack<KeyHandler> Handlers { get; } = new();
    private static bool _isActive;

    public static void StartProcessLoop()
    {
        _isActive = true;
        while (_isActive)
        {
            Handlers.Peek().HandleKey(Console.ReadKey(true));
        }
    }

    public static void StopProcessLoop() => _isActive = false;

    public static void
        AddBinding<TEnum>(Dictionary<ConsoleKey, TEnum> controlMode, Dictionary<TEnum, Action> actions) =>
        Handlers.Push(new ActionHandler(UtilityFunctions.JoinDictionaries(controlMode, actions)));

    public static void AddTextInputHandler(Action<char> inputAction, Dictionary<ConsoleKey, Action> reservedKeyActions,
        Func<char, bool> isCharacterAllowed = null)
    {
        Handlers.Push(new TextInputHandler(inputAction, reservedKeyActions, isCharacterAllowed));
    }

    public static void RemoveLastBinding() => Handlers.Pop();

    public static void ClearBindings() => Handlers.Clear();
}