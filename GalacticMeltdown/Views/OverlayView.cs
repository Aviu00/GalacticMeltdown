using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class OverlayView : View
{
    private Level _level; // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player; // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public OverlayView(Level level)
    {
        _level = level;
        _player = level.Player;
        Width = 0;
        Height = 0;
        // TODO: subscribe to events
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }
}