using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.Rendering;

public class LevelView : View
{
    private Level _level;
    private IFocusable _focusObject;
    private ObservableCollection<ISightedObject> _sightedObjects;
    private HashSet<(int, int)> _visiblePoints;
    private (char symbol, ConsoleColor color)?[,] _seenCells;
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;

    public LevelView(Level level)
    {
        _level = level;
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

    private void SightedObjectUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null) foreach (var sightedObject in e.NewItems)
        {
            ((ISightedObject) sightedObject).VisiblePointsChanged += UpdateVisiblePoints;
        }
        if (e.OldItems is not null) foreach (var sightedObject in e.OldItems)
        {
            ((ISightedObject) sightedObject).VisiblePointsChanged -= UpdateVisiblePoints;
        }
        UpdateVisiblePoints();
    }

    private bool Inbounds(int x, int y)
    {
        return x >= 0 && x < _seenCells.GetLength(0) && y >= 0 && y < _seenCells.GetLength(1);
    }

    private void UpdateVisiblePoints()
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
        NeedRedraw?.Invoke(this);
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject)) return;
        if (_focusObject is not null) _focusObject.InFocus = false;
        _focusObject = focusObj;
        _focusObject.InFocus = true;
        _focusObject.Moved += FocusObjectMoved;
        NeedRedraw?.Invoke(this);
    }

    private void FocusObjectMoved(object sender, int x0, int y0, int x1, int y1)
    {
        // Redraw happens on visible tile calculation already
        if (_focusObject is ISightedObject focusObject && _sightedObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this);
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        // Draw object in focus on top of everything else
        if (x == centerScreenX && y == centerScreenY) 
            return new ViewCellData(_focusObject.SymbolData, _focusObject.BgColor);
        
        var coords = Utility.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = Utility.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
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