using System;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Actors;

public class Player : Actor, IControllable
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDex = 16;
    private const int PlayerDef = 4;

    private int _viewRadius = 15;
    private bool _xray;

    public bool InFocus { get; set; }

    public bool NoClip; //Temporary implementation of debugging "cheat codes"

    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int ViewRadius
    {
        get => _viewRadius;
        set
        {
            if (value <= 0) return;
            _viewRadius = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler VisiblePointsChanged;
    
    public Player(int x, int y, Level level) : base(PlayerHp, PlayerEnergy, PlayerDex, PlayerDef, x, y, level)
    {
        SymbolData = ('@', ConsoleColor.White);
        BgColor = null;
    }
    
    public void SetControlFunc(Action controlFunc)
    {
        DoAction = controlFunc;
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        Tile tile = Level.GetTile(X + deltaX, Y + deltaY);
        if (!(NoClip || (tile is null || tile.IsWalkable) && Level.GetNonTileObject(X + deltaX, Y + deltaY) is null))
            return false;

        MoveTo(X + deltaX, Y + deltaY);
        VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
    }
}