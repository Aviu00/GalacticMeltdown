using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.Rendering;

public class ButtonListView : View
{
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    private readonly ImmutableList<MenuButtonInfo> _buttons;

    private int _currentButtonIndex;
    private int _topVisibleButtonIndex;
    private int _selectedButtonY;

    public ButtonListView(ICollection<Button> buttons)
    {
        // TODO: this should have at least one button, check that
        _buttons = ImmutableList<MenuButtonInfo>.Empty.AddRange(buttons.Select(button => new MenuButtonInfo(button)));
        _currentButtonIndex = 0;
        _topVisibleButtonIndex = 0;
    }

    private void CalculateButtonText()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].RenderedText = _buttons[i].Button.MakeText(Width);
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
        char symbol = _buttons[Height - (y - _topVisibleButtonIndex) - 1].RenderedText[x];
        ConsoleColor fgColor = DataHolder.Colors.TextColor;
        ConsoleColor bgColor = _selectedButtonY == y
            ? DataHolder.Colors.BackgroundColorSelected
            : DataHolder.Colors.BackgroundColorUnselected;
        return new ViewCellData(symbol == ' ' ? null : (symbol, fgColor), bgColor);
    }

    public void PressCurrent()
    {
        _buttons[_currentButtonIndex].Button.Press();
    }

    public void SelectNext()
    {
        _currentButtonIndex = (_currentButtonIndex + 1) % _buttons.Count;
        UpdateOutVars();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void SelectPrev()
    {
        _currentButtonIndex -= 1;
        if (_currentButtonIndex == -1) _currentButtonIndex = _buttons.Count - 1;
        UpdateOutVars();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateOutVars()
    {
        _topVisibleButtonIndex = Math.Max(0, _currentButtonIndex - Height + 1);
        _selectedButtonY = Height - (_currentButtonIndex - _topVisibleButtonIndex) - 1;
    }
}