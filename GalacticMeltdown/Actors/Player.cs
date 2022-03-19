using System;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Actors;

public class Player : Actor, ISightedObject, IControllable
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDex = 16;
    private const int PlayerDef = 4;
    private const int PlayerViewRange = 20;
    private const int PlayerStr = 10;

    private Action _giveControlToUser;
    
    private bool _xray;

    private int _strength;

    public int Strength
    {
        get => _strength;
        set
        {
            if (value == _strength) return;
            _strength = value;
            FireStatAffected(Stat.Strength);
        }
    }
    
    public bool NoClip;

    public bool InFocus { get; set; }

    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int ViewRange
    {
        get => _viewRange;
        set
        {
            if (value <= 0) return;
            _viewRange = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler VisiblePointsChanged;
    
    public Player(int x, int y, Level level)
        : base(PlayerHp, PlayerEnergy, PlayerDex, PlayerDef, PlayerViewRange, x, y, level)
    {
        SymbolData = ('@', ConsoleColor.White);
        BgColor = null;
        _strength = PlayerStr;
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        Tile tile = Level.GetTile(X + deltaX, Y + deltaY);
        if (Level.GetNonTileObject(X + deltaX, Y + deltaY) is not null)
            return false;
        if (!NoClip && (tile is null || !tile.IsWalkable))
            return Level.InteractWithDoor(X + deltaX, Y + deltaY, this);
        MoveTo(X + deltaX, Y + deltaY);
        VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override void TakeAction()
    {
        _giveControlToUser?.Invoke();
    }

    public void SetControlFunc(Action controlFunc)
    {
        _giveControlToUser = controlFunc;
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}