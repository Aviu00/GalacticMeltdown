using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated;

public static class UserInterface
{
    private static Renderer _renderer;
    private static InputProcessor _inputProcessor;

    private static OrderedSet<(object, Action)> _tasks;
    private static Dictionary<object, (object, Action)> _objectTasks;
    
    private static Dictionary<object, (object parent, HashSet<object> children)> _children;

    static UserInterface()
    {
        _renderer = new Renderer();
        _inputProcessor = new InputProcessor();
        _tasks = new OrderedSet<(object, Action)>();
        _objectTasks = new Dictionary<object, (object, Action)>();
    }

    public static void Start()
    {
        while (_tasks.Any())
        {
            var (obj, task) = _tasks.Pop();
            _objectTasks.Remove(obj);
            task();
        }

        _renderer.CleanUp();
    }

    public static void SetTask(object obj, Action task)
    {
        if (!_children.ContainsKey(obj)) return;
        if (_objectTasks.ContainsKey(obj))
        {
            _tasks.Remove(_objectTasks[obj]);
            _objectTasks.Remove(obj);
        }
        (object, Action) taskTuple = (obj, task);
        _tasks.Add(taskTuple);
        _objectTasks.Add(obj, taskTuple);
    }

    public static void SetViewPositioner(object obj, ViewPositioner viewPositioner)
    {
        if (!_children.ContainsKey(obj)) return;
        _renderer.SetView(obj, viewPositioner);
    }

    public static void SetController(object obj, Controller controller)
    {
        if (!_children.ContainsKey(obj)) return;
        _inputProcessor.SetController(obj, controller);
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
        if (!_children.ContainsKey(obj)) return;
        _inputProcessor.TakeControl(obj);
    }

    public static void YieldControl(object obj)
    {
        if (!_children.ContainsKey(obj)) return;
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
        
        _renderer.RemoveViewPositioner(obj);
        _inputProcessor.RemoveController(obj);
        if (!_objectTasks.ContainsKey(obj)) return;
        _tasks.Remove(_objectTasks[obj]);
        _objectTasks.Remove(obj);
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