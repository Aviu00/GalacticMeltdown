using System;
using System.Collections.Generic;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public class ActionController : Controller
{
    private Dictionary<ConsoleKey, Action> _actionMapping;

    public ActionController(Dictionary<ConsoleKey, Action> actionMapping) : base(true)
    {
        _actionMapping = actionMapping;
    }

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (_actionMapping.ContainsKey(keyInfo.Key)) _actionMapping[keyInfo.Key]();
        base.HandleKey(keyInfo);
    }
}