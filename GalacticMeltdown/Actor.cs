using System;

namespace GalacticMeltdown;

public delegate void DiedEventHandler(Actor sender);

public abstract class Actor
{
    private int _hp;
    private int _maxHp;

    protected int MaxHp
    {
        get => _maxHp;
        set
        {
            _maxHp = value;
            Hp = _hp;  // Update hp
        }
    }
    
    protected int Hp
    { 
        get => _hp;
        set
        {
            _hp = Math.Min(value, _maxHp);
            if (_hp <= 0) Died?.Invoke(this);
        }
    }

    private int _maxEnergy;
    private int _energy;
    protected int Energy
    {
        get => _energy;
        set => _energy = Math.Min(value, _maxEnergy);
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    public event DiedEventHandler Died;

    public abstract void Hit(Actor hitter, int damage);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y)
    {
        _maxHp = maxHp;
        _hp = maxHp;
        _maxEnergy = maxEnergy;
        Energy = maxEnergy;
        Dex = dex;
        Def = def;
        X = x;
        Y = y;
    }
}