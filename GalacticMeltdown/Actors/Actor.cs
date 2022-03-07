using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Actors;

public abstract class Actor : IObjectOnMap
{
    protected bool TurnStopped;
    
    public bool IsActive => Hp > 0 && Energy > 0 && !TurnStopped;

    protected LimitedNumber HpLim;

    public int Hp
    {
        get => HpLim.Value;
        protected set
        {
            HpLim.Value = value;
            if (value <= 0) Died?.Invoke(this, EventArgs.Empty);
        }
    }

    protected LimitedNumber EnergyLim;

    public int Energy
    {
        get => EnergyLim.Value;
        protected set
        {
            EnergyLim.Value = value;
            if (value < EnergyLim.Value) SpentEnergy?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Dex { get; protected set; }
    public int Def { get; protected set; }

    public int X { get; set; }
    public int Y { get; set; }

    public (char symbol, ConsoleColor color) SymbolData { get; protected init; }
    public ConsoleColor? BgColor { get; protected init; }

    public Action DoAction { get; protected set; }

    public LevelRelated.Level Level { get; }

    public event EventHandler Died;
    public event EventHandler SpentEnergy;
    public event EventHandler InvolvedInTurn;
    public event EventHandler<MoveEventArgs> Moved;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    public void StopTurn() => TurnStopped = true;

    public void SendAffected() => InvolvedInTurn?.Invoke(this, EventArgs.Empty);

    public Actor(int maxHp, int maxEnergy, int dex, int def, int x, int y, LevelRelated.Level level)
    {
        Level = level;
        HpLim = new LimitedNumber(maxHp, maxHp);
        EnergyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        X = x;
        Y = y;
        TurnStopped = false;
    }

    public virtual void Hit(Actor hitter, int damage)
    {
        SendAffected();
    }

    public virtual void FinishTurn()
    {
        if (Hp <= 0) return;
        TurnStopped = false;
        Energy += EnergyLim.MaxValue;
    }
}