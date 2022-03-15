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

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (_actionMapping.ContainsKey(keyInfo.Key)) _actionMapping[keyInfo.Key]();
        base.HandleKey(keyInfo);
    }
}