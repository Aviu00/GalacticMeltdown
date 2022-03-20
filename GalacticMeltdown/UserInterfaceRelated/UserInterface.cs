using System;
using System.Collections.Generic;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated;

public static class UserInterface
{
    private static Renderer _renderer;
    private static InputProcessor _inputProcessor;

    private static Action _nextTask;
    
    private static Dictionary<object, (object parent, HashSet<object> children)> _children;

    static UserInterface()
    {
        _renderer = new Renderer();
        _inputProcessor = new InputProcessor();
    }

    public static void Start()
    {
        while (_nextTask is not null)
        {
            Action task = _nextTask;
            _nextTask = null;
            task();
        }

        _renderer.CleanUp();
    }

    public static void SetTask(Action task)
    {
        _nextTask ??= task;
    }

    public static void SetView(object sender, View view)
    {
        _renderer.SetView(sender, view);
    }

    public static void SetController(object sender, Controller controller)
    {
        _inputProcessor.SetController(sender, controller);
    }

    public static void SetRoot(object root)
    {
        _children = new Dictionary<object, (object parent, HashSet<object> children)>
        {
            {root, (null, new HashSet<object>())}
        };
    }

    public static void TakeControl(object obj)
    {
        _inputProcessor.TakeControl(obj);
    }

    public static void YieldControl(object obj)
    {
        _inputProcessor.YieldControl(obj);
    }

    public static void Forget(object obj)
    {
        if (obj is null || !_children.ContainsKey(obj)) return;
        foreach (object child in _children[obj].children)
        {
            Forget(child);
        }
        
        object parent = _children[obj].parent;
        if (parent is not null) _children[parent].children.Remove(obj);
        _children.Remove(obj);
        
        _renderer.RemoveView(obj);
        _inputProcessor.RemoveController(obj);
    }
    
    public static void AddChild(object parent, object child)
    {
        if (!_children.ContainsKey(parent)) return;
        _children[parent].children.Add(child);
        _children.Add(child, (parent, new HashSet<object>()));
    }

    public static void PlayAnimations()
    {
        _renderer.PlayAnimations();
    }
}