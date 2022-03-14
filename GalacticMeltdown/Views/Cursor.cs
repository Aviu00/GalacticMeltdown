using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class Cursor : IControllable
{
    private int _minX;
    private int _minY;
    private int _maxX;
    private int _maxY;

    private Func<(int, int)> _getStartCoords;
    private Func<(int, int, int, int)> _getBounds;

    public ConsoleColor Color => DataHolder.Colors.CursorColor;
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public bool InFocus { get; set; }
    
    public Action<int, int, int, int> Action { private get; set; }
    
    public event EventHandler<MoveEventArgs> Moved;

    public Cursor(int x, int y, Func<(int, int)> getStartCoords, Func<(int, int, int, int)> getBounds)
    {
        X = x;
        Y = y;
        _getStartCoords = getStartCoords;
        _getBounds = getBounds;
    }
    
    public bool TryMove(int deltaX, int deltaY)
    {
        int newX = X + deltaX, newY = Y + deltaY;
        if (!IsPositionInbounds(newX, newY)) return false;
        MoveTo(newX, newY);
        return true;
    }

    public void Interact()
    {
        var (x0, y0) = _getStartCoords();
        Action?.Invoke(x0, y0, X, Y);
    }

    private void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    private bool IsPositionInbounds(int x, int y)
    {
        var (minX, minY, maxX, maxY) = _getBounds();
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }
}