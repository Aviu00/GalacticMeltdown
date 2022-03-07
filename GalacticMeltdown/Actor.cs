using System;

namespace GalacticMeltdown;

public abstract class Actor : IObjectOnMap
{
    protected bool Dead;
    protected LimitedNumber HpLim;

    public int Hp
    {
        get => HpLim.Value;
        protected set
        {
            HpLim.Value = value;
            if (value <= 0) Die();
        }
    }

    protected LimitedNumber EnergyLim;

    public int Energy
    {
        get => EnergyLim.Value;
        protected set
        {
            EnergyLim.Value = value;
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

    public event EventHandler Died;
    public event EventHandler RanOutOfEnergy;
    public event EventHandler Stopped;
    public event EventHandler Affected;
    public event EventHandler<MoveEventArgs> Moved;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    public void StopTurn() => Stopped?.Invoke(this, EventArgs.Empty);

    public void Die()
    {
        Dead = true;
        Died?.Invoke(this, EventArgs.Empty);
    }

    public void OutOfEnergy() => RanOutOfEnergy?.Invoke(this, EventArgs.Empty);

    public void SendAffected() => Affected?.Invoke(this, EventArgs.Empty);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level)
    {
        Level = level;
        HpLim = new LimitedNumber(maxHp, maxHp);
        EnergyLim = new LimitedNumber(maxEnergy, maxEnergy);
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
        Energy += EnergyLim.MaxValue;
    }
}