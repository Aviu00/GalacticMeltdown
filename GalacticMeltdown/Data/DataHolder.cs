using System;
using System.Collections.Generic;
using System.IO;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;

namespace GalacticMeltdown.Data;

public static partial class DataHolder
{
    public const int ChunkSize = 25;
    public const int ActiveChunkRadius = 2;
    
    public static readonly string ProjectDirectory = 
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));

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
    
    public static readonly Dictionary<ItemCategory, string> CategoryName = new()
    {
        {ItemCategory.Item, "Other"},
        {ItemCategory.ConsumableItem, "Consumables"},
        {ItemCategory.WeaponItem, "Weapons"},
        {ItemCategory.EquippableItem, "Armor"},
        {ItemCategory.RangedWeaponItem, "Guns"}
    };

    public static readonly Dictionary<BodyPart, string> BodyPartName = new()
    {
        {BodyPart.Head, "Head"},
        {BodyPart.Torso, "Torso"},
        {BodyPart.Legs, "Legs"},
        {BodyPart.Feet, "Feet"},
        {BodyPart.Hands, "Hands"},
    };

    public readonly struct Colors
    {
        public const ConsoleColor OutOfVisionTileColor = ConsoleColor.DarkGray;
        public const ConsoleColor TextColor = ConsoleColor.Magenta;
        public const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
        public const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;
        public const ConsoleColor InputLineBgColorSelected = ConsoleColor.DarkGreen;
        public const ConsoleColor TextBoxDefaultBgColor = ConsoleColor.DarkGray;
        public const ConsoleColor TextBoxTextColor = ConsoleColor.White;
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
        public const ConsoleColor TextViewColor = ConsoleColor.Blue;
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
            {ConsoleKey.A, MainControl.OpenAmmoSelection},
            {ConsoleKey.O, MainControl.InteractWithDoors},
            {ConsoleKey.C, MainControl.UseCursor},
            {ConsoleKey.U, MainControl.Shoot},
            {ConsoleKey.Multiply, MainControl.IncreaseViewRange},
            {ConsoleKey.Subtract, MainControl.ReduceViewRange},
            {ConsoleKey.Escape, MainControl.OpenPauseMenu},
            {ConsoleKey.Divide, MainControl.OpenConsole},
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
            {ConsoleKey.Enter, TextInputControl.FinishInput},
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
        
        public static Dictionary<ConsoleKey, InventoryControl> InventoryMenu = new()
        {
            {ConsoleKey.UpArrow, InventoryControl.Up},
            {ConsoleKey.DownArrow, InventoryControl.Down},
            {ConsoleKey.LeftArrow, InventoryControl.Left},
            {ConsoleKey.RightArrow, InventoryControl.Right},
            {ConsoleKey.Enter, InventoryControl.Select},
            {ConsoleKey.Escape, InventoryControl.Back},
            {ConsoleKey.Q, InventoryControl.OpenEquipmentMenu},
            {ConsoleKey.C, InventoryControl.OpenCategorySelection},
            {ConsoleKey.Divide, InventoryControl.OpenSearchBox},
        };
    }
    
    static DataHolder()
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        EnemyTypes = new EnemyTypesExtractor().EnemiesTypes;
        ItemTypes = new ItemTypesExtractor().ItemTypes;
        LootTables = new LootTableDataExtractor().LootTables;
    }

    public static List<RoomType> GetRooms()
    {
        return new RoomTypesExtractor(TileTypes).Rooms;
    }
}