using System;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class InputView : View
{
    private const ConsoleColor BgColor = DataHolder.Colors.TextBoxDefaultBgColor;
    private const ConsoleColor TextColor = DataHolder.Colors.TextBoxTextColor;

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;
    
    private StringBuilder _currentText = new();
    public string Text => _currentText.ToString();

    public override ViewCellData GetSymbol(int x, int y)
    {
        ConsoleColor? bgColor = y > _currentText.Length / Width ? null : BgColor;
        (char, ConsoleColor TextColor)? symbolData =
            y * Width + x < _currentText.Length ? (_currentText[y * Width + x], TextColor) : null;
        return new ViewCellData(symbolData, bgColor);
    }

    public override ViewCellData[,] GetAllCells()
    {
        ViewCellData[,] cells = new ViewCellData[Width, Height];
        return cells;
    }

    public void DeleteCharacter()
    {
        if (_currentText.Length == 0) return;
        _currentText.Length--;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void AddCharacter(char character)
    {
        _currentText.Append(character);
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}