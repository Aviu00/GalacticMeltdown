using System;
using System.Text;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class InputView : View, IOneCellUpdate
{
    private const ConsoleColor BgColor = Colors.Input.Background;
    private const ConsoleColor TextColor = Colors.Input.Text;

    public override event EventHandler NeedRedraw;
    public event EventHandler<OneCellUpdateEventArgs> OneCellUpdate;

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
        if (Width == 0 || Height == 0) return cells;
        if (_currentText.Length / Width < Height)
        {
            for (int row = 0; row < _currentText.Length / Width + 1; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    cells[col, row] = new ViewCellData(null, BgColor);
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
        if (_currentText.Length-- <= Width * Height)
        {
            if ((_currentText.Length + 1) % Width == 0)
                NeedRedraw?.Invoke(this, EventArgs.Empty);
            else
            {
                OneCellUpdate?.Invoke(this,
                    new OneCellUpdateEventArgs((_currentText.Length % Width, _currentText.Length / Width,
                        new ViewCellData(null, BgColor))));
            }
        }
    }

    public void AddCharacter(char character)
    {
        _currentText.Append(character);
        if (_currentText.Length <= Width * Height)
        {
            if (_currentText.Length % Width == 0)
                NeedRedraw?.Invoke(this, EventArgs.Empty);
            else
                OneCellUpdate?.Invoke(this,
                    new OneCellUpdateEventArgs((_currentText.Length % Width - 1, _currentText.Length / Width,
                        new ViewCellData((_currentText[^1], TextColor), BgColor))));
        }
    }
}