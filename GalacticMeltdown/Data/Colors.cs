using System;

namespace GalacticMeltdown.Data;

public static class Colors
{
    public static class CursorBg
    {
        public const ConsoleColor Cursor = ConsoleColor.White;
        public const ConsoleColor LineNoHighlight = ConsoleColor.Red;
        public const ConsoleColor LineNonWalkable = ConsoleColor.Yellow;
        public const ConsoleColor LineItem = ConsoleColor.DarkCyan;
        public const ConsoleColor LineEnemy = ConsoleColor.DarkMagenta;
        public const ConsoleColor LineFriend = ConsoleColor.DarkGreen;
    }

    public static class Overlay
    {
        public const ConsoleColor Hp = ConsoleColor.Red;
        public const ConsoleColor Energy = ConsoleColor.Yellow;
        public const ConsoleColor Strength = ConsoleColor.DarkRed;
        public const ConsoleColor Dexterity = ConsoleColor.Cyan;
        public const ConsoleColor Defence = ConsoleColor.Gray;
        public const ConsoleColor Other = ConsoleColor.Blue;
    }

    public static class MenuLine
    {
        public const ConsoleColor Text = ConsoleColor.Magenta;

        public static class Button
        {
            public const ConsoleColor Selected = ConsoleColor.DarkGray;
        }

        public static class InputLine
        {
            public const ConsoleColor Selected = ConsoleColor.DarkGreen;
            public const ConsoleColor Pressed = ConsoleColor.Magenta;
        }
    }

    public static class TextView
    {
        public const ConsoleColor Background = DefaultMain;
        public const ConsoleColor Normal = ConsoleColor.Blue;
    }

    public static class Input
    {
        public const ConsoleColor Background = ConsoleColor.DarkGray;
        public const ConsoleColor Text = ConsoleColor.White;
        public const ConsoleColor Cursor = ConsoleColor.Gray;
    }
    
    public static class Player
    {
        public const ConsoleColor Color = DefaultSym;
    }
    
    public static class FlashAnim
    {
        public const ConsoleColor Tracer = ConsoleColor.Magenta;
        public const ConsoleColor Effect = ConsoleColor.Yellow;
        public const ConsoleColor MeleeHit = ConsoleColor.Red;
        public const ConsoleColor MeleeMiss = ConsoleColor.DarkGray;
    }
    
    public static class Item
    {
        public const ConsoleColor Background = ConsoleColor.Cyan;
        public const ConsoleColor Symbol = DefaultSym;
    }
    
    public static class Minimap
    {
        public const ConsoleColor Undiscovered = OutOfView;
        public const ConsoleColor Visited = ConsoleColor.DarkYellow;
        public const ConsoleColor CenterChunk = ConsoleColor.Green;
        public const ConsoleColor FinalRoom = ConsoleColor.Cyan;
    }
    
    public static class Debug
    {
        public const ConsoleColor Main = DefaultSym;
        public const ConsoleColor Sym = DefaultMain;
    }

    public const ConsoleColor DefaultMain = ConsoleColor.Black;
    public const ConsoleColor DefaultSym = ConsoleColor.White;
    public const ConsoleColor OutOfView = ConsoleColor.DarkGray;
}