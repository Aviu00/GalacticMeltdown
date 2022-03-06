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
    int ViewRadius { get; }
    bool Xray { get; }
    event VisiblePointsChangedEventHandler VisiblePointsChanged;
}

public interface IMovable : IHasCoords
{
    event MovedEventHandler Moved;
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

public delegate void MovedEventHandler(IMovable sender, int x0, int y0, int x1, int y1);

