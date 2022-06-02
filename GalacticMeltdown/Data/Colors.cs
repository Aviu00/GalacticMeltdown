using System;

namespace GalacticMeltdown.Data;

public readonly struct Colors
{
    public readonly struct CursorBg
    {
        public const ConsoleColor Cursor = ConsoleColor.White;
        public const ConsoleColor LineNoHighlight = ConsoleColor.Red;
        public const ConsoleColor LineNonWalkable = ConsoleColor.Yellow;
        public const ConsoleColor LineItem = ConsoleColor.DarkCyan;
        public const ConsoleColor LineEnemy = ConsoleColor.DarkMagenta;
        public const ConsoleColor LineFriend = ConsoleColor.DarkGreen;
    }

    public readonly struct Overlay
    {
        public const ConsoleColor Hp = ConsoleColor.Red;
        public const ConsoleColor Energy = ConsoleColor.Yellow;
        public const ConsoleColor Strength = ConsoleColor.DarkRed;
        public const ConsoleColor Dexterity = ConsoleColor.Cyan;
        public const ConsoleColor Defence = ConsoleColor.Gray;
        public const ConsoleColor Other = ConsoleColor.Blue;
    }

    public readonly struct MenuLine
    {
        public const ConsoleColor Text = ConsoleColor.Magenta;

        public readonly struct Button
        {
            public const ConsoleColor Selected = ConsoleColor.DarkGray;
        }

        public readonly struct InputLine
        {
            public const ConsoleColor Selected = ConsoleColor.DarkGreen;
            public const ConsoleColor Pressed = ConsoleColor.Magenta;
        }
    }

    public readonly struct TextView
    {
        public const ConsoleColor Background = DefaultMain;
        public const ConsoleColor Normal = ConsoleColor.Blue;
    }

    public readonly struct Input
    {
        public const ConsoleColor Background = ConsoleColor.DarkGray;
        public const ConsoleColor Text = ConsoleColor.White;
    }
    
    public readonly struct Player
    {
        public const ConsoleColor Color = DefaultSym;
    }
    
    public readonly struct FlashAnim
    {
        public const ConsoleColor Tracer = ConsoleColor.Magenta;
        public const ConsoleColor Effect = ConsoleColor.Yellow;
        public const ConsoleColor MeleeHit = ConsoleColor.Red;
        public const ConsoleColor MeleeMiss = ConsoleColor.DarkGray;
    }
    
    public readonly struct Item
    {
        public const ConsoleColor Background = ConsoleColor.Cyan;
        public const ConsoleColor Symbol = DefaultSym;
    }
    
    public readonly struct Minimap
    {
        public const ConsoleColor Undiscovered = OutOfView;
        public const ConsoleColor Visited = ConsoleColor.DarkYellow;
        public const ConsoleColor CenterChunk = ConsoleColor.Green;
        public const ConsoleColor FinalRoom = ConsoleColor.Cyan;
    }

    public const ConsoleColor DefaultMain = ConsoleColor.Black;
    public const ConsoleColor DefaultSym = ConsoleColor.White;
    public const ConsoleColor OutOfView = ConsoleColor.DarkGray;
}