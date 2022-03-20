using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public class InputProcessor
{
    private Dictionary<object, Controller> _objectControllers;
    private HashSet<Controller> _dormantControllers;
    private OrderedSet<Controller> _activeControllers;
    
    private bool _inLoop;
    private Controller _currentController;

    public InputProcessor()
    {
        _objectControllers = new Dictionary<object, Controller>();
        _dormantControllers = new HashSet<Controller>();
        _activeControllers = new OrderedSet<Controller>();
    }

    public void YieldControl(object sender)
    {
        if (!_objectControllers.ContainsKey(sender)) return;

        Controller controller = _objectControllers[sender];
        if (controller != _currentController) return;
        _dormantControllers.Add(controller);
        RemoveCurrentController();
    }

    public void TakeControl(object sender)
    {
        if (!_objectControllers.ContainsKey(sender)) return;
        
        Controller controller = _objectControllers[sender];
        if (controller == _currentController) return;
        
        if (_dormantControllers.Contains(controller))
        {
            _dormantControllers.Remove(controller);
            SetControllingHandler(controller);
        }
        else if (_activeControllers.Contains(controller))
        {
            _activeControllers.Remove(controller);
            SetControllingHandler(controller);
        }
    }

    private void SetControllingHandler(Controller controller)
    {
        if (_currentController is not null)
        {
            _activeControllers.Add(_currentController);
        }

        _currentController = controller;
        if (!_inLoop) Loop();
    }

    public void SetController(object sender, Controller controller)
    {
        if (_objectControllers.ContainsKey(sender))
        {
            Controller oldHandler = _objectControllers[sender];
            _dormantControllers.Remove(oldHandler);
            _activeControllers.Remove(oldHandler);
            if (_currentController == oldHandler)
            {
                _currentController = controller;
            }
        }
        else
        {
            _dormantControllers.Add(controller);
        }
        
        _objectControllers[sender] = controller;
    }

    public void RemoveController(object obj)
    {
        if (!_objectControllers.ContainsKey(obj)) return;
        Controller controller = _objectControllers[obj];
        _dormantControllers.Remove(controller);
        _activeControllers.Remove(controller);
        if (_currentController == controller)
        {
            RemoveCurrentController();
        }
        _objectControllers.Remove(obj);
    }

    private void RemoveCurrentController()
    {
        _currentController = null;
        if (!_activeControllers.Any()) return;
        _currentController = _activeControllers.Pop();
    }

    private void Loop()
    {
        _inLoop = true;
        while (_currentController is not null)
        {
            _currentController.HandleKey(Console.ReadKey(true));
        }

        _inLoop = false;
    }
}