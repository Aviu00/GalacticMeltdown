using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
    private const ConsoleColor BackgroundColorUnselected = ConsoleColor.Yellow;
    private const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkBlue;

    public ButtonListView(ICollection<Button> buttons)
    {
        // TODO: this should have at least one button, check that
        _buttons = ImmutableList<Button>.Empty.AddRange(buttons);
        _currentButtonIndex = 0;
        _topVisibleButtonIndex = 0;
        _buttonText = new string[_buttons.Count];
    }

    private void CalculateVisibleButtonText()
    {
        for (int i = 0; i < _buttonText.Length; i++)
        {
            _buttonText[i] = _buttons[i].MakeText(Width);
        }
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        CalculateVisibleButtonText();
        UpdateOffsets();
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y > _buttons.Count) return new ViewCellData(null, null);
        _selectedButtonY = _currentButtonIndex - _topVisibleButtonIndex;
        char symbol = _buttonText[_currentButtonIndex][x];
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
        UpdateOffsets();
        NeedRedraw?.Invoke(this);
    }

    public void SelectPrev()
    {
        _currentButtonIndex = (_buttons.Count + _currentButtonIndex - 1) % _buttons.Count;  // -1 should wrap
        UpdateOffsets();
        NeedRedraw?.Invoke(this);
    }

    private void UpdateOffsets()
    {
        _topVisibleButtonIndex = Math.Max(0, _currentButtonIndex - Height + 1);
        _selectedButtonY = _currentButtonIndex - _topVisibleButtonIndex;
    }
}