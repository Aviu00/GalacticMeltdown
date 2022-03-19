using System;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.LevelRelated;

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
    int ViewRange { get; }
    bool Xray { get; }
    event EventHandler VisiblePointsChanged;
}

public interface IMovable : IHasCoords
{
    event EventHandler<MoveEventArgs> Moved;
}

public interface IObjectOnMap : IHasCoords, IDrawable
{
}

public interface IFocusable : IMovable
{
    bool InFocus { get; set; }
}

public interface IControllable : IFocusable
{
    bool TryMove(int deltaX, int deltaY);
}