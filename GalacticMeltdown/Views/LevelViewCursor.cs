using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

public partial class LevelView
{
    private HashSet<(int, int)> _cursorLinePoints = new();
    private Cursor _cursor;

    private (int, int)? _cursorStartCoords;

    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(_focusObject.X, _focusObject.Y, GetCursorStartCoords, IsPointInsideView);
            _cursor.Moved += CursorMoveHandler;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
            return _cursor;
        }
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
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveCursor()
    {
        if (_cursor.InFocus) return;
        DrawCursorLine = false;
        _cursor.Moved -= CursorMoveHandler;
        _cursor = null;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void CalculateCursorLinePoints()
    {
        var (x0, y0) = GetCursorStartCoords();
        _cursorLinePoints = Algorithms.BresenhamGetPointsOnLine(x0, y0, Cursor.X, Cursor.Y).ToHashSet();
    }

    private void CursorMoveHandler(object sender, MoveEventArgs _)
    {
        if (_drawCursorLine) CalculateCursorLinePoints();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private ConsoleColor? GetCursorLineColor(int x, int y)
    {
        if (!_visiblePoints.Contains((x, y))) return DataHolder.Colors.HighlightedNothingColor;
        
        if (_level.GetNonTileObject(x, y) is Enemy) return DataHolder.Colors.HighlightedEnemyColor;
        if (_level.GetTile(x, y) is {IsWalkable: false}) return DataHolder.Colors.HighlightedSolidTileColor;
        if (_level.Player.X == x && _level.Player.Y == y) return DataHolder.Colors.HighlightedFriendColor;
        
        return DataHolder.Colors.HighlightedNothingColor;
    }

    private (int, int) GetCursorStartCoords()
    {
        return _cursorStartCoords ?? (_focusObject.X, _focusObject.Y);
    }
}