using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

internal enum InputLineState
{
    Selected,
    Unselected,
    Pressed
}

public class InputLine : PressableListLine
{
    private const ConsoleColor EnteredTextColor = Colors.Input.Text;
    private const ConsoleColor Unselected = Colors.Input.Background;
    private const ConsoleColor Cursor = Colors.Input.Cursor;
    private const ConsoleColor Selected = Colors.MenuLine.InputLine.Selected;
    private const ConsoleColor Pressed = Colors.MenuLine.InputLine.Pressed;

    private StringBuilder _currentText;

    private InputLineState _state;

    private Func<char, bool> _characterValidationFunction;

    public string Text => _currentText.ToString();

    public event EventHandler Updated;

    public InputLine(Func<char, bool> characterValidationFunction = null)
    {
        _currentText = new StringBuilder();
        _characterValidationFunction = characterValidationFunction;
        _state = InputLineState.Unselected;
    }

    public override ViewCellData this[int x]
    {
        get
        {
            char symbol;
            ConsoleColor bgColor = _state switch
            {
                InputLineState.Pressed => Pressed,
                InputLineState.Selected => Selected,
                InputLineState.Unselected => Unselected,
                _ => Unselected
            };
            if (_currentText.Length < Width - 1)
            {
                if (x < _currentText.Length)
                {
                    symbol = _currentText[x];
                }
                else
                {
                    symbol = ' ';
                    if (x == _currentText.Length && _state == InputLineState.Pressed) bgColor = Cursor;
                }
            }
            else
            {
                if (x == Width - 1)
                {
                    symbol = ' ';
                    if (_state == InputLineState.Pressed) bgColor = Cursor;
                }
                else
                {
                    symbol = _currentText[_currentText.Length - (Width - 1) + x];                    
                }
            }
            return new ViewCellData((symbol, EnteredTextColor), bgColor);
        }
    }

    public override void Press()
    {
        _state = InputLineState.Pressed;
        Updated?.Invoke(this, EventArgs.Empty);
        UserInterface.SetController(this,
            new TextInputController(AddCharacter,
                UtilityFunctions.JoinDictionaries(KeyBindings.TextInput,
                    new Dictionary<TextInputControl, Action>
                    {
                        {TextInputControl.Back, Back},
                        {TextInputControl.DeleteCharacter, DeleteCharacter}
                    }),
                _characterValidationFunction ?? (chr =>
                    chr == ' ' || !(char.IsControl(chr) || char.IsSeparator(chr) || char.IsSurrogate(chr)))));
        UserInterface.TakeControl(this);
    }

    private void DeleteCharacter()
    {
        if (_currentText.Length == 0) return;
        _currentText.Length--;
        Updated?.Invoke(this, EventArgs.Empty);
    }

    private void AddCharacter(char character)
    {
        _currentText.Append(character);
        Updated?.Invoke(this, EventArgs.Empty);
    }

    public override void Select()
    {
        _state = InputLineState.Selected;
        Updated?.Invoke(this, EventArgs.Empty);
    }

    public override void Deselect()
    {
        _state = InputLineState.Unselected;
        Updated?.Invoke(this, EventArgs.Empty);
    }

    private void Back()
    {
        _state = InputLineState.Selected;
        Updated?.Invoke(this, EventArgs.Empty);
        UserInterface.YieldControl(this);
    }
}