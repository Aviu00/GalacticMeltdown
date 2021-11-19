using System;

namespace GalacticMeltdown
{
    public abstract class GameObject
    {
        public char Symbol { get; set; }
        public ConsoleColor Color = ConsoleColor.White;
        //public bool Drawable = true;
    }
}