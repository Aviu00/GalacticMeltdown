using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

internal class SeenTilesArray
{
    private const int Offset = 1;
    private char?[,] _array;

    public SeenTilesArray(int mapWidth, int mapHeight)
    {
        _array = new char?[mapWidth + Offset, mapHeight + Offset];
    }

    public char? this[int x, int y]
    {
        get => _array[x + Offset, y + Offset];
        set => _array[x + Offset, y + Offset] = value;
    }

    public bool Inbounds(int x, int y)
    {
        return x + Offset >= 0 && x + Offset < _array.GetLength(0)
            && y + Offset >= 0 && y + Offset < _array.GetLength(1);
    }
}

public partial class LevelView : View
{
    private readonly Level _level;

    private IFocusable _focusObject;

    private ObservableCollection<ISightedObject> _sightedObjects;
    private HashSet<(int, int)> _visiblePoints;
    private SeenTilesArray _seenCells;

    private (int minX, int minY, int maxX, int maxY) ViewBounds => 
        (_focusObject.X - Width / 2, _focusObject.Y - Height / 2,
            _focusObject.X + (Width - 1) / 2, _focusObject.Y + (Height - 1) / 2);

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public LevelView(Level level)
    {
        _level = level;
        _level.SomethingMoved += MoveHandler;
        _level.NpcDied += DeathHandler;
        _sightedObjects = _level.SightedObjects;
        _sightedObjects.CollectionChanged += SightedObjectUpdateHandler;
        foreach (var sightedObject in _sightedObjects)
        {
            sightedObject.VisiblePointsChanged += UpdateVisiblePoints;
        }

        var (width, height) = _level.Size;
        _seenCells = new SeenTilesArray(width, height);
        UpdateVisiblePoints();
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
        var (levelX, levelY) = coords;
        
        ConsoleColor? backgroundColor = null;
        if (DrawCursorLine && _cursorLinePoints.Contains(coords)) 
            backgroundColor = GetCursorLineColor(levelX, levelY);
        
        if (x == centerScreenX && y == centerScreenY)
        {
            if (_cursor is not null && _cursor.X == levelX && _cursor.Y == levelY) backgroundColor = _cursor.Color;
            // Draw object in focus on top of everything else
            if (_focusObject is IDrawable drawable)
                return new ViewCellData(drawable.SymbolData, backgroundColor ?? drawable.BgColor);
        }
        
        if (_visiblePoints.Contains(coords))
        {
            IDrawable drawableObj = _level.GetDrawable(levelX, levelY);
            return drawableObj is null
                ? new ViewCellData(null, null)
                : new ViewCellData(drawableObj.SymbolData, backgroundColor ?? drawableObj.BgColor);
        }

        if (_seenCells.Inbounds(levelX, levelY) && _seenCells[levelX, levelY] is not null)
            return new ViewCellData((_seenCells[levelX, levelY].Value, DataHolder.Colors.OutOfVisionTileColor),
                backgroundColor);
        
        return new ViewCellData(null, backgroundColor);
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject)) return;
        if (_focusObject is not null) _focusObject.InFocus = false;

        _focusObject = focusObj;
        _focusObject.InFocus = true;
        SetCursorBounds();
        _focusObject.Moved += FocusObjectMoved;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void MoveHandler(object sender, MoveEventArgs e)
    {
        HashSet<(int, int, ViewCellData)> updated = new(2);
        if (CanPlayerSeePoint(e.X0, e.Y0))
        {
            IDrawable drawableObj = _level.GetDrawable(e.X0, e.Y0);
            var (viewX, viewY) = ToViewCoords(e.X0, e.Y0);
            updated.Add((viewX, viewY,
                drawableObj is null
                    ? new ViewCellData(null, null)
                    : new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor)));
        }

        if (CanPlayerSeePoint(e.X1, e.Y1))
        {
            IDrawable drawableObj = _level.GetDrawable(e.X1, e.Y1);
            var (viewX, viewY) = ToViewCoords(e.X1, e.Y1);
            updated.Add((viewX, viewY,
                drawableObj is null
                    ? new ViewCellData(null, null)
                    : new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor)));
        }

        if (updated.Any()) CellsChanged?.Invoke(this, new CellChangeEventArgs(updated));
    }

    private void SightedObjectUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (var sightedObject in e.NewItems)
            {
                ((ISightedObject) sightedObject).VisiblePointsChanged += UpdateVisiblePoints;
            }

        if (e.OldItems is not null)
            foreach (var sightedObject in e.OldItems)
            {
                ((ISightedObject) sightedObject).VisiblePointsChanged -= UpdateVisiblePoints;
            }

        UpdateVisiblePoints();
    }

    private void UpdateVisiblePoints(object sender = null, EventArgs _ = null)
    {
        _visiblePoints = _sightedObjects
            .SelectMany((obj, _) => _level.GetPointsVisibleAround(obj.X, obj.Y, obj.ViewRange, obj.Xray))
            .ToHashSet();
        foreach (var (x, y) in _visiblePoints)
        {
            if (!_seenCells.Inbounds(x, y)) continue;
            var tile = _level.GetTile(x, y);
            if (tile is not null) _seenCells[x, y] = tile.SymbolData.symbol;
        }

        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void DeathHandler(object sender, EventArgs _)
    {
        if (sender is not Actor actor) return;

        if (!CanPlayerSeePoint(actor.X, actor.Y)) return;

        var (viewX, viewY) = ToViewCoords(actor.X, actor.Y);
        CellsChanged?.Invoke(this,
            new CellChangeEventArgs(new HashSet<(int, int, ViewCellData)> {(viewX, viewY, GetSymbol(viewX, viewY))}));
    }

    private void FocusObjectMoved(object sender, MoveEventArgs _)
    {
        SetCursorBounds();
        // Redraw happens on visible tile calculation already
        if (_focusObject is ISightedObject focusObject && _sightedObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private bool CanPlayerSeePoint(int x, int y)
    {
        return _visiblePoints.Contains((x, y)) && IsPointInsideView(x, y);
    }

    private bool IsPointInsideView(int x, int y)
    {
        var (minX, minY, maxX, maxY) = ViewBounds;
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    private (int screenX, int screenY) ToViewCoords(int xLevel, int yLevel)
    {
        var (minX, minY, _, _) = ViewBounds;
        return UtilityFunctions.ConvertAbsoluteToRelativeCoords(xLevel, yLevel, minX, minY);
    }
}