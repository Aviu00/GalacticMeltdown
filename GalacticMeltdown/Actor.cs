using System;

namespace GalacticMeltdown;

public delegate void DiedEventHandler(Actor sender);

public delegate void OutOfEnergyEventHandler(Actor sender);

public delegate void StoppedEventHandler(Actor sender);

public delegate void AffectedEventHandler(Actor sender);

public abstract class Actor : IObjectOnMap
{
    protected bool Dead;
    protected LimitedNumber _hp;

    public int Hp
    {
        get => _hp.Value;
        protected set
        {
            _hp.Value = value;
            if (value <= 0) Die();
        }
    }

    protected LimitedNumber _energy;

    public int Energy
    {
        get => _energy.Value;
        protected set
        {
            _energy.Value = value;
            if (Energy <= 0) OutOfEnergy();
        }
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; set; }
    public int Y { get; set; }

    public (char symbol, ConsoleColor color) SymbolData { get; protected init; }
    public ConsoleColor? BgColor { get; protected init; }

    public Action DoAction { get; protected set; }

    public Level Level { get; }

    public event DiedEventHandler Died;
    public event OutOfEnergyEventHandler RanOutOfEnergy;
    public event StoppedEventHandler Stopped;
    public event AffectedEventHandler Affected;
    public event MovedEventHandler Moved;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke((IMovable) this, oldX, oldY, X, Y);
    }

    public void StopTurn() => Stopped?.Invoke(this);

    public void Die()
    {
        Dead = true;
        Died?.Invoke(this);
    }

    public void OutOfEnergy() => RanOutOfEnergy?.Invoke(this);

    public void SendAffected() => Affected?.Invoke(this);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level)
    {
        Level = level;
        _hp = new LimitedNumber(maxHp, maxHp);
        _energy = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        X = x;
        Y = y;
        Dead = false;
    }

    public virtual void Hit(Actor hitter, int damage)
    {
        SendAffected();
    }

    public virtual void FinishTurn()
    {
        Energy += _energy.MaxValue;
    }
}