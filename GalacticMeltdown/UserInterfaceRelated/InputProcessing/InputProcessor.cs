using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public static class InputProcessor
{
    private static Dictionary<object, KeyHandler> _objectHandlers;
    private static HashSet<KeyHandler> _dormantHandlers;
    private static OrderedSet<KeyHandler> _activeHandlers;
    private static KeyHandler _controllingHandler;

    private static Dictionary<object, (object parent, HashSet<object> children)> _children;

    private static Stack<KeyHandler> Handlers { get; } = new();
    private static bool _isActive;

    public static void SetController(object sender, KeyHandler controller)
    {
        if (_objectHandlers.ContainsKey(sender))
        {
            KeyHandler oldHandler = _objectHandlers[sender];
            _dormantHandlers.Remove(oldHandler);
            _activeHandlers.Remove(oldHandler);
            if (_controllingHandler == oldHandler)
            {
                RemoveCurrentController();
            }
        }

        _objectHandlers[sender] = controller;
    }

    public static void Forget(object sender)
    {
        if (!_objectHandlers.ContainsKey(sender)) return;
        KeyHandler controller = _objectHandlers[sender];
        _dormantHandlers.Remove(controller);
        _activeHandlers.Remove(controller);
        if (_controllingHandler == controller)
        {
            RemoveCurrentController();
        }
    }

    private static void RemoveCurrentController()
    {
        if (!_activeHandlers.Any()) return;
        _controllingHandler = _activeHandlers.Pop();
    }

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