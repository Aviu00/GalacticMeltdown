using System;

namespace GalacticMeltdown;

public delegate void DiedEventHandler(Actor sender);

public abstract class Actor
{
    private LimitedStat _hp;
    public int Hp
    {
        get => _hp.Value;
        set
        {
            _hp.Value = value;
            if (value <= 0) Died?.Invoke(this);
        }
    }

    private LimitedStat _energy;
    public int Energy
    {
        get => _energy.Value;
        set => _energy.Value = value;
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    public event DiedEventHandler Died;

    public abstract void Hit(Actor hitter, int damage);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y)
    {
        _hp = new LimitedStat(maxHp, maxHp);
        _energy = new LimitedStat(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        X = x;
        Y = y;
    }
}