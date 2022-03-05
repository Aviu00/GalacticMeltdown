using System;

namespace GalacticMeltdown;

public delegate void DiedEventHandler(Actor sender);

public abstract class Actor : IEntity
{
    private LimitedNumber _hp;
    public int Hp
    {
        get => _hp.Value;
        protected set
        {
            _hp.Value = value;
            if (value <= 0) Died?.Invoke(this);
        }
    }

    private LimitedNumber _energy;
    public int Energy
    {
        get => _energy.Value;
        protected set => _energy.Value = value;
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; set; }
    public int Y { get; set; }
    
    public virtual (char symbol, ConsoleColor color) SymbolData { get; }
    public virtual ConsoleColor? BgColor { get; }

    public event DiedEventHandler Died;

    public abstract void Hit(Actor hitter, int damage);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y)
    {
        _hp = new LimitedNumber(maxHp, maxHp);
        _energy = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        X = x;
        Y = y;
    }
}