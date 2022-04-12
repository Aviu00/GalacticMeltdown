using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.UserInterfaceRelated.Cursor;
using Newtonsoft.Json;

namespace GalacticMeltdown.Views;

public partial class LevelView
{
    [JsonIgnore] private Cursor _cursor;

    [JsonIgnore] private bool _focusOnCursor;
    [JsonIgnore] private bool _blockRedrawOnCursorMove;

    [JsonIgnore]
    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(FocusObject.X, FocusObject.Y, this);
            _cursor.Moved += OnCursorMove;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
            return _cursor;
        }
    }

    public void RemoveCursor()
    {
        if (ReferenceEquals(_cursor, FocusObject)) ToggleCursorFocus();
        _focusOnCursor = false;
        _cursor.Moved -= OnCursorMove;
        _cursor = null;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void OnCursorMove(object sender, MoveEventArgs _)
    {
        if (!_blockRedrawOnCursorMove) NeedRedraw?.Invoke(this, EventArgs.Empty);
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
        Cursor.ToggleLine();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleCursorFocus()
    {
        _focusOnCursor = !_focusOnCursor;
        _blockRedrawOnCursorMove = true;
        _cursor.MoveInbounds();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
        _blockRedrawOnCursorMove = false;
    }
}