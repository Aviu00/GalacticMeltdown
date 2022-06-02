using System;

namespace GalacticMeltdown.Data;

public static partial class DataHolder
{
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
}