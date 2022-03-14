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

    public ConsoleColor Color => DataHolder.Colors.CursorColor;
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public bool InFocus { get; set; }
    
    public Action<int, int, int, int> Action { private get; set; }
    
    public event EventHandler<MoveEventArgs> Moved;

    public Cursor(int x, int y, Func<(int, int)> getStartCoords)
    {
        X = x;
        Y = y;
        _getStartCoords = getStartCoords;
    }
    
    public bool TryMove(int deltaX, int deltaY)
    {
        int newX = X + deltaX, newY = Y + deltaY;
        if (!(newX >= _minX && newX <= _maxX && newY >= _minY && newY <= _maxY)) return false;
        MoveTo(newX, newY);
        return true;
    }

    public void Interact()
    {
        var (x0, y0) = _getStartCoords();
        Action?.Invoke(x0, y0, X, Y);
    }

    public void SetBounds(int minX, int minY, int maxX, int maxY)
    {
        _minX = minX;
        _minY = minY;
        _maxX = maxX;
        _maxY = maxY;
        MoveTo(Math.Min(Math.Max(X, _minX), _maxX), Math.Min(Math.Max(Y, _minY), _maxY));
    }

    private void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }
}