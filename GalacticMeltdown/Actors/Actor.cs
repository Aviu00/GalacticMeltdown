using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public abstract class Actor : IObjectOnMap
{
    [JsonProperty] private bool _turnStopped;

    [JsonProperty] protected int viewRange;

    [JsonIgnore] public bool IsActive => Hp > 0 && Energy > 0 && !_turnStopped;

    [JsonProperty] protected readonly LimitedNumber HpLim;
    [JsonProperty] protected readonly LimitedNumber EnergyLim;
    [JsonProperty] private int _dex;
    [JsonProperty] private int _def;

    [JsonIgnore]
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

    [JsonIgnore]
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

    [JsonIgnore]
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

    [JsonIgnore]
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

    [JsonIgnore] public int MaxHp => (int) HpLim.MaxValue!;
    [JsonIgnore] public int MaxEnergy => (int) EnergyLim.MaxValue!;

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

    protected Actor()
    {
    }
    protected Actor(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level)
    {
        Level = level;
        HpLim = new LimitedNumber(maxHp, maxHp, 0);
        EnergyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dex = dex;
        Def = def;
        this.viewRange = viewRange;
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