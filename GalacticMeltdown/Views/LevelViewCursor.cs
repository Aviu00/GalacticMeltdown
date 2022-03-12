using System.Collections.Generic;

namespace GalacticMeltdown.Views;

public partial class LevelView
{
    private HashSet<(int, int)> _cursorLinePoints = new();
    private Cursor _cursor;

    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(_focusObject.X, _focusObject.Y);
            SetCursorBounds();
            return _cursor;
        }
        private set => _cursor = value;
    }
    
    private bool _drawCursorLine;

    public bool DrawCursorLine
    {
        get => _drawCursorLine;
        set
        {
            _drawCursorLine = value;
            _cursorLinePoints.Clear();
        }
    }

    public void RemoveCursor()
    {
        if (_cursor.InFocus) return;
        DrawCursorLine = false;
        Cursor = null;
    }
    
    private void SetCursorBounds()
    {
        if (_cursor is null) return;
        int minX, minY, maxX, maxY;
        if (_cursor.InFocus)
        {
            minX = -1;
            minY = -1;
            (maxX, maxY) = _level.Size;
        }
        else
        {
            (minX, minY, maxX, maxY) = ViewBounds;
        }

        _cursor.SetBounds(minX, minY, maxX, maxY);
    }
}