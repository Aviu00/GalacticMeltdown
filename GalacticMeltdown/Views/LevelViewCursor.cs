using System;
using System.Collections.Generic;
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

    public Cursor GetCursor((int minX, int minY, int maxX, int maxY)? levelBounds = null)
    {
        if (_cursor is not null) return _cursor;
        _cursor = new Cursor(this, FocusObject.X, FocusObject.Y, levelBounds);
        _cursor.Moved += OnCursorMove;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
        return _cursor;
    }

    public void RemoveCursor()
    {
        if (ReferenceEquals(_cursor, FocusObject)) ToggleCursorFocus();
        _focusOnCursor = false;
        _cursor.Moved -= OnCursorMove;
        _cursor = null;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void OnCursorMove(object sender, MoveEventArgs e)
    {
        if (!_focusOnCursor && _cursor.LinePoints is null)
        {
            var (oldScreenX, oldScreenY) = ToViewCoords(e.OldX, e.OldY);
            var (newScreenX, newScreenY) = ToViewCoords(_cursor.X, _cursor.Y);
            MultiCellUpdate?.Invoke(this,
                new MultiCellUpdateEventArgs(new List<(int, int, ViewCellData)>
                {
                    (oldScreenX, oldScreenY, GetSymbol(oldScreenX, oldScreenY)),
                    (newScreenX, newScreenY, GetSymbol(newScreenX, newScreenY))
                }));
        }
        else
        {
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        }
    }

    private ConsoleColor? GetCursorLineColor(int x, int y)
    {
        if (!_visiblePoints.Contains((x, y))) return Colors.CursorBg.LineNoHighlight;
        
        if (_level.GetNonTileObject(x, y) is Npc) return Colors.CursorBg.LineEnemy;
        if (_level.GetItems(x, y) is not null) return Colors.CursorBg.LineItem;
        if (_level.GetTile(x, y) is {IsWalkable: false}) return Colors.CursorBg.LineNonWalkable;
        if (_level.Player.X == x && _level.Player.Y == y) return Colors.CursorBg.LineFriend;
        
        return Colors.CursorBg.LineNoHighlight;
    }

    public void ToggleCursorLine()
    {
        _cursor.ToggleLine();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleCursorFocus()
    {
        _focusOnCursor = !_focusOnCursor;
        _cursor.MoveInbounds();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        _cursor?.MoveInbounds();
    }
}