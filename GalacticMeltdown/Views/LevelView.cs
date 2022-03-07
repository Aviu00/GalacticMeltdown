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

public class LevelView : View
{
    private readonly Level _level;
    private IFocusable _focusObject;
    private ObservableCollection<ISightedObject> _sightedObjects;
    private HashSet<(int, int)> _visiblePoints;
    private (char symbol, ConsoleColor color)?[,] _seenCells;
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
        _seenCells = new (char symbol, ConsoleColor color)?[width, height];
        UpdateVisiblePoints();
    }

    private (int screenX, int screenY) ToViewCoords(int xLevel, int yLevel)
    {
        return UtilityFunctions.ConvertAbsoluteToRelativeCoords(xLevel, yLevel, _focusObject.X, _focusObject.Y);
    }

    private void MoveHandler(object sender, MoveEventArgs e)
    {
        HashSet<(int, int, ViewCellData)> updated = new(2);
        if (_visiblePoints.Contains((e.X0, e.Y0)))
        {
            IDrawable drawableObj = _level.GetDrawable(e.X0, e.Y0);
            var (viewX, viewY) = ToViewCoords(e.X0, e.Y0);
            updated.Add((viewX, viewY,
                drawableObj is null
                    ? new ViewCellData(null, null)
                    : new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor)));
        }

        if (_visiblePoints.Contains((e.X1, e.Y1)))
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

    private void DeathHandler(object sender, EventArgs _)
    {
        if (sender is not Actor actor) return;

        if (!_visiblePoints.Contains((actor.X, actor.Y))) return;

        IDrawable drawableObj = _level.GetDrawable(actor.X, actor.Y);
        var (viewX, viewY) = ToViewCoords(actor.X, actor.Y);
        CellsChanged?.Invoke(this,
            new CellChangeEventArgs(new HashSet<(int, int, ViewCellData)>
            {
                (viewX, viewY, new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor))
            }));
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

    private bool Inbounds(int x, int y)
    {
        return x >= 0 && x < _seenCells.GetLength(0) && y >= 0 && y < _seenCells.GetLength(1);
    }

    private void UpdateVisiblePoints(object sender = null, EventArgs _ = null)
    {
        _visiblePoints = _sightedObjects
            .SelectMany((obj, _) => _level.GetPointsVisibleAround(obj.X, obj.Y, obj.ViewRadius, obj.Xray))
            .ToHashSet();
        foreach (var (x, y) in _visiblePoints)
        {
            if (!Inbounds(x, y)) continue;
            var tile = _level.GetTile(x, y);
            if (tile is not null) _seenCells[x, y] = tile.SymbolData;
        }

        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject)) return;
        if (_focusObject is not null) _focusObject.InFocus = false;
        _focusObject = focusObj;
        _focusObject.InFocus = true;
        _focusObject.Moved += FocusObjectMoved;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void FocusObjectMoved(object sender, MoveEventArgs _)
    {
        // Redraw happens on visible tile calculation already
        if (_focusObject is ISightedObject focusObject && _sightedObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        // Draw object in focus on top of everything else
        if (x == centerScreenX && y == centerScreenY)
            return new ViewCellData(_focusObject.SymbolData, _focusObject.BgColor);
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
        var (levelX, levelY) = coords;
        if (_visiblePoints.Contains(coords))
        {
            IDrawable drawableObj = _level.GetDrawable(levelX, levelY);
            return drawableObj is null
                ? new ViewCellData(null, null)
                : new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor);
        }

        if (Inbounds(levelX, levelY) && _seenCells[levelX, levelY] is not null)
            return new ViewCellData((_seenCells[levelX, levelY].Value.symbol, DataHolder.Colors.OutOfVisionTileColor),
                null);
        return new ViewCellData(null, null);
    }
}