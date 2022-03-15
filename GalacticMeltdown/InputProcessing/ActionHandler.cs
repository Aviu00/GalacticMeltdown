using System;
using System.Collections.Generic;

namespace GalacticMeltdown.InputProcessing;

public class ActionHandler : KeyHandler
{
    private Dictionary<ConsoleKey, Action> _actionMapping;

    public ActionHandler(Dictionary<ConsoleKey, Action> actionMapping) : base(true)
    {
        _actionMapping = actionMapping;
    }

    public override void HandleKey(ConsoleKey key)
    {
        if (_actionMapping.ContainsKey(key)) _actionMapping[key]();
        base.HandleKey(key);
    }
}