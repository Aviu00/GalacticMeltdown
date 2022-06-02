using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;

namespace GalacticMeltdown.Data;

public static partial class DataHolder
{
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
            {ConsoleKey.D5, MainControl.StopTurn},
            {ConsoleKey.S, MainControl.StopTurn},
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

    public static readonly List<string> InfoLines = new()
    {
        "Galactic Meltdown",
        "",
        "You are an astronaut, caught stranded on a damaged spaceship that has been taken over by hostile alien " +
        "forces. The only way you can survive is by making a daring escape through the teleport pad located " +
        "somewhere on the ship. You have to find a way out or you will be assimilated by terrible creatures and " +
        "become one of them. You have a limited amount of time, so all you can do is grab a few spare items on the " +
        "ship and make a run for it.",
        "",
        "This game is turn-based: each entity has some energy that it can use to perform actions during a turn.", 
        "After an entity performs an action, all other entities close to the player may perform an action as well.",
        "A turn finishes when no entities spend energy when performing their action.",
        "At the end of each turn all entities restore their energy.",
        "The amount restored is the maximum possible amount available to the entity.",
        "",
        "Key bindings",
        "Use 1-9 or arrows to move, hit, or open doors",
        "Use Escape to go back",
        "",
        "In game:",
        "    Esc: open pause menu",
        "    O: get a cursor for opening doors",
        "    P: get a cursor for picking up items",
        "    U: get a cursor to shoot",
        "    C: get a cursor for examining objects",
        "    D: wait for other creatures to move",
        "    S: skip a turn",
        "    E: open inventory",
        "    A: open ammo selection menu",
        "    /: open console",
        "",
        "Cursor:",
        "    L: toggle line",
        "    F: toggle focus",
        "    Enter: interact with an object",
        "",
        "Level menu:",
        "    C: create level",
        "    D: delete level",
        "",
        "Level creation dialog:",
        "    Enter: select text field to type in",
        "    Tab: create the level",
        "",
        "Inventory:",
        "    Enter: open item dialog",
        "    Q: open equipment menu",
        "    C: open category selection",
        "    /: open item search",
    };
}