using System;
using System.Collections.Generic;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;

namespace GalacticMeltdown.Data;

public static partial class DataHolder
{
    public const int ChunkSize = 25;
    public const int ActiveChunkRadius = 2;

    public static int CurrentSeed { get; set; }

    public static readonly Dictionary<string, TileTypeData> TileTypes;
    public static readonly Dictionary<string, ItemData> ItemTypes;
    public static readonly Dictionary<string, ILoot> LootTables;
    public static readonly Dictionary<string, EnemyTypeData> EnemyTypes;
    
    public static readonly Dictionary<string, ConsoleColor> ColorName = new()
    {
        {"white", ConsoleColor.White},
        {"black", ConsoleColor.Black},
        {"blue", ConsoleColor.Blue},
        {"cyan", ConsoleColor.Cyan},
        {"green", ConsoleColor.Green},
        {"gray", ConsoleColor.Gray},
        {"magenta", ConsoleColor.Magenta},
        {"red", ConsoleColor.Red},
        {"yellow", ConsoleColor.Yellow},
        {"dark_gray", ConsoleColor.DarkGray},
        {"dark_blue", ConsoleColor.DarkBlue},
        {"dark_cyan", ConsoleColor.DarkCyan},
        {"dark_green", ConsoleColor.DarkGreen},
        {"dark_magenta", ConsoleColor.DarkMagenta},
        {"dark_red", ConsoleColor.DarkRed},
        {"dark_yellow", ConsoleColor.DarkYellow},
    };

    public readonly struct Colors
    {
        public const ConsoleColor OutOfVisionTileColor = ConsoleColor.DarkGray;
        public const ConsoleColor TextColor = ConsoleColor.Magenta;
        public const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
        public const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;
        public const ConsoleColor InputLineBgColorSelected = ConsoleColor.DarkGreen;
        public const ConsoleColor InputLineBgColorUnselected = ConsoleColor.DarkGray;
        public const ConsoleColor InputLineTextColor = ConsoleColor.White;
        public const ConsoleColor MenuBorderColor = ConsoleColor.Yellow;
        public const ConsoleColor CursorColor = ConsoleColor.White;
        public const ConsoleColor HighlightedNothingColor = ConsoleColor.Red;
        public const ConsoleColor HighlightedSolidTileColor = ConsoleColor.Yellow;
        public const ConsoleColor HighlightedItemColor = ConsoleColor.DarkCyan;
        public const ConsoleColor HighlightedEnemyColor = ConsoleColor.DarkMagenta;
        public const ConsoleColor HighlightedFriendColor = ConsoleColor.DarkGreen;
        public const ConsoleColor HpColor = ConsoleColor.Red;
        public const ConsoleColor EnergyColor = ConsoleColor.Yellow;
        public const ConsoleColor StrColor = ConsoleColor.DarkRed;
        public const ConsoleColor DexColor = ConsoleColor.Cyan;
        public const ConsoleColor DefColor = ConsoleColor.Gray;
        public const ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    }

    public struct CurrentBindings
    {
        public static Dictionary<ConsoleKey, MainControl> Main = new()
        {
            {ConsoleKey.UpArrow, MainControl.MoveUp},
            {ConsoleKey.DownArrow, MainControl.MoveDown},
            {ConsoleKey.LeftArrow, MainControl.MoveLeft},
            {ConsoleKey.RightArrow, MainControl.MoveRight},
            {ConsoleKey.D8, MainControl.MoveUp},
            {ConsoleKey.D9, MainControl.MoveNe},
            {ConsoleKey.D6, MainControl.MoveRight},
            {ConsoleKey.D3, MainControl.MoveSe},
            {ConsoleKey.D2, MainControl.MoveDown},
            {ConsoleKey.D1, MainControl.MoveSw},
            {ConsoleKey.D4, MainControl.MoveLeft},
            {ConsoleKey.D7, MainControl.MoveNw},
            {ConsoleKey.S, MainControl.StopTurn},
            {ConsoleKey.D5, MainControl.StopTurn},
            {ConsoleKey.D, MainControl.DoNothing},
            {ConsoleKey.P, MainControl.PickUpItem},
            {ConsoleKey.E, MainControl.OpenInventory},
            {ConsoleKey.O, MainControl.InteractWithDoors},
            {ConsoleKey.C, MainControl.UseCursor},
            {ConsoleKey.Multiply, MainControl.IncreaseViewRange},
            {ConsoleKey.Subtract, MainControl.ReduceViewRange},
            {ConsoleKey.Z, MainControl.ToggleNoClip},
            {ConsoleKey.X, MainControl.ToggleXRay},
            {ConsoleKey.Escape, MainControl.OpenPauseMenu}
        };

        public static Dictionary<ConsoleKey, SelectionControl> Selection = new()
        {
            {ConsoleKey.UpArrow, SelectionControl.Up},
            {ConsoleKey.DownArrow, SelectionControl.Down},
            {ConsoleKey.LeftArrow, SelectionControl.Left},
            {ConsoleKey.RightArrow, SelectionControl.Right},
            {ConsoleKey.Enter, SelectionControl.Select},
            {ConsoleKey.Escape, SelectionControl.Back},
            {ConsoleKey.Tab, SelectionControl.SpecialAction},
        };

        public static Dictionary<ConsoleKey, CursorControl> Cursor = new()
        {
            {ConsoleKey.UpArrow, CursorControl.MoveUp},
            {ConsoleKey.DownArrow, CursorControl.MoveDown},
            {ConsoleKey.LeftArrow, CursorControl.MoveLeft},
            {ConsoleKey.RightArrow, CursorControl.MoveRight},
            {ConsoleKey.D8, CursorControl.MoveUp},
            {ConsoleKey.D9, CursorControl.MoveNe},
            {ConsoleKey.D6, CursorControl.MoveRight},
            {ConsoleKey.D3, CursorControl.MoveSe},
            {ConsoleKey.D2, CursorControl.MoveDown},
            {ConsoleKey.D1, CursorControl.MoveSw},
            {ConsoleKey.D4, CursorControl.MoveLeft},
            {ConsoleKey.D7, CursorControl.MoveNw},
            {ConsoleKey.Enter, CursorControl.Interact},
            {ConsoleKey.Escape, CursorControl.Back},
            {ConsoleKey.L, CursorControl.ToggleLine},
            {ConsoleKey.F, CursorControl.ToggleFocus},
        };

        public static Dictionary<ConsoleKey, TextInputControl> TextInput = new()
        {
            {ConsoleKey.Backspace, TextInputControl.DeleteCharacter},
            {ConsoleKey.Escape, TextInputControl.Back},
        };

        public static Dictionary<ConsoleKey, LevelMenuControl> LevelMenu = new()
        {
            {ConsoleKey.UpArrow, LevelMenuControl.SelectPrev},
            {ConsoleKey.DownArrow, LevelMenuControl.SelectNext},
            {ConsoleKey.Enter, LevelMenuControl.Start},
            {ConsoleKey.Escape, LevelMenuControl.GoBack},
            {ConsoleKey.C, LevelMenuControl.Create},
            {ConsoleKey.D, LevelMenuControl.Delete},
        };
    }
    
    static DataHolder()
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        EnemyTypes = new EnemyTypesExtractor().EnemiesTypes;
        ItemTypes = new ItemTypesExtractor().ItemTypes;
        LootTables = new LootTableDataExtractor().LootTables;
    }

    public static List<RoomTypes> GetRooms()
    {
        return new RoomTypesExtractor(TileTypes).Rooms;
    }
}