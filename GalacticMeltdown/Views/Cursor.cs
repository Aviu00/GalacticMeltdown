using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class Cursor : IControllable
{
    public (char symbol, ConsoleColor color) SymbolData { get; }
    public ConsoleColor? BgColor { get; }
    
    public int X { get; }
    public int Y { get; }
    
    public event EventHandler<MoveEventArgs> Moved;
    
    public bool InFocus { get; set; }
    
    public bool TryMove(int deltaX, int deltaY)
    {
        return true;
    }
}