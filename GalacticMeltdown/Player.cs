using System;

namespace GalacticMeltdown;

public delegate void PerformedActionEventHandler(int movePoints);

public class Player : Actor, IControllable
{
    public override (char symbol, ConsoleColor color) SymbolData { get; }
    public override ConsoleColor? BgColor { get; }
    
    private int _viewRadius = 15;
    private bool _xray;
    
    public bool InFocus { get; set; }
    
    private Level _level;

    public bool NoClip;  //Temporary implementation of debugging "cheat codes"

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
    public event PerformedActionEventHandler PerformedAction;
    public event PositionChangedEventHandler PositionChanged;

    public bool TryMove(int deltaX, int deltaY)
    {
        var tile = _level.GetTile(X + deltaX, Y + deltaY);
        if (!(NoClip || (tile is null || tile.IsWalkable) && _level.GetEntity(X + deltaX, Y + deltaY) is null))
            return false;
        X += deltaX;
        Y += deltaY;
        VisiblePointsChanged?.Invoke();
        PositionChanged?.Invoke();
        PerformedAction?.Invoke(100);
        return true;
    }

    public Player(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y)
    {
        _level = level;
        SymbolData = ('@', ConsoleColor.White);
        BgColor = null;
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}
