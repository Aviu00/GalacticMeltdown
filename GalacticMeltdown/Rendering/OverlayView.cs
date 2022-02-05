using System;

namespace GalacticMeltdown.Rendering;

public class OverlayView : View
{
    private Map _map;  // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player;  // State, Effects, (?) Event log, Enemy-in-sight indicator
    private SymbolData?[,] _symbols; 

    public OverlayView(Map map)
    {
        _map = map;
        _player = map.Player;
        _symbols = new SymbolData?[Width, Height];
        // TODO: subscribe to events
    }

    public override SymbolData GetSymbol(int x, int y)
    {
        if (_symbols[x, y] is null) return new SymbolData(' ', ConsoleColor.Black, ConsoleColor.Black);
        return (SymbolData) _symbols[x, y];
    }

    public override void Resize(int width, int height)
    {
        int oldWidth = Width, oldHeight = Height;
        base.Resize(width, height);
        if (oldWidth == Width && oldHeight == Height) return;
        _symbols = new SymbolData?[Width, Height];
        
    }
}