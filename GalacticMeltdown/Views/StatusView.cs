using System;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Views;

public class StatusView : View
{
    private const ConsoleColor HpColor = Colors.Overlay.Hp;
    private const ConsoleColor EnergyColor = Colors.Overlay.Energy;
    private const ConsoleColor StrColor = Colors.Overlay.Strength;
    private const ConsoleColor DexColor = Colors.Overlay.Dexterity;
    private const ConsoleColor DefColor = Colors.Overlay.Defence;
    private const ConsoleColor OtherTextColor = Colors.Overlay.Other;

    private Level _level; // Maybe later
    private Player _player; // State, equipment

    private (string text, ConsoleColor color)?[] Lines =>
        new (string text, ConsoleColor color)?[]
        {
            ($"HP: {_player.Hp}/{_player.MaxHp}", HpColor),
            ($"Energy: {_player.Energy}/{_player.MaxEnergy}", EnergyColor), ($"STR: {_player.Strength}", StrColor),
            ($"DEF: {_player.Defence}", DefColor), ($"DEX: {_player.Dexterity}", DexColor),
            _player.Equipment[BodyPart.Hands] is not null
                ? ($"Held: {_player.Equipment[BodyPart.Hands].Name}", OtherTextColor)
                : null,
            _player.Equipment[BodyPart.Hands] is WeaponItem && _player.ChosenAmmoId is not null
                ? (
                    $"Ammo: {_player.Inventory.Count(item => item.Id == _player.ChosenAmmoId)} "
                    + $"({_player.Inventory.First(item => item.Id == _player.ChosenAmmoId).Name})", OtherTextColor)
                : null,
            _player.Equipment[BodyPart.Head] is not null
                ? ($"Head: {_player.Equipment[BodyPart.Head].Name}", OtherTextColor)
                : null,
            _player.Equipment[BodyPart.Torso] is not null
                ? ($"Torso: {_player.Equipment[BodyPart.Torso].Name}", OtherTextColor)
                : null,
            _player.Equipment[BodyPart.Legs] is not null
                ? ($"Legs: {_player.Equipment[BodyPart.Legs].Name}", OtherTextColor)
                : null,
            _player.Equipment[BodyPart.Feet] is not null
                ? ($"Feet: {_player.Equipment[BodyPart.Feet].Name}", OtherTextColor)
                : null,
        };

    public override event EventHandler NeedRedraw;

    public StatusView(Level level)
    {
        _level = level;
        _player = level.Player;
        _player.StatChanged += OnStatChange;
        _player.EquipmentChanged += OnEquipmentChange;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        (string text, ConsoleColor color)?[] lines = Lines;

        if (Height - y - 1 < lines.Length && lines[Height - y - 1] is not null)
        {
            var (text, color) = lines[Height - y - 1]!.Value;
            return new ViewCellData(x < text.Length ? (text[x], color) : null, null);
        }
        
        return new ViewCellData(null, null);
    }

    public override ViewCellData[,] GetAllCells()
    {
        ViewCellData[,] cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        (string text, ConsoleColor color)?[] lines = Lines;
        for (int line = 0; line < Math.Min(lines.Length, Height); line++)
        {
            if (lines[line] is null) continue;
            var (text, color) = lines[line].Value;
            for (int col = 0; col < Math.Min(text.Length, Width); col++)
            {
                cells[col, Height - line - 1] = new ViewCellData((text[col], color), null);
            }
        }
        return cells;
    }

    private void OnStatChange(object sender, StatChangeEventArgs e)
    {
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnEquipmentChange(object sender, EventArgs _)
    {
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }
}