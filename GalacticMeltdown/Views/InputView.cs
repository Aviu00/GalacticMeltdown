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
        var cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0) return cells;
        if (_currentText.Length / Width < Height)
        {
            for (int row = 0; row < _currentText.Length / Width; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    cells[row, col] = new ViewCellData(null, BgColor);
                }
            }
        }
        
        for (var i = 0; i < _currentText.Length && i / Width < Height; i++)
        {
            cells[i % Width, i / Width] = new ViewCellData((_currentText[i], TextColor), BgColor);
        }
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