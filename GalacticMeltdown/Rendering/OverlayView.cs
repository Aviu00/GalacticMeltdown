using System;

namespace GalacticMeltdown.Rendering;

public class OverlayView : View
{
    private Map _map;  // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player;  // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?
    private ViewCellData[,] _symbols;
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;

    public OverlayView(Map map)
    {
        _map = map;
        _player = map.Player;
        Width = 0;
        Height = 0;
        // TODO: subscribe to events
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (!(0 <= x && x < Width && 0 <= y && y < Height)) return new ViewCellData(null, null);
        return _symbols[x, y];
    }

    public override void Resize(int width, int height)
    {
        int oldWidth = Width, oldHeight = Height;
        base.Resize(width, height);
        if (oldWidth == Width && oldHeight == Height) return;
        _symbols = new ViewCellData[Width, Height];
        
    }
}