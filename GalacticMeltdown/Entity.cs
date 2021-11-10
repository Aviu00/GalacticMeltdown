using System;

namespace GalacticMeltdown
{
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Color = ConsoleColor.White;
        //public bool Drawable = true;
    }
}