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
        if (Width < 8)
        {
            string placeHolder = Width < 3 ? "" : "...";
            for (int i = 0; i < _buttonText.Length; i++)
                _buttonText[i] = placeHolder;
            return;
        }

        for (int i = 0; i < _buttonText.Length; i++)
        {
            const string ellipsis = "...";
            const string separator = "  ";
            const string noSpaceForRightText = $"{separator}{ellipsis}";
            string rightText = _buttons[i].TextRight;
            int maxLeftStringLength = Width - noSpaceForRightText.Length;
            string leftText = _buttons[i].TextLeft;
            string screenText;
            if (rightText.Length == 0)
            {
                screenText = leftText.Length > Width - ellipsis.Length 
                    ? leftText.Substring(0, Width - ellipsis.Length) 
                    : leftText;
            }
            else if (leftText.Length == 0)
            {
                if (rightText.Length < Width)
                {
                    screenText = string.Join("", Enumerable.Repeat(" ", Width - rightText.Length)) + rightText;
                }
                else
                {
                    screenText = ellipsis + rightText.Substring(rightText.Length - (Width - ellipsis.Length));
                }
            }
            else if (leftText.Length >= maxLeftStringLength)
            {
                screenText = leftText.Substring(0, maxLeftStringLength) + noSpaceForRightText;
            }
            else
            {
                screenText = leftText;
                screenText += separator;
                int spaceLeft = Width - screenText.Length;
                if (rightText.Length >= spaceLeft)
                {
                    screenText += rightText.Substring(0, spaceLeft - ellipsis.Length) + ellipsis;
                }
                else
                {
                    screenText += string.Join("", Enumerable.Repeat(" ", spaceLeft - rightText.Length)) + rightText;
                }
            }

            _buttonText[i] = screenText;
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