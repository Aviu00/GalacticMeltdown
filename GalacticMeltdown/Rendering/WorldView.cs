using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class WorldView : View
{
    private Map _map;
    private IHasCoords _focusObject;
    private List<ICanSeeTiles> _tileRevealingObjects;
    private HashSet<(int, int)> _visiblePoints;

    public WorldView(Map map)
    {
        _map = map;
    }

    public void AddTileRevealingObject(ICanSeeTiles obj)
    {
        _tileRevealingObjects.Add(obj);
    }

    public void SetFocus(IHasCoords focusObj)
    {
        _focusObject = focusObj;
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
        drawableObj = _map.GetTile(x, y);
        if (drawableObj is not null && ((Tile) drawableObj).Seen)
        {
            return new SymbolData(drawableObj.Symbol, Utility.OutOfVisionTileColor, ConsoleColor.Black);
        }

        return new SymbolData(' ', ConsoleColor.Black, ConsoleColor.Black);
    }
}