using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GalacticMeltdown.Rendering;

public class ButtonListView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;
    private readonly ImmutableList<Button> _buttons;
    private int _currentButtonIndex;
    private int _topVisibleButtonIndex;
    private string[] _buttonText;
    private int _selectedButtonY;
    private const ConsoleColor TextColor = ConsoleColor.Magenta;
    private const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
    private const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;

    public ButtonListView(ICollection<Button> buttons)
    {
        // TODO: this should have at least one button, check that
        _buttons = ImmutableList<Button>.Empty.AddRange(buttons);
        _currentButtonIndex = 0;
        _topVisibleButtonIndex = 0;
        _buttonText = new string[_buttons.Count];
    }

    private void CalculateButtonText()
    {
        for (int i = 0; i < _buttonText.Length; i++)
        {
            _buttonText[i] = _buttons[i].MakeText(Width);
        }
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        CalculateButtonText();
        UpdateOutVars();
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _buttons.Count) return new ViewCellData(null, null);
        char symbol = _buttonText[Height - (y - _topVisibleButtonIndex) - 1][x];
        ConsoleColor fgColor = TextColor;
        ConsoleColor bgColor = _selectedButtonY == y ? BackgroundColorSelected : BackgroundColorUnselected;
        return new ViewCellData(symbol == ' ' ? null : (symbol, fgColor), bgColor);
    }

    public void PressCurrent()
    {
        _buttons[_currentButtonIndex].Press();
    }

    public void SelectNext()
    {
        _currentButtonIndex = (_currentButtonIndex + 1) % _buttons.Count;
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SelectPrev()
    {
        _currentButtonIndex = (_buttons.Count + _currentButtonIndex - 1) % _buttons.Count;  // -1 should wrap
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    private void UpdateOutVars()
    {
        _topVisibleButtonIndex = Math.Max(0, _currentButtonIndex - Height + 1);
        _selectedButtonY = Height - (_currentButtonIndex - _topVisibleButtonIndex) - 1;
    }
}