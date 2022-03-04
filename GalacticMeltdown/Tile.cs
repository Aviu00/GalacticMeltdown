using System;
using GalacticMeltdown.Data;

namespace GalacticMeltdown;

public class Tile : IDrawable
{
    private TileTypeData _typeData;
    public string Name => _typeData.Name;
    public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public ConsoleColor? BgColor => null;
    public bool IsTransparent => _typeData.IsTransparent;
    public bool IsWalkable => _typeData.IsWalkable;

    public bool ConnectToWalls => _typeData.IsConnection;
    
    public bool Seen = false;

    public Tile(TileTypeData typeData)
    {
        _typeData = typeData;
    }
}