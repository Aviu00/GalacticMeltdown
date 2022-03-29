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
[JsonSubtypes.KnownSubType(typeof(Enemy), "Enemy")]
public abstract class Actor : IObjectOnMap
{
    [JsonProperty] protected abstract string ActorName { get; }
    [JsonProperty] public readonly Level Level;
    [JsonProperty] private bool _turnStopped;
    public const int ActorStr = 10;
    private const int ActorMoveSpeed = 0;
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
    public int ViewRange
    {
        get => _viewRange;
        set
        {
            if (value == _viewRange) return;
            _viewRange = value;
            FireStatAffected(Stat.ViewRange);
        }
    }

    [JsonIgnore] public bool IsActive => Hp > 0 && Energy > 0 && !_turnStopped;

    [JsonProperty] public readonly LimitedNumber HpLim;
    [JsonProperty] public readonly LimitedNumber EnergyLim;
    [JsonProperty] private int _dexterity;
    [JsonProperty] private int _defence;

    [JsonProperty] private List<Effect> _effects;

    [JsonIgnore]
    public virtual int Hp
    {
        get => HpLim.Value;
        set
        {
            if (value == HpLim.Value) return;
            HpLim.Value = value;
            if (HpLim.Value > HpLim.MaxValue!.Value) HpLim.Value = HpLim.MaxValue.Value;
            if (HpLim.Value == 0) Died?.Invoke(this, EventArgs.Empty);
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

    [JsonIgnore] public int MaxHp => (int) HpLim.MaxValue!;
    [JsonIgnore] public int MaxEnergy => (int) EnergyLim.MaxValue!;

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
        _effects = new();
        Level = level;
        HpLim = new LimitedNumber(maxHp, maxHp, 0);
        EnergyLim = new LimitedNumber(maxEnergy, maxEnergy);
        Dexterity = dexterity;
        Defence = defence;
        ViewRange = viewRange;
        X = x;
        Y = y;
        _turnStopped = false;
        Strength = ActorStr;
        MoveSpeed = ActorMoveSpeed;
    }

    public virtual bool Hit(int damage, bool ignoreDexterity, bool ignoreDefence)
    {
        if (!ignoreDexterity && Dexterity > 0 && UtilityFunctions.ChanceRoll(5, _dexterity))
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
        Energy += EnergyLim.MaxValue!.Value;
    }

    public virtual void StopTurn() => _turnStopped = true;

    protected void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Tile tile = Level.GetTile(x, y);
        if (tile is not null)
        {
            if (tile.MoveCost - MoveSpeed <= 0)
                Energy -= 1;
            else
            {
                Energy -= tile.MoveCost - MoveSpeed;
            }
               
        }
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    protected void SendAffected() => InvolvedInTurn?.Invoke(this, EventArgs.Empty);

    protected void FireStatAffected(Stat stat)
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
        return _viewRange < 1 ? 1 : _viewRange;
    }

    public abstract ActorActionInfo TakeAction();
}