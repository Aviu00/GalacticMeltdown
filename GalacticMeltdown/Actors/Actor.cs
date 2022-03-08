using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public abstract class Actor : IObjectOnMap
{
    private bool _turnStopped;

    public bool IsActive => Hp > 0 && Energy > 0 && !_turnStopped;

    private readonly LimitedNumber _hpLim;
    protected int Hp
    {
        get => _hpLim.Value;
        set
        {
            _hpLim.Value = value;
            if (value <= 0) Died?.Invoke(this, EventArgs.Empty);
        }
    }

    private readonly LimitedNumber _energyLim;
    private int Energy
    {
        get => _energyLim.Value;
        set
        {
            _energyLim.Value = value;
            if (value < _energyLim.Value) SpentEnergy?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; private set; }
    public int Y { get; private set; }

    public (char symbol, ConsoleColor color) SymbolData { get; protected init; }
    public ConsoleColor? BgColor { get; protected init; }

    protected Level Level { get; }

    public event EventHandler Died;
    public event EventHandler SpentEnergy;
    public event EventHandler InvolvedInTurn;
    public event EventHandler<MoveEventArgs> Moved;
    
    protected Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level)
    {
        Level = level;
        _hpLim = new LimitedNumber(maxHp, maxHp);
        _energyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
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
        if (Hp <= 0) return;
        _turnStopped = false;
        Energy += _energyLim.MaxValue;
    }
    
    public void StopTurn() => _turnStopped = true;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }
    
    protected void SendAffected() => InvolvedInTurn?.Invoke(this, EventArgs.Empty);

    public abstract void TakeAction();
}