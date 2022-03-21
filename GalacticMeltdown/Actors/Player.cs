using System;
using System.Collections.Generic;
using GalacticMeltdown.Items;
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

    [JsonProperty] protected override string ActorName => "Player";
    private Action _giveControlToUser;
    
    private bool _xray;

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
    public override int ViewRange
    {
        get => base.ViewRange;
        set
        {
            base.ViewRange = value;
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private Dictionary<ItemCategory, List<Item>> _inventory;

    public event EventHandler VisiblePointsChanged;

    [JsonConstructor]
    private Player()
    {
    }
    public Player(int x, int y, Level level)
        : base(PlayerHp, PlayerEnergy, PlayerDexterity, PlayerDefence, PlayerViewRange, x, y, level)
    {
        _inventory = new Dictionary<ItemCategory, List<Item>>();
        foreach (ItemCategory val in Enum.GetValues(typeof(ItemCategory)))
        {
            _inventory[val] = new List<Item>();
        }
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        Tile tile = Level.GetTile(X + deltaX, Y + deltaY);
        if (Level.GetNonTileObject(X + deltaX, Y + deltaY) is not null)
        {// temporary except of "return false"
            Actor act = (Actor) Level.GetNonTileObject(X + deltaX, Y + deltaY);
            act.Hit(50,true, true);
            return false;
        }//temporary
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

    public void PickUp(Item item)
    {
        _inventory[item.Category].Add(item);
    }
}