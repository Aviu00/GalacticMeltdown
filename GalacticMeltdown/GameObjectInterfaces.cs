using System;

namespace GalacticMeltdown;

public interface IDrawable
{
    public (char symbol, ConsoleColor color) SymbolData { get; }
    public ConsoleColor? BgColor { get; }
}

public interface IHasCoords
{
    public int X { get; }
    public int Y { get; }
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

public interface IObjectOnMap : IHasCoords, IDrawable
{
}

public interface IFocusable : IMovable, IDrawable
{
    bool InFocus { get; set; }
}

public interface IControllable : IFocusable, ISightedObject
{
    public bool TryMove(int deltaX, int deltaY);
}

public delegate void VisiblePointsChangedEventHandler();

public delegate void PositionChangedEventHandler();
