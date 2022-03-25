using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Player : Actor, ISightedObject, IControllable
{
    private const int PlayerHp = 100;
    private const int PlayerEnergy = 100;
    private const int PlayerDexterity = 8;
    private const int PlayerDefence = 0;
    private const int PlayerViewRange = 20;

    private const int DefaultAttackEnergy = 40;
    private const int DefaultMinDamage = 0;
    private const int DefaultMaxDamage = 10;

    [JsonProperty] protected override string ActorName => "Player";
    private Action _giveControlToUser;
    private ActorActionInfo _actionInfo;
    
    private bool _xray;

    public override (char symbol, ConsoleColor color) SymbolData => ('@', ConsoleColor.White);
    [JsonIgnore] public override ConsoleColor? BgColor => null;

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

    [JsonProperty] public Dictionary<ItemCategory, List<Item>> Inventory;
    [JsonProperty] public readonly Dictionary<BodyPart, EquippableItem> Equipment;
    [JsonProperty] public string ChosenAmmoId;

    public event EventHandler VisiblePointsChanged;

    [JsonConstructor]
    private Player()
    {
        StatChanged += OnStatChanged;
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

        StatChanged += OnStatChanged;
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        int newX = X + deltaX, newY = Y + deltaY;
        Tile tile = Level.GetTile(newX, newY);
        if (Level.GetNonTileObject(newX, newY) is Enemy enemy)
        {
            bool hit = HitTarget(enemy);
            _actionInfo = new ActorActionInfo(hit ? ActorAction.MeleeAttackHit : ActorAction.MeleeAttackMissed,
                new List<(int, int)> {(newX, newY)});
            return true;
        }

        if (!NoClip && (tile is null || !tile.IsWalkable))
        {
            if (Level.InteractWithDoor(newX, newY, this))
            {
                _actionInfo = new ActorActionInfo(ActorAction.InteractWithDoor, new List<(int, int)> {(newX, newY)});
                return true;
            }
            _actionInfo = null;
            return false;
        }
        _actionInfo = new ActorActionInfo(ActorAction.Move, new List<(int, int)> {(X, Y), (newX, newY)});
        MoveTo(newX, newY);
        VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override ActorActionInfo TakeAction()
    {
        _actionInfo = null;
        _giveControlToUser?.Invoke();
        return _actionInfo;
    }

    public void SetControlFunc(Action controlFunc)
    {
        _giveControlToUser = controlFunc;
    }

    public void AddToInventory(Item item)
    {
        Inventory[item.Category].Add(item);
    }

    public void Unequip(BodyPart bodyPart)
    {
        Item item = Equipment[bodyPart];
        if (item is null) return;
        ((EquippableItem) item).Unequip(this);
        Equipment[bodyPart] = null;
        AddToInventory(item);
        ChosenAmmoId = null;
    }

    public void Equip(EquippableItem item)
    {
        Unequip(item.BodyPart);

        Inventory[item.Category].Remove(item);
        Equipment[item.BodyPart] = item;
        item.Equip(this);

        if (item is WeaponItem weapon)
        {
            SetFirstAvailableAmmoId(weapon);
        }
    }

    private void SetFirstAvailableAmmoId(WeaponItem weapon)
    {
        if (weapon.AmmoTypes is null) return;
        ChosenAmmoId = Inventory[ItemCategory.Item].FirstOrDefault(item => weapon.AmmoTypes.Keys.Contains(item.Id))?.Id;
    }

    public void Drop(Item item)
    {
        if (item.Stackable)
        {
            List<Item> items = Inventory[item.Category].Where(match => match.Id == item.Id).ToList();
            Inventory[item.Category].RemoveAll(match => match.Id == item.Id);
            foreach (Item droppedItem in items)
            {
                Level.AddItem(droppedItem, X, Y);
            }
        }
        else
        {
            Inventory[item.Category].Remove(item);
            Level.AddItem(item, X, Y);
        }

        if (item.Id == ChosenAmmoId)
        {
            ChosenAmmoId = null;
        }
    }

    public void Consume(ConsumableItem item)
    {
        Inventory[item.Category].Remove(item);
        item.Consume(this);
    }

    private bool HitTarget(Actor target)
    {
        bool hit;
        if (Equipment[BodyPart.Hands] is null)
        {
            hit = target.Hit(Random.Shared.Next(DefaultMinDamage, DefaultMaxDamage), false, false);
            Energy -= DefaultAttackEnergy;
            return hit;
        }

        WeaponItem weaponItem = (WeaponItem) Equipment[BodyPart.Hands];
        int minDamage = weaponItem.MinHitDamage;
        int maxDamage = weaponItem.MaxHitDamage;
        if (weaponItem.AmmoTypes is not null && weaponItem is not RangedWeaponItem)
        {
            Item ammo = Inventory[ItemCategory.Item].FirstOrDefault(item => weaponItem.AmmoTypes.ContainsKey(item.Id));
            if (ammo is not null)
            {
                minDamage += weaponItem.AmmoTypes[ammo.Id].minDamage;
                maxDamage += weaponItem.AmmoTypes[ammo.Id].maxDamage;
                RemoveAmmo(ammo);
                var actorStateChangerData = weaponItem.AmmoTypes[ammo.Id].actorStateChangerData;
                if (actorStateChangerData is not null)
                {
                    DataHolder.ActorStateChangers[actorStateChangerData.Type](target, actorStateChangerData.Power,
                        actorStateChangerData.Duration);
                }
            }
        }

        int damage = UtilityFunctions.CalculateMeleeDamage(minDamage, maxDamage, Strength);
        hit = target.Hit(damage, false, false);
        var stateChanger = weaponItem.StateChanger;
        if (stateChanger is not null)
        {
            DataHolder.ActorStateChangers[stateChanger.Type](target, stateChanger.Power,
                stateChanger.Duration);
        }

        Energy -= weaponItem.HitEnergy;
        return hit;
    }

    public bool Shoot(int x, int y)
    {
        if (Equipment[BodyPart.Hands] is not RangedWeaponItem gun || ChosenAmmoId is null) return false;
        Item ammo = Inventory[ItemCategory.Item].First(item => item.Id == ChosenAmmoId);
        List<(int, int)> lineCells = new();
        foreach (var point in Algorithms.BresenhamGetPointsOnLine(X, Y, x, y).Skip(1))
        {
            lineCells.Add(point);
            var (xi, yi) = point;
            if (Level.GetTile(xi, yi) is {IsWalkable: false}) break;
            IObjectOnMap obj = Level.GetNonTileObject(xi, yi);
            if (obj is null) continue;
            if (obj is not Actor actor) break;

            double distance = UtilityFunctions.GetDistance(X, Y, xi, yi);
            if (!UtilityFunctions.Chance(UtilityFunctions.RangeAttackHitChance(distance, gun.Spread))) continue;
            actor.Hit(
                Random.Shared.Next(gun.MinHitDamage + gun.AmmoTypes[ammo.Id].minDamage,
                    gun.MaxHitDamage + gun.AmmoTypes[ammo.Id].maxDamage + 1), true, false);
            ActorStateChangerData stateChanger = gun.AmmoTypes[ammo.Id].actorStateChangerData;
            if (stateChanger is not null)
            {
                DataHolder.ActorStateChangers[stateChanger.Type](actor, stateChanger.Power,
                    stateChanger.Duration);
            }

            break;
        }
        
        _actionInfo = new ActorActionInfo(ActorAction.Shoot, lineCells);
        RemoveAmmo(ammo);
        Energy -= gun.ShootEnergy;
        return true;
    }

    private void RemoveAmmo(Item ammo)
    {
        Inventory[ItemCategory.Item].Remove(ammo);
        if (!Inventory[ItemCategory.Item].Any(match => ammo.Id == match.Id))
            SetFirstAvailableAmmoId((WeaponItem) Equipment[BodyPart.Hands]);
    }

    private void OnStatChanged(object sender, StatChangeEventArgs e)
    {
        if (e.Stat == Stat.ViewRange)
            VisiblePointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void StopTurn()
    {
        base.StopTurn();
        _actionInfo = new ActorActionInfo(ActorAction.StopTurn, new List<(int, int)>());
    }
}