using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public class InputProcessor
{
    private Dictionary<object, Controller> _objectControllers;
    private HashSet<Controller> _dormantHandlers;
    private OrderedSet<Controller> _activeHandlers;
    
    private bool _inLoop;
    private Controller _currentController;

    private Controller CurrentController
    {
        get => _currentController;
        set
        {
            _currentController = value;
            if (!_inLoop) Loop();
        }
    }

    private Dictionary<object, (object parent, HashSet<object> children)> _children;

    private static Stack<Controller> Handlers { get; } = new();
    private static bool _isActive;

    public InputProcessor()
    {
        _objectControllers = new Dictionary<object, Controller>();
        _dormantHandlers = new HashSet<Controller>();
        _activeHandlers = new OrderedSet<Controller>();
    }

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
        if (!_objectControllers.ContainsKey(sender)) return;
        
        Controller controller = _objectControllers[sender];
        if (controller != CurrentController) return;
        _activeHandlers.Add(CurrentController);
        CurrentController = controller;
    }

    public void TakeControl(object sender)
    {
        if (!_objectControllers.ContainsKey(sender)) return;
        
        Controller controller = _objectControllers[sender];
        if (_dormantHandlers.Contains(controller))
        {
            _dormantHandlers.Remove(controller);
            SetControllingHandler(controller);
        }
        else if (_activeHandlers.Contains(controller))
        {
            _activeHandlers.Remove(controller);
            SetControllingHandler(controller);
        }
    }

    private void SetControllingHandler(Controller controller)
    {
        if (CurrentController is not null)
        {
            _activeHandlers.Add(CurrentController);
        }

        CurrentController = controller;
    }

    public void SetController(object sender, Controller controller)
    {
        if (_objectControllers.ContainsKey(sender))
        {
            Controller oldHandler = _objectControllers[sender];
            _dormantHandlers.Remove(oldHandler);
            _activeHandlers.Remove(oldHandler);
            if (CurrentController == oldHandler)
            {
                RemoveCurrentController();
            }
        }

        _objectControllers[sender] = controller;
        _dormantHandlers.Add(controller);
    }

    public void Forget(object obj)
    {
        if (!_children.ContainsKey(obj)) return;
        ForgetInternal(obj);
    }

    private void ForgetInternal(object obj)
    {
        foreach (object child in _children[obj].children)
        {
            ForgetInternal(child);
        }
        
        object parent = _children[obj].parent;
        if (parent is not null) _children[parent].children.Remove(obj);
        _children.Remove(obj);
        
        if (!_objectControllers.ContainsKey(obj)) return;
        Controller controller = _objectControllers[obj];
        _dormantHandlers.Remove(controller);
        _activeHandlers.Remove(controller);
        if (CurrentController == controller)
        {
            RemoveCurrentController();
        }
        _objectControllers.Remove(obj);
    }

    private void RemoveCurrentController()
    {
        CurrentController = null;
        if (!_activeHandlers.Any()) return;
        CurrentController = _activeHandlers.Pop();
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
        while (CurrentController is not null)
        {
            CurrentController.HandleKey(Console.ReadKey(true));
        }

        _inLoop = false;
    }
}