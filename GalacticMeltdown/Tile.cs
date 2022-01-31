using System;
using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    public class Tile : IDrawable, IImmovable
    {
        public char Symbol { get; }
        public ConsoleColor FgColor { get; }
        public ConsoleColor BgColor { get; }
        public int X { get; }
        public int Y { get; }
        public bool WasSeenByPlayer = false;
        public bool IsTransparent { get; }
        public bool IsWalkable { get; }

        public Tile(TileTypeData tileTypeData, int x, int y)
        {
            X = x;
            Y = y;
            Symbol = tileTypeData.Symbol;
            FgColor = tileTypeData.Color;
            BgColor = ConsoleColor.Black;
            IsTransparent = tileTypeData.IsTransparent;
            IsWalkable = tileTypeData.IsWalkable;
        }
    }
}