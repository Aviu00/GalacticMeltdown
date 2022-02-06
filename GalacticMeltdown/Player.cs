using System;

namespace GalacticMeltdown;

public class Player : IEntity, IControllable, ICanSeeTiles
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
    private int _viewRadius = 15;
    private Func<int, int, Tile> _tileAt;
    private bool _xray;

    public bool NoClip;//Temporary implementation of debugging "cheat codes"

    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            ViewChanged?.Invoke();
        }
    }

    public int ViewRadius
    {
        get => _viewRadius;
        set
        {
            if (value > 0)
            {
                _viewRadius = value;
                ViewChanged?.Invoke();
            }
        }
    }
    
    public event ViewChangedEventHandler ViewChanged;
    public delegate void PerformedActionEventHandler(int movePoints);

    public event PerformedActionEventHandler PerformedAction;

    public bool TryMove(int deltaX, int deltaY)
    {
        if (!NoClip && !_tileAt(X + deltaX, Y + deltaY).IsWalkable) return false;
        X += deltaX;
        Y += deltaY;
        PerformedAction?.Invoke(100);
        ViewChanged?.Invoke();
        return true;
    }

    public Player(int x, int y, Func<int, int, Tile> tileAt)
    {
        X = x;
        Y = y;
        _tileAt = tileAt;
        Symbol = '@';
        FgColor = ConsoleColor.White;
        BgColor = ConsoleColor.Black;
    }
}
