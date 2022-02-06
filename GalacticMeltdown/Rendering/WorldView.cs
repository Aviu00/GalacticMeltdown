using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown.Rendering;

public class WorldView : View
{
    private Map _map;
    private IFocusPoint _focusObject;
    private LinkedList<ICanSeeTiles> _tileRevealingObjects;
    private HashSet<(int, int)> _visiblePoints;
    public override event ViewChangedEventHandler ViewChanged;

    public WorldView(Map map)
    {
        _map = map;
        _tileRevealingObjects = new LinkedList<ICanSeeTiles>();
    }

    private void UpdateVisiblePoints()
    {
        _visiblePoints = _tileRevealingObjects
            .SelectMany((obj, _) => _map.GetPointsVisibleAround(obj.X, obj.Y, obj.ViewRadius, obj.Xray))
            .ToHashSet();
        foreach (var (x, y) in _visiblePoints)
        {
            var tile = _map.GetTile(x, y);
            if (tile is not null) tile.Seen = true;
        }
        ViewChanged?.Invoke(this);
    }

    public void AddTileRevealingObject(ICanSeeTiles obj)
    {
        obj.VisiblePointsChanged += UpdateVisiblePoints;
        _tileRevealingObjects.AddFirst(obj);
        UpdateVisiblePoints();
    }

    public void RemoveTileRevealingObject(ICanSeeTiles obj)
    {
        obj.VisiblePointsChanged -= UpdateVisiblePoints;
        _tileRevealingObjects.Remove(obj);
        UpdateVisiblePoints();
    }

    public void SetFocus(IFocusPoint focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject)) return;
        _focusObject = focusObj;
        _focusObject.PositionChanged += FocusObjectMoved;
        ViewChanged?.Invoke(this);
    }

    private void FocusObjectMoved()
    {
        // Already re-rendered
        if (_focusObject is ICanSeeTiles focusObject && _tileRevealingObjects.Contains(focusObject)) return;
        ViewChanged?.Invoke(this);
    }

    public override SymbolData GetSymbol(int x, int y)
    {
        var coords = Utility.ConvertAbsoluteToRelativeCoords(x, y, Width / 2, Height / 2);
        coords = Utility.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
        IDrawable drawableObj;
        if (_visiblePoints.Contains(coords))
        {
            drawableObj = _map.GetDrawable(coords.x, coords.y);
            return new SymbolData(drawableObj.Symbol, drawableObj.FgColor, drawableObj.BgColor);
        }
        drawableObj = _map.GetTile(coords.x, coords.y);
        if (drawableObj is not null && ((Tile) drawableObj).Seen)
        {
            return new SymbolData(drawableObj.Symbol, Utility.OutOfVisionTileColor, ConsoleColor.Black);
        }

        return new SymbolData(' ', ConsoleColor.Black, ConsoleColor.Black);
    }
}