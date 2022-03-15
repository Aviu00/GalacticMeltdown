using System;
using System.Collections.Generic;

namespace GalacticMeltdown.InputProcessing;

public class TextInputHandler : KeyHandler
{
    private Action<char> _inputAction;
    private Dictionary<ConsoleKey, Action> _reservedKeyActions;
    private HashSet<char> _allowedCharacters;

    public TextInputHandler(Action<char> inputAction, Dictionary<ConsoleKey, Action> reservedKeyActions,
        HashSet<char> allowedCharacters) : base(false)
    {
        _inputAction = inputAction;
        _reservedKeyActions = reservedKeyActions;
        _allowedCharacters = allowedCharacters;
    }
    
    public override void HandleKey(ConsoleKey key)
    {
        if (_reservedKeyActions.ContainsKey(key)) _reservedKeyActions[key]();
        base.HandleKey(key);
    }
}