using System;

namespace GalacticMeltdown;

public interface IDrawable
{
    public (char symbol, ConsoleColor color) SymbolData { get; }
    public ConsoleColor? BgColor { get; }
}

public interface IHasCoords
{
    public int X { get; set; }
    public int Y { get; set; }
}

public interface ISightedObject : IHasCoords
{
    int ViewRadius { get; set; }
    bool Xray { get; set; }
    public event VisiblePointsChangedEventHandler VisiblePointsChanged;
}

public interface IFocusPoint : IHasCoords
{
    public event PositionChangedEventHandler PositionChanged;
}

public interface IEntity : IDrawable, IHasCoords //Entity is an object that can be placed on a tile
{
        
}

public interface IControllable : IHasCoords
{
    public bool TryMove(int deltaX, int deltaY);
}

public delegate void VisiblePointsChangedEventHandler();

public delegate void PositionChangedEventHandler();
