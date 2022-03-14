using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
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
            _cursor = new Cursor(_focusObject.X, _focusObject.Y, GetCursorStartCoords);
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
        var (x0, y0) = GetCursorStartCoords();
        _cursorLinePoints = Algorithms.BresenhamGetPointsOnLine(x0, y0, Cursor.X, Cursor.Y)
            .TakeWhile(point => _level.GetTile(point.x, point.y) is null or {IsWalkable: true})
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