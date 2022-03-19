using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public class InputProcessor
{
    private Dictionary<object, Controller> _objectHandlers = new();
    private HashSet<Controller> _dormantHandlers = new();
    private OrderedSet<Controller> _activeHandlers = new();
    
    private bool _inLoop;
    private Controller _controllingHandler;

    private Controller ControllingHandler
    {
        get => _controllingHandler;
        set
        {
            _controllingHandler = value;
            if (!_inLoop) Loop();
        }
    }

    private Dictionary<object, (object parent, HashSet<object> children)> _children;

    private static Stack<Controller> Handlers { get; } = new();
    private static bool _isActive;

    public void AddChild(object parent, object child)
    {
        if (!_children.ContainsKey(parent)) return;
        _children[parent].children.Add(child);
        _children.Add(child, (parent, new HashSet<object>()));
    }

    public void SetRoot(object root)
    {
        _children = new Dictionary<object, (object parent, HashSet<object> children)>
        {
            {root, (null, new HashSet<object>())}
        };
    }

    public void YieldControl(object sender)
    {
        if (!_objectHandlers.ContainsKey(sender)) return;
        
        Controller controller = _objectHandlers[sender];
        if (controller != ControllingHandler) return;
        _activeHandlers.Add(ControllingHandler);
        ControllingHandler = controller;
    }

    public void TakeControl(object sender)
    {
        if (!_objectHandlers.ContainsKey(sender)) return;
        
        Controller controller = _objectHandlers[sender];
        if (_dormantHandlers.Contains(controller))
        {
            _dormantHandlers.Remove(controller);
            ControllingHandler = controller;
        }
        else if (_activeHandlers.Contains(controller))
        {
            _activeHandlers.Remove(controller);
            ControllingHandler = controller;
        }
    }

    public void SetController(object sender, Controller controller)
    {
        if (_objectHandlers.ContainsKey(sender))
        {
            Controller oldHandler = _objectHandlers[sender];
            _dormantHandlers.Remove(oldHandler);
            _activeHandlers.Remove(oldHandler);
            if (ControllingHandler == oldHandler)
            {
                RemoveCurrentController();
            }
        }

        _objectHandlers[sender] = controller;
        _dormantHandlers.Add(controller);
    }

    public void Forget(object sender)
    {
        if (!_objectHandlers.ContainsKey(sender)) return;
        Controller controller = _objectHandlers[sender];
        _dormantHandlers.Remove(controller);
        _activeHandlers.Remove(controller);
        if (ControllingHandler == controller)
        {
            RemoveCurrentController();
        }

        foreach (var child in _children[sender].children)
        {
            Forget(child);
        }

        _children.Remove(sender);
        _objectHandlers.Remove(sender);
        object parent = _children[sender].parent;
        if (parent is null) return;
        _children[parent].children.Remove(sender);
    }

    private void RemoveCurrentController()
    {
        if (!_activeHandlers.Any()) return;
        ControllingHandler = _activeHandlers.Pop();
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

    private void Loop()
    {
        _inLoop = true;
        while (ControllingHandler is not null)
        {
            ControllingHandler.HandleKey(Console.ReadKey(true));
        }

        _inLoop = false;
    }
}