using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.Cursor;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Views;

public partial class LevelView
{
    private HashSet<(int x, int y)> _cursorLinePoints = new();
    private Cursor _cursor;

    private IFocusable _prevFocus;

    [JsonIgnore]
    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(_focusObject.X, _focusObject.Y, GetCursorStartCoords, () => ViewBounds, this);
            _cursor.Moved += CursorMoveHandler;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
            return _cursor;
        }
    }

    private bool _drawCursorLine;

    [JsonIgnore]
    private bool DrawCursorLine
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
        if (ReferenceEquals(_cursor, _focusObject)) ToggleCursorFocus();
        _drawCursorLine = false;
        _cursorLinePoints.Clear();
        _cursor.Moved -= CursorMoveHandler;
        _cursor = null;
        _prevFocus = null;
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
        if (!ReferenceEquals(_focusObject, _cursor)) NeedRedraw?.Invoke(this, EventArgs.Empty);
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
        return _prevFocus is not null ? (_prevFocus.X, _prevFocus.Y) : (_focusObject.X, _focusObject.Y);
    }

    public void ToggleCursorLine()
    {
        _drawCursorLine = !_drawCursorLine;
        _cursorLinePoints.Clear();
        if (_drawCursorLine) CalculateCursorLinePoints();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleCursorFocus()
    {
        if (_cursor is null) return;
        if (!ReferenceEquals(_cursor, _focusObject))
        {
            _prevFocus = _focusObject;
            SetFocus(_cursor);
        }
        else
        {
            SetFocus(_prevFocus);
            _prevFocus = null;
            _cursor.MoveInbounds();
        }
    }
}