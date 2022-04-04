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

    private bool _focusOnCursor;

    [JsonIgnore]
    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(FocusObject.X, FocusObject.Y, this);
            _cursor.Moved += CursorMoveHandler;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
            return _cursor;
        }
    }

    [JsonIgnore] private bool _drawCursorLine;

    public void RemoveCursor()
    {
        if (ReferenceEquals(_cursor, FocusObject)) ToggleCursorFocus();
        _drawCursorLine = false;
        _focusOnCursor = false;
        _cursorLinePoints.Clear();
        _cursor.Moved -= CursorMoveHandler;
        _cursor = null;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void CalculateCursorLinePoints()
    {
        var (x0, y0) = (_focusedOn.X, _focusedOn.Y);
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

    public void ToggleCursorLine()
    {
        _drawCursorLine = !_drawCursorLine;
        _cursorLinePoints.Clear();
        if (_drawCursorLine) CalculateCursorLinePoints();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleCursorFocus()
    {
        _focusOnCursor = !_focusOnCursor;
        _cursor.MoveInbounds();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}