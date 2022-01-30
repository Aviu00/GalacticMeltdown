using System;

namespace GalacticMeltdown;

public abstract class GameObject
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; set; }
    public ConsoleColor FGColor = ConsoleColor.White;

    public ConsoleColor BGColor = ConsoleColor.Black;
    //public bool Drawable = true;
}