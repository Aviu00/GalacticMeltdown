using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

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
            _cursor.Moved += CursorMoveHandler;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
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
            if (_drawCursorLine) CalculateCursorLinePoints();
        }
    }

    public void RemoveCursor()
    {
        if (_cursor.InFocus) return;
        DrawCursorLine = false;
        _cursor.Moved -= CursorMoveHandler;
        _cursor = null;
    }

    private void CalculateCursorLinePoints()
    {
        _cursorLinePoints = Algorithms.BresenhamGetPointsOnLine(_focusObject.X, _focusObject.Y, Cursor.X, Cursor.Y)
            .TakeWhile(point =>
            {
                Tile tile = _level.GetTile(point.x, point.y);
                return tile is null || tile.IsWalkable;
            })
            .ToHashSet();
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

    private void CursorMoveHandler(object sender, MoveEventArgs _)
    {
        if (_drawCursorLine) CalculateCursorLinePoints();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}