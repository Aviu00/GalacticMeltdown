using System;
using System.Collections.Generic;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors.Effects;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

[JsonConverter(typeof(JsonSubtypes), "ActorName")]
[JsonSubtypes.KnownSubType(typeof(Player), "Player")]
[JsonSubtypes.KnownSubType(typeof(Npc), "Npc")]
public abstract class Actor : IObjectOnMap
{
    public const int DefaultStrength = 10;
    private const int DefaultMoveSpeed = 0;
    
    [JsonProperty] protected abstract string ActorName { get; }

    [JsonProperty] public readonly Level Level;

    [JsonProperty] private bool _turnStopped;

    [JsonProperty] private int _strength;
    [JsonProperty] private int _moveSpeed;
    [JsonProperty] private int _viewRange;

    [JsonIgnore]
    public int Strength
    {
        get => _strength;
        set
        {
            if (value == _strength) return;
            _strength = value;
            FireStatAffected(Stat.Strength);
        }
    }
    
    [JsonIgnore]
    public int MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (value == _moveSpeed) return;
            _moveSpeed = value;
            FireStatAffected(Stat.MoveSpeed);
        }
    }

    [JsonIgnore]
    public virtual int ViewRange
    {
        get => _viewRange; 
        set => _viewRange = value;
    }

    [JsonIgnore] public bool IsActive => Hp > 0 && Energy > 0 && !_turnStopped;

    [JsonProperty] private readonly LimitedNumber _hpLim;
    [JsonProperty] private readonly LimitedNumber _energyLim;
    [JsonProperty] private int _dexterity;
    [JsonProperty] private int _defence;

    [JsonProperty] private List<Effect> _effects;

    [JsonIgnore]
    public virtual int Hp
    {
        get => _hpLim.Value;
        set
        {
            if (value == _hpLim.Value) return;
            _hpLim.Value = value;
            FireStatAffected(Stat.Hp);
            if (value <= 0) Died?.Invoke(this, EventArgs.Empty);
        }
    }

    [JsonIgnore]
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

    [JsonIgnore]
    public int Dexterity
    {
        get => _dexterity;
        set
        {
            if (value == _dexterity) return;
            _dexterity = value;
            FireStatAffected(Stat.Dexterity);
        } 
    }

    [JsonIgnore]
    public int Defence
    {
        get => _defence;
        set
        {
            if (value == _defence) return;
            _defence = value;
            FireStatAffected(Stat.Defence);
        }
    }

    [JsonIgnore] public int MaxHp
    {
        get => (int) _hpLim.MaxValue!;
        set
        {
            if (value == _hpLim.MaxValue) return;
            _hpLim.MaxValue = value;
            FireStatAffected(Stat.Hp);
            if (value <= 0) Died?.Invoke(this, EventArgs.Empty);
        }
    }

    [JsonIgnore] public int MaxEnergy
    {
        get => (int) _energyLim.MaxValue!;
        set
        {
            if (value == _energyLim.MaxValue) return;
            _energyLim.MaxValue = value;
            if (value <= 0) Died?.Invoke(this, EventArgs.Empty);
            FireStatAffected(Stat.Energy);
        }
    }

    [JsonProperty] public int X { get; private set; }
    [JsonProperty] public int Y { get; private set; }

    public abstract (char symbol, ConsoleColor color) SymbolData { get; }
    public abstract ConsoleColor? BgColor { get; }


    public event EventHandler Died;
    public event EventHandler SpentEnergy;
    public event EventHandler<StatChangeEventArgs> StatChanged; 
    public event EventHandler InvolvedInTurn;
    public event EventHandler<MoveEventArgs> Moved;

    [JsonConstructor]
    protected Actor()
    {
    }
    protected Actor(int maxHp, int maxEnergy, int dexterity, int defence, int viewRange, int x, int y, Level level)
    {
        _effects = new List<Effect>();
        Level = level;
        _hpLim = new LimitedNumber(maxHp, maxHp, 0);
        _energyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dexterity = dexterity;
        Defence = defence;
        _viewRange = viewRange;
        X = x;
        Y = y;
        _turnStopped = false;
        Strength = DefaultStrength;
        MoveSpeed = DefaultMoveSpeed;
    }

    public virtual bool Hit(int damage, bool ignoreDexterity, bool ignoreDefence)
    {
        if (!ignoreDexterity && Dexterity > 0 && UtilityFunctions.OccuredMultipleTries(5, _dexterity))
            return false;
        if (!ignoreDefence && Defence > 0)
        {
            damage -= Random.Shared.Next(0, Defence + 1);
            if (damage < 0) damage = 0;
        }
        Hp -= damage;
        return true;
    }

    public void FinishTurn()
    {
        if (Hp == 0) return;
        _turnStopped = false;
        Energy += _energyLim.MaxValue!.Value;
    }

    public virtual void StopTurn() => _turnStopped = true;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Tile tile = Level.GetTile(x, y);
        if (tile is not null) Energy -= Math.Max(tile.MoveCost - MoveSpeed, 1);
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY));
    }

    protected void SendAffected() => InvolvedInTurn?.Invoke(this, EventArgs.Empty);

    private void FireStatAffected(Stat stat)
    {
        StatChanged?.Invoke(this, new StatChangeEventArgs(stat));
    }

    public void AddEffect(Effect effect)
    {
        _effects.Add(effect);
    }

    public void RemoveEffect(Effect effect)
    {
        _effects.Remove(effect);
    }

    public int GetViewRange()
    {
        return ViewRange < 1 ? 1 : ViewRange;
    }

    public abstract ActorActionInfo TakeAction();

    public abstract void Alert(Actor actor);
}