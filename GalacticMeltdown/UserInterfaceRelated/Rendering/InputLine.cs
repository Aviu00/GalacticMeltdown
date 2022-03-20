using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class InputLine : PressableListLine
{
    private const ConsoleColor Selected = DataHolder.Colors.InputLineBgColorSelected;
    private const ConsoleColor Unselected = DataHolder.Colors.InputLineBgColorUnselected;
    private const ConsoleColor TextColor = DataHolder.Colors.InputLineTextColor;

    private StringBuilder _currentText;

    private bool _selected;

    public event EventHandler Updated;

    public override ViewCellData this[int x]
    {
        get
        {
            char symbol;
            if (_currentText.Length < Width)
            {
                symbol = x < _currentText.Length ? _currentText[x] : ' ';
            }
            else
            {
                symbol = _currentText[_currentText.Length - Width + x];
            }
            return new ViewCellData((symbol, TextColor), _selected ? Selected : Unselected);
        }
    }

    public override void Press()
    {
        UserInterface.SetController(this,
            new TextInputHandler(AddCharacter,
                UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.TextInput,
                    new Dictionary<TextInputControl, Action>
                    {
                        {TextInputControl.Back, () => UserInterface.Forget(this)},
                        {TextInputControl.DeleteCharacter, DeleteCharacter}
                    }),
                chr => chr == ' ' || !(char.IsControl(chr) || char.IsSeparator(chr) || char.IsSurrogate(chr))));
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

    public override void Select() => _selected = true;
    
    public override void Deselect() => _selected = false;
}