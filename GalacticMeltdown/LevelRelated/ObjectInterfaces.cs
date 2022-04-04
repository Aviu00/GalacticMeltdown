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
    int GetViewRange();
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
}

public interface IControllable : IFocusable
{
    bool TryMove(int deltaX, int deltaY);
}