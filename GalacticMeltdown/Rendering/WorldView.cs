using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class WorldView : View
{
    private Map _map;
    private IHasCoords _focusObject;
    private int _viewRadius;
    private Dictionary<(int, int), IDrawable> _visibleObjects;
    
    public WorldView(Map map)
    {
        _map = map;
        _map.Updated += MapChange;
        _focusObject = map.Player;
        _viewRadius = map.Player.ViewRange;
        _visibleObjects = _map.GetObjectsVisibleAround(_focusObject.X, _focusObject.Y, _viewRadius);
    }

    public override SymbolData GetSymbol(int x, int y)
    {
        (x, y) = Utility.ConvertAbsoluteToRelativeCoords(x, y, Width / 2, Height / 2);
        (x, y) = Utility.ConvertRelativeToAbsoluteCoords(x, y, _focusObject.X, _focusObject.Y);
        IDrawable drawableObj;
        if (_visibleObjects.ContainsKey((x, y)))
        {
            drawableObj = _visibleObjects[(x, y)];
            return new SymbolData(drawableObj.Symbol, drawableObj.FgColor, drawableObj.BgColor);
        }

        drawableObj = _map.GetTile(x, y);
        if (drawableObj is not null && ((Tile) drawableObj).WasSeenByPlayer)
        {
            return new SymbolData(drawableObj.Symbol, Utility.OutOfVisionTileColor, ConsoleColor.Black);
        }

        return new SymbolData(' ', ConsoleColor.Black, ConsoleColor.Black);
    }

    private void MapChange()
    {
        _visibleObjects = _map.GetObjectsVisibleAround(_focusObject.X, _focusObject.Y, _viewRadius);
    }

    public void SetFocus(IHasCoords focusObject, int viewRadius)
    {
        _focusObject = focusObject;
        _viewRadius = viewRadius;
        _visibleObjects = _map.GetObjectsVisibleAround(_focusObject.X, _focusObject.Y, _viewRadius);
    }
}