using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class TextView : View
{
    private const ConsoleColor TextColor = DataHolder.Colors.TextViewColor;
    private const ConsoleColor DefaultBackgroundColor = DataHolder.Colors.DefaultBackgroundColor;

    private List<string> _lines;
    private char[,] _characters;
    private int _topTextRow;

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;

    public TextView(List<string> lines)
    {
        _lines = lines;
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        int totalLines = _lines.Sum(line => (line.Length + Width - 1) / Width);
        _characters = new char[Width, totalLines];
        int curRow = 0;
        foreach (var textLine in _lines)
        {
            if (textLine.Length == 0)
            {
                curRow++;
                continue;
            }

            for (int i = 0; i < textLine.Length; i++)
            {
                if (i % Width == 0) curRow++;
                _characters[curRow, i % Width] = textLine[i];
            }
        }

        _topTextRow = 0;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _characters.GetLength(1)) return new ViewCellData(null, null);
        char character = _characters[x, Height - y - 1 + _topTextRow];
        return new ViewCellData(character == '\0' ? null : (character, TextColor), null);
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        for (int viewX = 0; viewX < Width; viewX++)
        {
            for (int viewY = 0; viewY < Height - _lines.Count; viewY++)
            {
                cells[viewX, viewY] = new ViewCellData(null, DefaultBackgroundColor);
            }
        }

        for (int viewX = 0; viewX < Width; viewX++)
        {
            for (int viewY = Math.Max(0, Height - _lines.Count); viewY < Height; viewY++)
            {
                char character = _characters[viewX, Height - viewY - 1 + _topTextRow];
                cells[viewX, viewY] = new ViewCellData(character == '\0' ? null : (character, TextColor), null);
            }
        }

        return cells;
    }

    public void ScrollNext()
    {
        if (_characters.GetLength(1) < Height || _characters.GetLength(1) - Height == _topTextRow) return;
        _topTextRow++;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void ScrollPrev()
    {
        if (_characters.GetLength(1) < Height || _topTextRow == 0) return;
        _topTextRow--;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}