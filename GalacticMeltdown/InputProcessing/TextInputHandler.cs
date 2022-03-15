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
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (_reservedKeyActions.ContainsKey(keyInfo.Key)) _reservedKeyActions[keyInfo.Key]();
        base.HandleKey(keyInfo);
    }
}