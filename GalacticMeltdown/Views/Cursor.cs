using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class Cursor : IControllable
{
    public (char symbol, ConsoleColor color) SymbolData { get; }
    public ConsoleColor? BgColor { get; }
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public event EventHandler<MoveEventArgs> Moved;
    
    public bool InFocus { get; set; }
    
    public bool TryMove(int deltaX, int deltaY)
    {
        if (true)
        {
            MoveTo(X + deltaX, Y + deltaY);
            return true;
        }
        return false;
    }

    private void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }
}