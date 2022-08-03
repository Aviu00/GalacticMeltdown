using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.Views;

public class TextView : View, IFullRedraw
{
    private const ConsoleColor TextColor = Colors.TextView.Normal;
    private const ConsoleColor BackgroundColor = Colors.TextView.Background;

    private List<string> _lines;
    private char[,] _characters;
    private int _topTextRow;

    public event EventHandler NeedRedraw;

    public TextView(List<string> lines)
    {
        _lines = lines;
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        _characters = new char[Width, _lines.Sum(line => line.Length == 0 ? 1 : (line.Length - 1) / Width + 1)];
        int curRow = -1;
        _topTextRow = 0;
        if (Width == 0 || Height == 0) return;
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
                _characters[i % Width, curRow] = textLine[i];
            }
        }
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _characters.GetLength(1) || Height == 0 || Width == 0)
            return new ViewCellData(null, BackgroundColor);
        char character = _characters[x, Height - y - 1 + _topTextRow];
        return new ViewCellData(character == '\0' ? null : (character, TextColor), BackgroundColor);
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        for (int viewX = 0; viewX < Width; viewX++)
        {
            for (int viewY = 0; viewY < Height - _lines.Count; viewY++)
            {
                cells[viewX, viewY] = new ViewCellData(null, BackgroundColor);
            }
        }
        if (Width == 0 || Height == 0) return cells;

        for (int viewX = 0; viewX < Width; viewX++)
        {
            for (int viewY = Math.Max(0, Height - _lines.Count); viewY < Height; viewY++)
            {
                char character = _characters[viewX, Height - viewY - 1 + _topTextRow];
                cells[viewX, viewY] = new ViewCellData(character == '\0' ? null : (character, TextColor),
                    BackgroundColor);
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