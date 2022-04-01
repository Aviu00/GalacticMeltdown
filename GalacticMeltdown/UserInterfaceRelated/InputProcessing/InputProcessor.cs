using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public class InputProcessor
{
    private Dictionary<object, Controller> _objectControllers;
    private OrderedSet<Controller> _activeControllers;
    
    private bool _inLoop;

    public InputProcessor()
    {
        _objectControllers = new Dictionary<object, Controller>();
        _activeControllers = new OrderedSet<Controller>();
    }

    public void YieldControl(object sender)
    {
        if (!_objectControllers.TryGetValue(sender, out Controller controller)) return;
        _activeControllers.Remove(controller);
    }

    public void TakeControl(object sender)
    {
        if (!_objectControllers.TryGetValue(sender, out Controller controller)) return;
        _activeControllers.Remove(controller);
        _activeControllers.Add(controller);
        if (!_inLoop) Loop();
    }

    public void SetController(object sender, Controller controller)
    {
        if (_objectControllers.TryGetValue(sender, out Controller oldController))
        {
            _activeControllers.Remove(oldController);
        }

        if (controller is null)
        {
            _objectControllers.Remove(sender);
            return;
        }

        _objectControllers[sender] = controller;
    }

    public void RemoveController(object obj)
    {
        if (!_objectControllers.TryGetValue(obj, out Controller controller)) return;
        _activeControllers.Remove(controller);
        _objectControllers.Remove(obj);
    }

    private void Loop()
    {
        _inLoop = true;
        Controller active;
        while ((active = _activeControllers.Last) is not null)
        {
            active.HandleKey(Console.ReadKey(true));
        }

        _inLoop = false;
    }
}