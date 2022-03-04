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

public interface IMovable : IHasCoords
{
    public event PositionChangedEventHandler PositionChanged;
}

public interface IFocusable : IMovable, IDrawable
{
    
}

public interface IEntity : IDrawable, IHasCoords
{
        
}

public interface IControllable : IFocusable, ISightedObject
{
    public bool TryMove(int deltaX, int deltaY);
}

public delegate void VisiblePointsChangedEventHandler();

public delegate void PositionChangedEventHandler();
