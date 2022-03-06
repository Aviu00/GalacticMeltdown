using System;

namespace GalacticMeltdown;

public interface IDrawable
{
    (char symbol, ConsoleColor color) SymbolData { get; }
    ConsoleColor? BgColor { get; }
}

public interface IHasCoords
{
    int X { get; }
    int Y { get; }
}

public interface ISightedObject : IHasCoords
{
    int ViewRadius { get; set; }
    bool Xray { get; set; }
    event VisiblePointsChangedEventHandler VisiblePointsChanged;
}

public interface IMovable : IHasCoords
{
    event PositionChangedEventHandler PositionChanged;
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
    bool TryMove(int deltaX, int deltaY);
}

public delegate void VisiblePointsChangedEventHandler();

public delegate void PositionChangedEventHandler();
