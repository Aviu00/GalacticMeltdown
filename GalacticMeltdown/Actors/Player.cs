using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [JsonIgnore] public bool GodMode { get; set; }

    [JsonIgnore] public override (char symbol, ConsoleColor color) SymbolData => ('@', Colors.Player.Color);
    [JsonIgnore] public override ConsoleColor? BgColor => null;

    [JsonIgnore] public bool NoClip;

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
    public override int Hp
    {
        get => base.Hp;
        set
        {
            if (GodMode) return;
            base.Hp = value;
        }
    }

    [JsonProperty] private string _chosenAmmoId;

    [JsonProperty] public ObservableCollection<Item> Inventory;
    [JsonProperty] public readonly Dictionary<BodyPart, EquippableItem> Equipment;

    public string ChosenAmmoId
    {
        get => _chosenAmmoId;
        set
        {
            _chosenAmmoId = value;
            EquipmentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler EquipmentChanged;
    public event EventHandler VisiblePointsChanged;

    [JsonConstructor]
    private Player()
    {
        StatChanged += OnStatChanged;
    }

    public Player(int x, int y, Level level)
        : base(PlayerHp, PlayerEnergy, PlayerDexterity, PlayerDefence, PlayerViewRange, x, y, level)
    {
        Inventory = new ObservableCollection<Item>();
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
        if (Level.GetNonTileObject(newX, newY) is Npc npc)
        {
            bool hit = HitTarget(npc);
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
        Inventory.Add(item);
    }

    public void Unequip(BodyPart bodyPart)
    {
        Item item = Equipment[bodyPart];
        if (item is null) return;
        ((EquippableItem) item).Unequip(this);
        Equipment[bodyPart] = null;
        AddToInventory(item);
        if (bodyPart == BodyPart.Hands) ChosenAmmoId = null;
    }

    public void Equip(EquippableItem item)
    {
        Unequip(item.BodyPart);

        Inventory.Remove(item);
        Equipment[item.BodyPart] = item;
        item.Equip(this);

        if (item is WeaponItem weapon)
        {
            SetFirstAvailableAmmoId(weapon);
        }
        EquipmentChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetFirstAvailableAmmoId(WeaponItem weapon)
    {
        if (weapon.AmmoTypes is null) return;
        ChosenAmmoId = Inventory.FirstOrDefault(item => weapon.AmmoTypes.ContainsKey(item.Id))?.Id;
    }

    public void Drop(Item item)
    {
        if (item.Stackable)
        {
            List<Item> items = Inventory.Where(match => match.Id == item.Id).ToList();
            for (int i = Inventory.Count - 1; i >= 0; i--)
            {
                if (Inventory[i].Id == item.Id)
                    Inventory.RemoveAt(i);
            }
            foreach (Item droppedItem in items)
            {
                Level.AddItem(droppedItem, X, Y);
            }
        }
        else
        {
            Inventory.Remove(item);
            Level.AddItem(item, X, Y);
        }

        if (item.Id != ChosenAmmoId) return;
        ChosenAmmoId = null;
        EquipmentChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Consume(ConsumableItem item)
    {
        Inventory.Remove(item);
        item.Consume(this);
    }

    private bool HitTarget(Actor target)
    {
        bool hit;
        if (Equipment[BodyPart.Hands] is null)
        {
            hit = target.Hit(UtilityFunctions.CalculateMeleeDamage(DefaultMinDamage, DefaultMaxDamage, Strength),
                false, false);
            Energy -= DefaultAttackEnergy;
            return hit;
        }

        WeaponItem weaponItem = (WeaponItem) Equipment[BodyPart.Hands];
        int minDamage = weaponItem.MinHitDamage;
        int maxDamage = weaponItem.MaxHitDamage;
        if (weaponItem.AmmoTypes is not null && weaponItem is not RangedWeaponItem && _chosenAmmoId is not null)
        {
            Item ammo = GetAmmo();
            minDamage += weaponItem.AmmoTypes[ammo.Id].minDamage;
            maxDamage += weaponItem.AmmoTypes[ammo.Id].maxDamage;
            RemoveAmmo(ammo);
            EquipmentChanged?.Invoke(this, EventArgs.Empty);
            UtilityFunctions.ApplyStateChangers(weaponItem.AmmoTypes[ammo.Id].actorStateChangerData, target);
        }

        int damage = UtilityFunctions.CalculateMeleeDamage(minDamage, maxDamage, Strength);
        hit = target.Hit(damage, false, false);
        UtilityFunctions.ApplyStateChangers(weaponItem.StateChangers, target);
        Energy -= weaponItem.HitEnergy;
        return hit;
    }

    public bool Shoot(int x, int y)
    {
        if (Equipment[BodyPart.Hands] is not RangedWeaponItem gun || ChosenAmmoId is null) return false;
        Item ammo = GetAmmo();
        List<(int, int)> lineCells = new();
        foreach (var point in Algorithms.BresenhamGetPointsOnLine(X, Y, x, y, 200).Skip(1))
        {
            var (xi, yi) = point;
            if (Level.GetTile(xi, yi) is {IsWalkable: false}) break;
            IObjectOnMap obj = Level.GetNonTileObject(xi, yi);
            if (obj is null)
            {
                lineCells.Add(point);
                continue;
            }
            if (obj is not Actor actor) break;

            double distance = UtilityFunctions.GetDistance(X, Y, xi, yi);
            if (!UtilityFunctions.Chance(UtilityFunctions.RangeAttackHitChance(distance, gun.Spread))) continue;
            actor.Hit(
                Random.Shared.Next(gun.MinHitDamage + gun.AmmoTypes[ammo.Id].minDamage,
                    gun.MaxHitDamage + gun.AmmoTypes[ammo.Id].maxDamage + 1), true, false);
            UtilityFunctions.ApplyStateChangers(gun.AmmoTypes[ammo.Id].actorStateChangerData, actor);
            break;
        }
        
        _actionInfo = new ActorActionInfo(ActorAction.Shoot, lineCells);
        RemoveAmmo(ammo);
        Energy -= gun.ShootEnergy;
        EquipmentChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private Item GetAmmo()
    {
        return Inventory.FirstOrDefault(item => item.Id == ChosenAmmoId);
    }

    private void RemoveAmmo(Item ammo)
    {
        Inventory.Remove(ammo);
        if (!Inventory.Any(match => ammo.Id == match.Id))
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

    public override void Alert(Actor actor)
    {
    }
}