using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class Cursor : IControllable
{
    private Func<(int, int)> _getStartCoords;
    
    private Func<(int, int, int, int)> _getViewBounds;
    private (int minX, int minY, int maxX, int maxY)? _levelBounds;

    public ConsoleColor Color => DataHolder.Colors.CursorColor;
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public bool InFocus { get; set; }
    
    public Action<int, int, int, int> Action { private get; set; }
    
    public event EventHandler<MoveEventArgs> Moved;

    public Cursor(int x, int y, Func<(int, int)> getStartCoords, Func<(int, int, int, int)> getViewBounds,
        (int minX, int minY, int maxX, int maxY)? levelBounds = null)
    {
        X = x;
        Y = y;
        _getStartCoords = getStartCoords;
        _getViewBounds = getViewBounds;
        _levelBounds = levelBounds;
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
    
    public void MoveInbounds()
    {
        var (minX, minY, maxX, maxY) = GetBounds();
        if (!IsPositionInbounds(X, Y)) 
            MoveTo(Math.Min(Math.Max(X, minX), maxX), Math.Min(Math.Max(Y, minY), maxY));
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
        var (minX, minY, maxX, maxY) = GetBounds();
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    private (int minX, int minY, int maxX, int maxY) GetBounds()
    {
        if (_levelBounds is null) return _getViewBounds();

        var (minXView, minYView, maxXView, maxYView) = _getViewBounds();
        var (minXWorld, minYWorld, maxXWorld, maxYWorld) = _levelBounds.Value;
        return (Math.Max(minXView, minXWorld), Math.Max(minYView, minYWorld), Math.Min(maxXView, maxXWorld),
            Math.Min(maxYView, maxYWorld));
    }
}