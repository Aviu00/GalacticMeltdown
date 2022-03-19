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

    protected readonly LimitedNumber HpLim;
    protected readonly LimitedNumber EnergyLim;
    private int _dexterity;
    private int _defence;

    public int Hp
    {
        get => HpLim.Value;
        protected set
        {
            if (value == HpLim.Value) return;
            HpLim.Value = value;
            if (value == 0) Died?.Invoke(this, EventArgs.Empty);
            FireStatAffected(Stat.Hp);
        }
    }

    public int Energy
    {
        get => EnergyLim.Value;
        set
        {
            if (value == EnergyLim.Value) return;
            if (value < EnergyLim.Value) SpentEnergy?.Invoke(this, EventArgs.Empty);
            EnergyLim.Value = value;
            FireStatAffected(Stat.Energy);
        }
    }

    public int Dexterity
    {
        get => _dexterity;
        protected set
        {
            if (value == _dexterity) return;
            _dexterity = value;
            FireStatAffected(Stat.Dexterity);
        } 
    }

    public int Defence
    {
        get => _defence;
        protected set
        {
            if (value == _defence) return;
            _defence = value;
            FireStatAffected(Stat.Defence);
        }
    }

    public int MaxHp => (int) HpLim.MaxValue!;
    public int MaxEnergy => (int) EnergyLim.MaxValue!;

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

    protected Actor(int maxHp, int maxEnergy, int dexterity, int defence, int viewRange, int x, int y, Level level)
    {
        Level = level;
        HpLim = new LimitedNumber(maxHp, maxHp, 0);
        EnergyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dexterity = dexterity;
        Defence = defence;
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
        Energy += EnergyLim.MaxValue.Value;
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