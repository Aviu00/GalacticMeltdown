using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

internal struct StatInfo
{
    public string Text;
    public int Val;
    public int? MaxVal;

    public StatInfo(string text, int val, int? maxVal = null)
    {
        Text = text;
        Val = val;
        MaxVal = maxVal;
    }
}

public class OverlayView : View
{
    private Level _level; // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player; // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?

    private StatInfo _hpInfo;
    private StatInfo _energyInfo;
    private StatInfo _dexInfo;
    private StatInfo _defInfo;
    private StatInfo _strInfo;
    
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
        if (y == Width - 1)
        {
            
        }
        return new ViewCellData(null, null);
    }
}