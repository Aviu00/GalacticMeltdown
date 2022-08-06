using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class StatusView : View, ILineUpdate
{
    private const ConsoleColor HpColor = Colors.Overlay.Hp;
    private const ConsoleColor EnergyColor = Colors.Overlay.Energy;
    private const ConsoleColor StrColor = Colors.Overlay.Strength;
    private const ConsoleColor DexColor = Colors.Overlay.Dexterity;
    private const ConsoleColor DefColor = Colors.Overlay.Defence;
    private const ConsoleColor OtherTextColor = Colors.Overlay.Other;

    private Level _level; // Maybe later
    private Player _player; // State, equipment

    private readonly List<Func<(string text, ConsoleColor color)?>> _lines = new();

    private readonly Dictionary<Stat, int> _statLine = new();
    
    private readonly Dictionary<Equipment, int> _equipmentLine = new();

    public event EventHandler<LineUpdateEventArgs> LineUpdate;

    public StatusView(Level level)
    {
        _level = level;
        _player = level.Player;
        _player.StatChanged += OnStatChange;
        _player.EquipmentChanged += OnEquipmentChange;
        SetLines();
    }

    private void SetLines()
    {
        List<(Stat, Func<(string text, ConsoleColor color)?>)> stats =
            new()
            {
                (Stat.Hp, () => ($"HP: {_player.Hp}/{_player.MaxHp}", HpColor)),
                (Stat.Energy, () => ($"Energy: {_player.Energy}/{_player.MaxEnergy}", EnergyColor)),
                (Stat.Strength, () => ($"STR: {_player.Strength}", StrColor)),
                (Stat.Defence, () => ($"DEF: {_player.Defence}", DefColor)),
                (Stat.Dexterity, () => ($"DEX: {_player.Dexterity}", DexColor)),
            };
        List<(Equipment, Func<(string text, ConsoleColor color)?>)> equipment =
            new()
            {
                (Equipment.Hands, () => _player.Equipment[BodyPart.Hands] is not null
                    ? ($"Held: {_player.Equipment[BodyPart.Hands].Name}", OtherTextColor)
                    : null),
                (Equipment.Ammo, () =>
                    _player.Equipment[BodyPart.Hands] is WeaponItem && _player.ChosenAmmoId is not null
                        ? (
                            $"Ammo: {_player.Inventory.Count(item => item.Id == _player.ChosenAmmoId)} "
                            + $"({_player.Inventory.First(item => item.Id == _player.ChosenAmmoId).Name})",
                            OtherTextColor)
                        : null),
                (Equipment.Head, () => _player.Equipment[BodyPart.Head] is not null
                    ? ($"Head: {_player.Equipment[BodyPart.Head].Name}", OtherTextColor)
                    : null),
                (Equipment.Torso, () => _player.Equipment[BodyPart.Torso] is not null
                    ? ($"Torso: {_player.Equipment[BodyPart.Torso].Name}", OtherTextColor)
                    : null),
                (Equipment.Legs, () => _player.Equipment[BodyPart.Legs] is not null
                    ? ($"Legs: {_player.Equipment[BodyPart.Legs].Name}", OtherTextColor)
                    : null),
                (Equipment.Feet, () => _player.Equipment[BodyPart.Feet] is not null
                    ? ($"Feet: {_player.Equipment[BodyPart.Feet].Name}", OtherTextColor)
                    : null),
            };
        int i = 0;
        AddLoop(stats, _statLine);
        AddLoop(equipment, _equipmentLine);

        void AddLoop<TKey>(List<(TKey, Func<(string text, ConsoleColor color)?>)> lines,
            Dictionary<TKey, int> dictionary)
        {
            foreach ((TKey eqp, Func<(string text, ConsoleColor color)?> func) in lines)
            {
                dictionary.Add(eqp, i++);
                _lines.Add(func);
            }
        }
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int lineIndex = ConvertToTopY(y);
        if (lineIndex >= _lines.Count) return new ViewCellData(null, null);
        (string text, ConsoleColor color)? lineInfo = _lines[lineIndex]();
        if (lineInfo is null) return new ViewCellData(null, null);
        (string text, ConsoleColor color) = lineInfo.Value;
        return new ViewCellData(x < text.Length ? (text[x], color) : null, null);
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        for (var line = 0; line < Math.Min(_lines.Count, Height); line++)
        {
            (string text, ConsoleColor color)? lineInfo = _lines[line]();
            if (lineInfo is null) continue;
            (string text, ConsoleColor color) = lineInfo.Value;
            for (int col = 0; col < Math.Min(text.Length, Width); col++)
            {
                cells[col, ConvertToTopY(line)] = new ViewCellData((text[col], color), null);
            }
        }
        return cells;
    }

    private void OnStatChange(object sender, StatChangeEventArgs e) => UpdateStatusLine(_statLine, e.Stat);

    private void OnEquipmentChange(object sender, EquipmentChangeEventArgs e) =>
        UpdateStatusLine(_equipmentLine, e.Equipment);

    private void UpdateStatusLine<TKey>(Dictionary<TKey, int> lineDict, TKey statusType)
    {
        var lineContents = new List<ViewCellData>(Width);
        lineContents.AddRange(Enumerable.Repeat(new ViewCellData(null, null), Width));
        if (!lineDict.TryGetValue(statusType, out int lineNum)) return;
        if (lineNum >= Height) return;

        int lineY = ConvertToTopY(lineNum);
        (string text, ConsoleColor color)? lineInfo = _lines[lineNum]();
        if (lineInfo is null)
        {
            LineUpdate?.Invoke(this, new LineUpdateEventArgs(lineY, lineContents));
            return;
        }

        (string text, ConsoleColor color) = lineInfo.Value;
        for (var i = 0; i < Math.Min(Width, text.Length); i++)
            lineContents[i] = new ViewCellData((text[i], color), null);
        LineUpdate?.Invoke(this, new LineUpdateEventArgs(lineY, lineContents));
    }

    private int ConvertToTopY(int lineNum) => Height - lineNum - 1;
}