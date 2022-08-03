using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Rendering;

namespace GalacticMeltdown.Views;

public class LineView : View, IFullRedraw, IMultiCellUpdate, ILineUpdate
{
    private const ConsoleColor DefaultBackgroundColor = Colors.DefaultMain;
    
    private List<ListLine> _lines;
    private List<int> _pressableLineIndexes;
    private int _selectedIndex;
    
    public event EventHandler NeedRedraw;
    public event EventHandler<MultiCellUpdateEventArgs> MultiCellUpdate;
    public event EventHandler<LineUpdateEventArgs> LineUpdate;

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _lines.Count) return new ViewCellData(null, DefaultBackgroundColor);
        return !_pressableLineIndexes.Any() || _pressableLineIndexes[_selectedIndex] < Height
            ? _lines[Height - y - 1][x]
            : _lines[_pressableLineIndexes[_selectedIndex] - y][x];
    }

    public override ViewCellData[,] GetAllCells()
    {
        ViewCellData[,] cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
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
                cells[viewX, viewY] = !_pressableLineIndexes.Any() || _pressableLineIndexes[_selectedIndex] < Height
                    ? _lines[Height - viewY - 1][viewX]
                    : _lines[_pressableLineIndexes[_selectedIndex] - viewY][viewX];
            }
        }
        return cells;
    }

    public void SetLines(List<ListLine> lines, bool keepSelected = false)
    {
        if (_lines is not null)
        {
            if (_pressableLineIndexes.Any())
            {
                ((PressableListLine) _lines[_pressableLineIndexes[_selectedIndex]]).Deselect();
            }
            foreach (var line in _lines.OfType<PressableListLine>())
            {
                UserInterface.Forget(line);
                if (line is InputLine inputLine) inputLine.Updated -= OnInputLineUpdate;
            }
        }
        
        _lines = lines;
        
        _pressableLineIndexes = new List<int>(_lines.Count);
        for (var i = 0; i < _lines.Count; i++)
        {
            if (_lines[i] is not PressableListLine) continue;
            _pressableLineIndexes.Add(i);
            UserInterface.AddChild(this, _lines[i]);
            if (lines[i] is InputLine inputLine) inputLine.Updated += OnInputLineUpdate;
        }

        if (_pressableLineIndexes.Any())
        {
            _selectedIndex = keepSelected ? Math.Min(_selectedIndex, _pressableLineIndexes.Count - 1) : 0;
            ((PressableListLine) _lines[_pressableLineIndexes[_selectedIndex]]).Select();
        }
        
        if (Width != 0) _lines.ForEach(line => line.SetWidth(Width));
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public override void Resize(int width, int height)
    {
        foreach (ListLine line in _lines) line.SetWidth(width);
        base.Resize(width, height);
    }

    public void SelectNext()
    {
        if (!_pressableLineIndexes.Any()) return;
        int prevIndex = _selectedIndex;
        _selectedIndex += 1;
        if (_selectedIndex == _pressableLineIndexes.Count) _selectedIndex = 0;
        if (prevIndex == _selectedIndex) return;
        int prevLineIndex = _pressableLineIndexes[prevIndex];
        int curLineIndex = _pressableLineIndexes[_selectedIndex];
        ((PressableListLine) _lines[prevLineIndex]).Deselect();
        ((PressableListLine) _lines[curLineIndex]).Select();
        if (prevLineIndex < Height && curLineIndex < Height)
        {
            LineUpdate?.Invoke(this, new LineUpdateEventArgs(Height - prevLineIndex - 1, GetLineCells(prevLineIndex)));
            LineUpdate?.Invoke(this, new LineUpdateEventArgs(Height - curLineIndex - 1, GetLineCells(curLineIndex)));
        }
        else
        {
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SelectPrev()
    {
        if (!_pressableLineIndexes.Any()) return;
        int prevIndex = _selectedIndex;
        _selectedIndex -= 1;
        if (_selectedIndex == -1) _selectedIndex = _pressableLineIndexes.Count - 1;
        if (prevIndex == _selectedIndex) return;
        int prevLineIndex = _pressableLineIndexes[prevIndex];
        int curLineIndex = _pressableLineIndexes[_selectedIndex];
        ((PressableListLine) _lines[prevLineIndex]).Deselect();
        ((PressableListLine) _lines[curLineIndex]).Select();
        if (prevLineIndex < Height && curLineIndex < Height)
        {
            LineUpdate?.Invoke(this, new LineUpdateEventArgs(Height - prevLineIndex - 1, GetLineCells(prevLineIndex)));
            LineUpdate?.Invoke(this, new LineUpdateEventArgs(Height - curLineIndex - 1, GetLineCells(curLineIndex)));
        }
        else
        {
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        }
    }

    public void PressCurrent()
    {
        if (!_pressableLineIndexes.Any()) return;
        ((PressableListLine) _lines[_pressableLineIndexes[_selectedIndex]]).Press();
    }

    public ListLine GetCurrent()
    {
        if (!_pressableLineIndexes.Any()) return null;
        return _lines[_pressableLineIndexes[_selectedIndex]];
    }

    private void OnInputLineUpdate(object sender, InputLineUpdateEventArgs e)
    {
        if (Height == 0 || Width == 0) return;
        int lineY = _pressableLineIndexes[_selectedIndex] < Height
            ? Height - _pressableLineIndexes[_selectedIndex] - 1
            : 0;
        if (e.Cells is not null)
        {
            MultiCellUpdate?.Invoke(this,
                new MultiCellUpdateEventArgs(e.Cells.Select(el => (el.x, lineY, el.cell)).ToList()));
            return;
        }

        var lineContents = GetLineCells(_pressableLineIndexes[_selectedIndex]);

        LineUpdate?.Invoke(this, new LineUpdateEventArgs(lineY, lineContents));
    }

    private List<ViewCellData> GetLineCells(int index)
    {
        var lineContents = new List<ViewCellData>();
        for (int i = 0; i < Width; i++)
            lineContents.Add(_lines[index][i]);

        return lineContents;
    }
}