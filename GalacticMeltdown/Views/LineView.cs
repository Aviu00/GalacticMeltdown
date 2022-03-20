using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Events;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Rendering;

namespace GalacticMeltdown.Views;

public class LineView : View
{
    private List<ListLine> _lines;
    private List<int> _pressableLineIndexes;
    private int _selectedIndex;
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public override (double, double, double, double)? WantedPosition => null;

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _lines.Count) return new ViewCellData(null, null);
        return _pressableLineIndexes[_selectedIndex] < Height
            ? _lines[Height - y - 1][x]
            : _lines[_pressableLineIndexes[_selectedIndex] - y][x];
    }

    public void SetLines(List<ListLine> lines)
    {
        _lines = lines;
        
        _pressableLineIndexes = new List<int>(_lines.Count);
        for (var i = 0; i < _lines.Count; i++)
        {
            if (_lines[i] is PressableListLine)
            {
                _pressableLineIndexes.Add(i);
                UserInterface.AddChild(this, _lines[i]);
                if (lines[i] is InputLine inputLine) inputLine.Updated += OnInputLineUpdate;
            }
        }

        if (_pressableLineIndexes.Any())
        {
            _selectedIndex = 0;
            ((PressableListLine) _lines[_selectedIndex]).Select();
        }
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
        ((PressableListLine) _lines[prevIndex]).Deselect();
        ((PressableListLine) _lines[_selectedIndex]).Select();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void SelectPrev()
    {
        if (!_pressableLineIndexes.Any()) return;
        int prevIndex = _selectedIndex;
        _selectedIndex -= 1;
        if (_selectedIndex == -1) _selectedIndex = _pressableLineIndexes.Count - 1;
        if (prevIndex == _selectedIndex) return;
        ((PressableListLine) _lines[prevIndex]).Deselect();
        ((PressableListLine) _lines[_selectedIndex]).Select();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
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

    private void OnInputLineUpdate(object sender, EventArgs e)
    {
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}