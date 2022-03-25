using System;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

internal struct StatInfo
{
    public string Text;
    public int Val;
    public int? MaxVal;

    public StatInfo(string text, int val, int? maxVal = null)
    {
        Text = text;
        Val = val;
        MaxVal = maxVal;
    }
}

public class OverlayView : View
{
    private const ConsoleColor HpColor = DataHolder.Colors.HpColor;
    private const ConsoleColor EnergyColor = DataHolder.Colors.EnergyColor;
    private const ConsoleColor StrColor = DataHolder.Colors.StrColor;
    private const ConsoleColor DexColor = DataHolder.Colors.DexColor;
    private const ConsoleColor DefColor = DataHolder.Colors.DefColor;
    private const ConsoleColor OtherTextColor = ConsoleColor.Blue;

    private Level _level; // Enemy-in-sight indicator, (?) Event log
    private Player _player; // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?

    private StatInfo _hpInfo;
    private StatInfo _energyInfo;
    private StatInfo _dexInfo;
    private StatInfo _defInfo;
    private StatInfo _strInfo;
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;

    public OverlayView(Level level)
    {
        _level = level;
        _player = level.Player;
        _player.StatChanged += OnStatChange;
        RenderStats();
        // TODO: subscribe to events
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y == Height - 1)
        {
            if (!(_hpInfo.Val == _player.Hp && _hpInfo.MaxVal == _player.MaxHp)) RenderStats();
            if (x < _hpInfo.Text.Length) return new ViewCellData((_hpInfo.Text[x], HpColor), null);
        }
        else if (y == Height - 2)
        {
            if (!(_energyInfo.Val == _player.Energy && _energyInfo.MaxVal == _player.MaxEnergy)) RenderStats();
            if (x < _energyInfo.Text.Length) return new ViewCellData((_energyInfo.Text[x], EnergyColor), null);
        }
        else if (y == Height - 3)
        {
            if (_strInfo.Val != _player.Strength) RenderStats();
            if (x < _strInfo.Text.Length) return new ViewCellData((_strInfo.Text[x], StrColor), null);
        }
        else if (y == Height - 4)
        {
            if (_defInfo.Val != _player.Defence) RenderStats();
            if (x < _defInfo.Text.Length) return new ViewCellData((_defInfo.Text[x], DefColor), null);
        }
        else if (y == Height - 5)
        {
            if (_dexInfo.Val != _player.Dexterity) RenderStats();
            if (x < _dexInfo.Text.Length) return new ViewCellData((_dexInfo.Text[x], DexColor), null);
        }
        else if (y == Height - 6 && _player.Equipment[BodyPart.Hands] is not null)
        {
            string s = $"Held: {_player.Equipment[BodyPart.Hands].Name}";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        else if (y == Height - 7 && _player.Equipment[BodyPart.Hands] is WeaponItem && _player.ChosenAmmoId is not null)
        {
            int count = _player.Inventory[ItemCategory.Item].Count(item => item.Id == _player.ChosenAmmoId);
            string name = _player.Inventory[ItemCategory.Item].First(item => item.Id == _player.ChosenAmmoId).Name;
            string s = $"Ammo: {count} ({name})";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        else if (y == Height - 8 && _player.Equipment[BodyPart.Head] is not null)
        {
            string s = $"Head: {_player.Equipment[BodyPart.Head].Name}";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        else if (y == Height - 9 && _player.Equipment[BodyPart.Torso] is not null)
        {
            string s = $"Torso: {_player.Equipment[BodyPart.Torso].Name}";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        else if (y == Height - 10 && _player.Equipment[BodyPart.Legs] is not null)
        {
            string s = $"Legs: {_player.Equipment[BodyPart.Legs].Name}";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        else if (y == Height - 11 && _player.Equipment[BodyPart.Feet] is not null)
        {
            string s = $"Feet: {_player.Equipment[BodyPart.Feet].Name}";
            if (x < s.Length) return new ViewCellData((s[x], OtherTextColor), null);
        }
        return new ViewCellData(null, null);
    }
    
    private void RenderStats()
    {
        _hpInfo = new StatInfo($"HP: {_player.Hp}/{_player.MaxHp}", _player.Hp, _player.MaxHp);
        _energyInfo = new StatInfo($"Energy: {_player.Energy}/{_player.MaxEnergy}", _player.Energy, _player.MaxEnergy);
        _strInfo = new StatInfo($"STR: {_player.Strength}", _player.Strength);
        _defInfo = new StatInfo($"DEF: {_player.Defence}", _player.Defence);
        _dexInfo = new StatInfo($"DEX: {_player.Dexterity}", _player.Dexterity);
    }

    private void OnStatChange(object sender, StatChangeEventArgs e)
    {
        RenderStats();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}