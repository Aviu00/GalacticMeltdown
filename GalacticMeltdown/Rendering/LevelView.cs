using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.Rendering;

public class LevelView : View
{
    private Level _level;
    private IFocusable _focusObject;
    private LinkedList<ISightedObject> _tileRevealingObjects;
    private HashSet<(int, int)> _visiblePoints;
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;

    public LevelView(Level level)
    {
        _level = level;
        _tileRevealingObjects = new LinkedList<ISightedObject>();
    }

    private void UpdateVisiblePoints()
    {
        _visiblePoints = _tileRevealingObjects
            .SelectMany((obj, _) => _level.GetPointsVisibleAround(obj.X, obj.Y, obj.ViewRadius, obj.Xray))
            .ToHashSet();
        foreach (var (x, y) in _visiblePoints)
        {
            var tile = _level.GetTile(x, y);
            if (tile is not null) tile.Seen = true;
        }
        NeedRedraw?.Invoke(this);
    }

    public void AddTileRevealingObject(ISightedObject obj)
    {
        obj.VisiblePointsChanged += UpdateVisiblePoints;
        _tileRevealingObjects.AddFirst(obj);
        UpdateVisiblePoints();
    }

    public void RemoveTileRevealingObject(ISightedObject obj)
    {
        obj.VisiblePointsChanged -= UpdateVisiblePoints;
        _tileRevealingObjects.Remove(obj);
        UpdateVisiblePoints();
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject)) return;
        if (_focusObject is not null) _focusObject.InFocus = false;
        _focusObject = focusObj;
        _focusObject.InFocus = true;
        _focusObject.PositionChanged += FocusObjectMoved;
        NeedRedraw?.Invoke(this);
    }

    private void FocusObjectMoved()
    {
        // Already re-rendered
        if (_focusObject is ISightedObject focusObject && _tileRevealingObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this);
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        IDrawable drawableObj;
        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        // Draw object in focus on top of everything else
        if (x == centerScreenX && y == centerScreenY)
        {
            drawableObj = _focusObject;
            if (drawableObj is not null)
            {
                return new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor);
            }
        }
        var coords = Utility.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = Utility.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
        if (_visiblePoints.Contains(coords))
        {
            drawableObj = _level.GetDrawable(coords.x, coords.y);
            if (drawableObj is null)
                return new ViewCellData(null, null);
            return new ViewCellData(drawableObj.SymbolData, drawableObj.BgColor);
        }
        drawableObj = _level.GetTile(coords.x, coords.y);
        if (drawableObj is not null && ((Tile) drawableObj).Seen)
        {
            return new ViewCellData((drawableObj.SymbolData.symbol, 
                DataHolder.Colors.OutOfVisionTileColor), drawableObj.BgColor);
        }

        return new ViewCellData(null, null);
    }
}