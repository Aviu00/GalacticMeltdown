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
    private int _pressableIndex;
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public override (double, double, double, double)? WantedPosition => null;

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y < Height - _lines.Count) return new ViewCellData(null, null);
        return new ViewCellData(null, null);
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
            _pressableIndex = 0;
            ((PressableListLine) _lines[_pressableIndex]).Select();
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
        int prevIndex = _pressableIndex;
        _pressableIndex += 1;
        if (_pressableIndex == _pressableLineIndexes.Count) _pressableIndex = 0;
        if (prevIndex == _pressableIndex) return;
        ((PressableListLine) _lines[prevIndex]).Deselect();
        ((PressableListLine) _lines[_pressableIndex]).Select();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void SelectPrev()
    {
        if (!_pressableLineIndexes.Any()) return;
        int prevIndex = _pressableIndex;
        _pressableIndex -= 1;
        if (_pressableIndex == -1) _pressableIndex = _pressableLineIndexes.Count - 1;
        if (prevIndex == _pressableIndex) return;
        ((PressableListLine) _lines[prevIndex]).Deselect();
        ((PressableListLine) _lines[_pressableIndex]).Select();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void PressCurrent()
    {
        if (!_pressableLineIndexes.Any()) return;
        ((PressableListLine) _lines[_pressableLineIndexes[_pressableIndex]]).Press();
    }

    private void OnInputLineUpdate(object sender, EventArgs e)
    {
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}