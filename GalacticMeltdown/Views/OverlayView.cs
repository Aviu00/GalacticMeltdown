using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class OverlayView : View
{
    private LevelRelated.Level _level; // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player; // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?
    
    private ViewCellData[,] _symbols;
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public OverlayView(LevelRelated.Level level)
    {
        _level = level;
        _player = level.Player;
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