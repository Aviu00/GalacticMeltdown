using System;
using System.Collections.Generic;

namespace GalacticMeltdown.InputProcessing;

public class TextInputHandler : KeyHandler
{
    private readonly Action<char> _inputAction;
    private readonly Dictionary<ConsoleKey, Action> _reservedKeyActions;
    private readonly HashSet<char> _allowedCharacters;

    public TextInputHandler(Action<char> inputAction, Dictionary<ConsoleKey, Action> reservedKeyActions,
        HashSet<char> allowedCharacters) : base(false)
    {
        _inputAction = inputAction;
        _reservedKeyActions = reservedKeyActions;
        _allowedCharacters = allowedCharacters;
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        char character = keyInfo.KeyChar;
        if (_reservedKeyActions.ContainsKey(keyInfo.Key)) _reservedKeyActions[keyInfo.Key]();
        else if (_allowedCharacters is null || _allowedCharacters.Contains(character)) _inputAction(character);
        base.HandleKey(keyInfo);
    }
}