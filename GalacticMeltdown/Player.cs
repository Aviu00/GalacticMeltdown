using System;

namespace GalacticMeltdown;

public class Player : Actor, IControllable
{
    public override (char symbol, ConsoleColor color) SymbolData { get; }
    public override ConsoleColor? BgColor { get; }
    
    private int _viewRadius = 15;
    private bool _xray;
    
    public bool InFocus { get; set; }
    
    private Func<int, int, Tile> _tileAt;
    private Func<int, int, IEntity> _entityAt;

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

    public bool TryMove(int deltaX, int deltaY)
    {
        var tile = _tileAt(X + deltaX, Y + deltaY);
        if (!(NoClip || (tile is null || tile.IsWalkable) && _entityAt(X + deltaX, Y + deltaY) is null)) return false;
        X += deltaX;
        Y += deltaY;
        VisiblePointsChanged?.Invoke();
        PositionChanged?.Invoke();
        PerformedAction?.Invoke(100);
        return true;
    }

    public Player(int maxHp, int maxEnergy, int dex, int def, int x, int y, Func<int, int, Tile> tileAt,
        Func<int, int, IEntity> entityAt) : base(maxHp, maxEnergy, dex, def, x, y)
    {
        _tileAt = tileAt;
        _entityAt = entityAt;
        SymbolData = ('@', ConsoleColor.White);
        BgColor = null;
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}
