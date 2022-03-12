using System;
using System.Collections.Generic;
using GalacticMeltdown.InputProcessing;
using GalacticMeltdown.Launchers;

namespace GalacticMeltdown.Data;

public static class DataHolder
{
    public const int ChunkSize = 25;

    public static int CurrentSeed { get; set; }

    public static readonly List<Room> Rooms;
    public static readonly Dictionary<string, TileTypeData> TileTypes;

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

    public struct Colors
    {
        public const ConsoleColor OutOfVisionTileColor = ConsoleColor.DarkGray;
        public const ConsoleColor TextColor = ConsoleColor.Magenta;
        public const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
        public const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;
        public const ConsoleColor MenuBorderColor = ConsoleColor.Yellow;
        public const ConsoleColor CursorColor = ConsoleColor.White;
        public const ConsoleColor HighlightedNothingColor = ConsoleColor.Red;
        public const ConsoleColor HighlightedSolidTileColor = ConsoleColor.Yellow;
        public const ConsoleColor HighlightedItemColor = ConsoleColor.DarkCyan;
        public const ConsoleColor HighlightedEnemyColor = ConsoleColor.DarkMagenta;
        public const ConsoleColor HighlightedFriendColor = ConsoleColor.DarkGreen;
    }

    public struct CurrentBindings
    {
        public static Dictionary<ConsoleKey, PlayerControl> Player = new()
        {
            {ConsoleKey.UpArrow, PlayerControl.MoveUp},
            {ConsoleKey.DownArrow, PlayerControl.MoveDown},
            {ConsoleKey.LeftArrow, PlayerControl.MoveLeft},
            {ConsoleKey.RightArrow, PlayerControl.MoveRight},
            {ConsoleKey.D8, PlayerControl.MoveUp},
            {ConsoleKey.D9, PlayerControl.MoveNe},
            {ConsoleKey.D6, PlayerControl.MoveRight},
            {ConsoleKey.D3, PlayerControl.MoveSe},
            {ConsoleKey.D2, PlayerControl.MoveDown},
            {ConsoleKey.D1, PlayerControl.MoveSw},
            {ConsoleKey.D4, PlayerControl.MoveLeft},
            {ConsoleKey.D7, PlayerControl.MoveNw},
            {ConsoleKey.Multiply, PlayerControl.IncreaseViewRange},
            {ConsoleKey.Subtract, PlayerControl.ReduceViewRange},
            {ConsoleKey.Z, PlayerControl.ToggleNoClip},
            {ConsoleKey.X, PlayerControl.ToggleXRay},
            {ConsoleKey.Q, PlayerControl.Quit}
        };

        public static Dictionary<ConsoleKey, SelectionControl> Selection = new()
        {
            {ConsoleKey.UpArrow, SelectionControl.Up},
            {ConsoleKey.DownArrow, SelectionControl.Down},
            {ConsoleKey.LeftArrow, SelectionControl.Left},
            {ConsoleKey.RightArrow, SelectionControl.Right},
            {ConsoleKey.Enter, SelectionControl.Select},
            {ConsoleKey.Escape, SelectionControl.Back},
            {ConsoleKey.Tab, SelectionControl.SwitchButtonGroup},
        };
    }
    
    static DataHolder()
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        Rooms = new RoomDataExtractor(TileTypes).Rooms;
    }
}