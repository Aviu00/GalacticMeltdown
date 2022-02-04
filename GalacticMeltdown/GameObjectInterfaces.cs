using System;

namespace GalacticMeltdown;

public interface IDrawable
{
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
}

public interface IHasCoords
{
    public int X { get; set; }
    public int Y { get; set; }
}

public interface IEntity : IDrawable, IHasCoords //Entity is an object that can be placed on a tile
{
        
}

public interface IControllable : IHasCoords
{
    public bool TryMove(int deltaX, int deltaY);
}