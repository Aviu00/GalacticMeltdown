using System;

namespace GalacticMeltdown
{
    public interface IDrawable
    {
        public char Symbol { get; }
        public ConsoleColor FgColor { get; }
        public ConsoleColor BgColor { get; }
    }

    public interface IMovable
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public interface IEntity : IDrawable, IMovable //Entity is an object that can be placed on a tile
    {
        
    }
}