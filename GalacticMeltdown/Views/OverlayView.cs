using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;

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
    
    private Level _level; // Minimap, Enemy-in-sight indicator, (?) Event log
    private Player _player; // State, Effects, (?) Event log, Enemy-in-sight indicator
    // Performance monitor? Coordinates of controlled object, player?

    private StatInfo _hpInfo;
    private StatInfo _energyInfo;
    private StatInfo _dexInfo;
    private StatInfo _defInfo;
    private StatInfo _strInfo;
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public OverlayView(Level level)
    {
        _level = level;
        _player = level.Player;
        Width = 0;
        Height = 0;
        RenderStats();
        // TODO: subscribe to events
    }

    private void RenderStats()
    {
        _hpInfo = new StatInfo($"HP: {_player.Hp}/{_player.MaxHp}", _player.Hp, _player.MaxHp);
        _energyInfo = new StatInfo($"Energy: {_player.Energy}/{_player.MaxEnergy}", _player.Energy, _player.MaxEnergy);
        _strInfo = new StatInfo($"STR: {_player.Str}", _player.Str);
        _defInfo = new StatInfo($"DEF: {_player.Def}", _player.Def);
        _dexInfo = new StatInfo($"DEX: {_player.Dex}", _player.Dex);
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (y == Width - 1)
        {
            if (!(_hpInfo.Val == _player.Hp && _hpInfo.MaxVal == _player.MaxHp)) RenderStats();
            if (x < _hpInfo.Text.Length) return new ViewCellData((_hpInfo.Text[x], HpColor), null);
        }
        else if (y == Width - 2)
        {
            if (!(_energyInfo.Val == _player.Energy && _energyInfo.MaxVal == _player.MaxEnergy)) RenderStats();
            if (x < _energyInfo.Text.Length) return new ViewCellData((_energyInfo.Text[x], EnergyColor), null);
        }
        else if (y == Width - 3)
        {
            if (_strInfo.Val != _player.Str) RenderStats();
            if (x < _strInfo.Text.Length) return new ViewCellData((_strInfo.Text[x], StrColor), null);
        }
        else if (y == Width - 4)
        {
            if (_defInfo.Val != _player.Def) RenderStats();
            if (x < _defInfo.Text.Length) return new ViewCellData((_defInfo.Text[x], DefColor), null);
        }
        else if (y == Width - 5)
        {
            if (_dexInfo.Val != _player.Dex) RenderStats();
            if (x < _dexInfo.Text.Length) return new ViewCellData((_dexInfo.Text[x], DexColor), null);
        }
        return new ViewCellData(null, null);
    }
}