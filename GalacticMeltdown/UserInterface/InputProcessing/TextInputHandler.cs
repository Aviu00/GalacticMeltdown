using System;
using System.Collections.Generic;

namespace GalacticMeltdown.UserInterface.InputProcessing;

public class TextInputHandler : KeyHandler
{
    private readonly Action<char> _inputAction;
    private readonly Dictionary<ConsoleKey, Action> _reservedKeyActions;
    private readonly Func<char, bool> _isCharacterAllowed;

    public TextInputHandler(Action<char> inputAction,
        Dictionary<ConsoleKey, Action> reservedKeyActions,
        Func<char, bool> isCharacterAllowed = null) : base(false)
    {
        _inputAction = inputAction;
        _reservedKeyActions = reservedKeyActions;
        _isCharacterAllowed = isCharacterAllowed;
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        char character = keyInfo.KeyChar;
        if (_reservedKeyActions.ContainsKey(keyInfo.Key)) _reservedKeyActions[keyInfo.Key]();
        else if (_isCharacterAllowed is null || _isCharacterAllowed(character)) _inputAction(character);
        base.HandleKey(keyInfo);
    }
}