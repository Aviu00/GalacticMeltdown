using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public abstract class Actor : IObjectOnMap
{
    private bool _turnStopped;

    protected int _viewRange;

    public bool IsActive => Hp > 0 && Energy > 0 && !_turnStopped;

    private readonly LimitedNumber _hpLim;
    private readonly LimitedNumber _energyLim;
    private int _dex;
    private int _def;

    protected int Hp
    {
        get => _hpLim.Value;
        set
        {
            if (value == _hpLim.Value) return;
            _hpLim.Value = value;
            if (value == 0) Died?.Invoke(this, EventArgs.Empty);
            FireStatAffected(Stat.Hp);
        }
    }

    public int Energy
    {
        get => _energyLim.Value;
        set
        {
            if (value == _energyLim.Value) return;
            if (value < _energyLim.Value) SpentEnergy?.Invoke(this, EventArgs.Empty);
            _energyLim.Value = value;
            FireStatAffected(Stat.Energy);
        }
    }

    public int Dex
    {
        get => _dex;
        protected set
        {
            if (value == _dex) return;
            _dex = value;
            FireStatAffected(Stat.Dex);
        } 
    }

    public int Def
    {
        get => _def;
        protected set
        {
            if (value == _def) return;
            _def = value;
            FireStatAffected(Stat.Def);
        }
    }

    public int X { get; private set; }
    public int Y { get; private set; }

    public virtual (char symbol, ConsoleColor color) SymbolData { get; protected init; }
    public virtual ConsoleColor? BgColor { get; protected init; }

    public Level Level { get; }

    public event EventHandler Died;
    public event EventHandler SpentEnergy;
    public event EventHandler<StatChangeEventArgs> StatChanged; 
    public event EventHandler InvolvedInTurn;
    public event EventHandler<MoveEventArgs> Moved;

    protected Actor(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level)
    {
        Level = level;
        _hpLim = new LimitedNumber(maxHp, maxHp, 0);
        _energyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        _viewRange = viewRange;
        X = x;
        Y = y;
        _turnStopped = false;
    }

    public virtual void Hit(Actor hitter, int damage)
    {
        Hp -= damage;
        SendAffected();
    }

    public virtual void FinishTurn()
    {
        if (Hp == 0) return;
        _turnStopped = false;
        Energy += _energyLim.MaxValue.Value;
    }

    public void StopTurn() => _turnStopped = true;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Tile tile = Level.GetTile(x, y);
        if (tile is not null) Energy -= tile.MoveCost;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    protected void SendAffected() => InvolvedInTurn?.Invoke(this, EventArgs.Empty);

    protected void FireStatAffected(Stat stat)
    {
        StatChanged?.Invoke(this, new StatChangeEventArgs(stat));
    }

    public abstract void TakeAction();
}