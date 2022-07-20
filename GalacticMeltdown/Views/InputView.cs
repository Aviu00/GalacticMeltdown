using System;
using System.Collections.Generic;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class InputView : View, IMultiCellUpdate
{
    private const ConsoleColor BgColor = Colors.Input.Background;
    private const ConsoleColor TextColor = Colors.Input.Text;
    private const ConsoleColor CursorColor = Colors.Input.Cursor;

    public override event EventHandler NeedRedraw;
    public event EventHandler<MultiCellUpdateEventArgs> MultiCellUpdate;

    private StringBuilder _currentText = new();
    public string Text => _currentText.ToString();

    public override ViewCellData GetSymbol(int x, int y)
    {
        ConsoleColor? bgColor = y > _currentText.Length / Width ? null : BgColor;
        if (y * Width + x == _currentText.Length) bgColor = CursorColor;
        (char, ConsoleColor TextColor)? symbolData =
            y * Width + x < _currentText.Length ? (_currentText[y * Width + x], TextColor) : null;
        return new ViewCellData(symbolData, bgColor);
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        if (_currentText.Length < Height * Width)
        {
            for (int row = 0; row < _currentText.Length / Width + 1; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    cells[col, row] = new ViewCellData(null, BgColor);
                }
            }
        }
        
        for (var i = 0; i < Math.Min(_currentText.Length, Height * Width); i++)
        {
            cells[i % Width, i / Width] = new ViewCellData((_currentText[i], TextColor), BgColor);
        }

        if (_currentText.Length < Width * Height)
        {
            cells[_currentText.Length % Width, _currentText.Length / Width] =
                new ViewCellData(cells[_currentText.Length % Width, _currentText.Length / Width].SymbolData,
                    CursorColor);
        }
        return cells;
    }

    public void DeleteCharacter()
    {
        if (_currentText.Length == 0) return;
        if (_currentText.Length-- > Width * Height) return;
        if ((_currentText.Length + 1) % Width == 0)
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        else
            MultiCellUpdate?.Invoke(this,
                new MultiCellUpdateEventArgs(new List<(int, int, ViewCellData)>
                {
                    (_currentText.Length % Width, _currentText.Length / Width,
                        new ViewCellData(null, CursorColor)),
                    (_currentText.Length % Width + 1, _currentText.Length / Width,
                        new ViewCellData(null, BgColor)),
                }));
    }

    public void AddCharacter(char character)
    {
        _currentText.Append(character);
        if (_currentText.Length > Width * Height) return;
        if (_currentText.Length % Width == 0)
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        else
            MultiCellUpdate?.Invoke(this, new MultiCellUpdateEventArgs(new List<(int, int, ViewCellData)>
            {
                (_currentText.Length % Width - 1, _currentText.Length / Width,
                    new ViewCellData((_currentText[^1], TextColor), BgColor)),
                (_currentText.Length % Width, _currentText.Length / Width, new ViewCellData(null, CursorColor)),
            }));
    }
}