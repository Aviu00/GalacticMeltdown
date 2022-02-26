using System;

namespace GalacticMeltdown;

public class Player : IEntity, IControllable, ICanSeeTiles, IFocusPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    
    private int Energy;
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
    private int _viewRadius = 15;
    private Func<int, int, Tile> _tileAt;
    private Func<int, int, IEntity> _entityAt;
    private bool _xray;

    public bool NoClip;//Temporary implementation of debugging "cheat codes"

    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            VisiblePointsChanged?.Invoke();
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
                VisiblePointsChanged?.Invoke();
            }
        }
    }
    
    public event VisiblePointsChangedEventHandler VisiblePointsChanged;
    public delegate void PerformedActionEventHandler(int movePoints);

    public event PerformedActionEventHandler PerformedAction;
    public event PositionChangedEventHandler PositionChanged;
    public event PositionChangedEventHandler PositionChangedForEnemy;

    public bool TryMove(int deltaX, int deltaY)
    {
        var tile = _tileAt(X + deltaX, Y + deltaY);
        if (!(NoClip || (tile is null || tile.IsWalkable) && _entityAt(X + deltaX, Y + deltaY) is null)) return false;
        X += deltaX;
        Y += deltaY;
        Energy -= tile.TileMoveCost;
        if (Energy - tile.TileMoveCost < 0)
        {
            Energy = 40;
            PerformedAction?.Invoke(20);
        }
        PositionChanged?.Invoke();
        VisiblePointsChanged?.Invoke();
        PositionChangedForEnemy?.Invoke();
        return true;
    }

    public Player(int x, int y, Func<int, int, Tile> tileAt, Func<int, int, IEntity> entityAt)
    {
        X = x;
        Y = y;
        Energy = 40;
        _tileAt = tileAt;
        _entityAt = entityAt;
        Symbol = '@';
        FgColor = ConsoleColor.White;
        BgColor = ConsoleColor.Black;
    }
}
