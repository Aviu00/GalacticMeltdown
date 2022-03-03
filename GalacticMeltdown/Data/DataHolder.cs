using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Data;

public static class DataHolder
{
    public static readonly List<Room> Rooms;
    public static readonly Dictionary<string, TileTypeData> TileTypes;
    public static readonly Dictionary<string, ConsoleColor> Colors = new()
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
    public const ConsoleColor OutOfVisionTileColor = ConsoleColor.DarkGray;
    
    static DataHolder()
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        Rooms = new RoomDataExtractor(TileTypes).Rooms;
    }

    public struct CurrentBindings
    {
        public static Dictionary<ConsoleKey, PlaySession.PlayerAction> Player = new()
        {
            {ConsoleKey.UpArrow, PlaySession.PlayerAction.MoveUp},
            {ConsoleKey.DownArrow, PlaySession.PlayerAction.MoveDown},
            {ConsoleKey.LeftArrow, PlaySession.PlayerAction.MoveLeft},
            {ConsoleKey.RightArrow, PlaySession.PlayerAction.MoveRight},
            {ConsoleKey.D8, PlaySession.PlayerAction.MoveUp},
            {ConsoleKey.D9, PlaySession.PlayerAction.MoveNe},
            {ConsoleKey.D6, PlaySession.PlayerAction.MoveRight},
            {ConsoleKey.D3, PlaySession.PlayerAction.MoveSe},
            {ConsoleKey.D2, PlaySession.PlayerAction.MoveDown},
            {ConsoleKey.D1, PlaySession.PlayerAction.MoveSw},
            {ConsoleKey.D4, PlaySession.PlayerAction.MoveLeft},
            {ConsoleKey.D7, PlaySession.PlayerAction.MoveNw},
            {ConsoleKey.Multiply, PlaySession.PlayerAction.IncreaseViewRange},
            {ConsoleKey.Subtract, PlaySession.PlayerAction.ReduceViewRange},
            {ConsoleKey.Z, PlaySession.PlayerAction.ActivateNoClip},
            {ConsoleKey.X, PlaySession.PlayerAction.ActivateXRay},
            {ConsoleKey.Q, PlaySession.PlayerAction.Quit}
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
}