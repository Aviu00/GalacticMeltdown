using System;
using GalacticMeltdown.LevelRelated;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Player : Actor, ISightedObject, IControllable
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDexterity = 16;
    private const int PlayerDefence = 4;
    private const int PlayerViewRange = 20;
    //private const int PlayerStr = 10;

    [JsonProperty] protected override string ActorName => "Player";
    private Action _giveControlToUser;
    
    private bool _xray;

    private int _strength;

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

    public override (char symbol, ConsoleColor color) SymbolData => ('@', ConsoleColor.White);
    public override ConsoleColor? BgColor => null;

    [JsonIgnore] public bool NoClip;

    [JsonIgnore] public bool InFocus { get; set; }

    [JsonIgnore]
    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [JsonIgnore]
    public int ViewRange
    {
        get => _viewRange;
        set
        {
            if (value <= 0) return;
            _viewRange = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler VisiblePointsChanged;

    [JsonConstructor]
    private Player()
    {
    }
    public Player(int x, int y, Level level)
        : base(PlayerHp, PlayerEnergy, PlayerDexterity, PlayerDefence, PlayerViewRange, x, y, level)
    {
        _strength = Str;
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        Tile tile = Level.GetTile(X + deltaX, Y + deltaY);
        if (Level.GetNonTileObject(X + deltaX, Y + deltaY) is not null)
            return false;
        if (!NoClip && (tile is null || !tile.IsWalkable))
        {
            Level.InteractWithDoor(X + deltaX, Y + deltaY, this);
            return false;
        }
        MoveTo(X + deltaX, Y + deltaY);
        VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override void TakeAction()
    {
        _giveControlToUser?.Invoke();
    }

    public void SetControlFunc(Action controlFunc)
    {
        _giveControlToUser = controlFunc;
    }
}