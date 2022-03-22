using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Player : Actor, ISightedObject, IControllable
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDexterity = 4;
    private const int PlayerDefence = 0;
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

    [JsonProperty] public Dictionary<ItemCategory, List<Item>> Inventory;
    [JsonProperty] public readonly Dictionary<BodyPart, EquippableItem> Equipment;

    public event EventHandler VisiblePointsChanged;

    [JsonConstructor]
    private Player()
    {
    }
    public Player(int x, int y, Level level)
        : base(PlayerHp, PlayerEnergy, PlayerDexterity, PlayerDefence, PlayerViewRange, x, y, level)
    {
        Inventory = new Dictionary<ItemCategory, List<Item>>();
        foreach (ItemCategory val in Enum.GetValues<ItemCategory>())
        {
            Inventory[val] = new List<Item>();
        }

        Equipment = new Dictionary<BodyPart, EquippableItem>();
        foreach (BodyPart val in Enum.GetValues<BodyPart>())
        {
            Equipment[val] = null;
        }
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        Tile tile = Level.GetTile(X + deltaX, Y + deltaY);
        if (Level.GetNonTileObject(X + deltaX, Y + deltaY) is not null)
        {
            Actor act = (Actor) Level.GetNonTileObject(X + deltaX, Y + deltaY);
            act.Hit(50,true, true);
            return true;
        }
        if (!NoClip && (tile is null || !tile.IsWalkable))
        {
            return Level.InteractWithDoor(X + deltaX, Y + deltaY, this);
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

    public void AddToInventory(Item item)
    {
        Inventory[item.Category].Add(item);
    }

    public void Equip(EquippableItem item)
    {
        Item prevItem = Equipment[item.BodyPart];
        if (prevItem == item) return;
        if (prevItem is not null)
        {
            ((EquippableItem)prevItem).Unequip(this);
            AddToInventory(prevItem);
        }

        Inventory[item.Category].Remove(item);
        Equipment[item.BodyPart] = item;
        item.Equip(this);
    }

    public void Drop(Item item)
    {
        Inventory[item.Category].Remove(item);
        Level.AddItem(item, X, Y);
    }

    public void Consume(ConsumableItem item)
    {
        Inventory[item.Category].Remove(item);
        item.Consume(this);
    }

    public void Shoot(int x, int y)
    {
        if (Equipment[BodyPart.Hands] is not RangedWeaponItem gun) return;
        Item ammo = Inventory[ItemCategory.Item].FirstOrDefault(item => gun.ammoTypes.ContainsKey(item.Id));
        if (ammo is null) return;
        if (UtilityFunctions.Chance(
                UtilityFunctions.RangeAttackHitChance((int) UtilityFunctions.GetDistance(X, Y, x, y), gun.Spread)))
        {
            foreach (var (xi, yi) in Algorithms.BresenhamGetPointsOnLine(X, Y, x, y).Skip(1))
            {
                if (Level.GetNonTileObject(xi, yi) is Enemy enemy)
                {
                    Inventory[ItemCategory.Item].Remove(ammo);
                    enemy.Hit(
                        Random.Shared.Next(gun.MinHitDamage + gun.ammoTypes[ammo.Id].minDamage,
                            gun.MaxHitDamage + gun.ammoTypes[ammo.Id].maxDamage + 1), true, false);
                    ActorStateChangerData stateChanger = gun.ammoTypes[ammo.Id].actorStateChangerData;
                    if (stateChanger is not null)
                    {
                        DataHolder.ActorStateChangers[stateChanger.Type](enemy, stateChanger.Power,
                            stateChanger.Duration);
                    }
                    break;
                }
            }
        }

        Energy -= gun.ShootEnergy;
    }
}