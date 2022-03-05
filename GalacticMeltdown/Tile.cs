using System;
using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    public class Tile : IDrawable
    {
        public char Symbol { get; }
        public ConsoleColor FgColor { get; }
        public ConsoleColor BgColor { get; }
        public bool Seen = false;
        public bool IsTransparent { get; }
        public bool IsWalkable { get; }
        public bool ConnectToWalls { get; } //used in map generation
        public int TileMoveCost { get; } // temporary value

        public string Name { get; }

        public Tile(TileTypeData tileTypeData)
        {
            Symbol = tileTypeData.Symbol;
            FgColor = tileTypeData.Color;
            BgColor = ConsoleColor.Black;
            IsTransparent = tileTypeData.IsTransparent;
            IsWalkable = tileTypeData.IsWalkable;
            ConnectToWalls = tileTypeData.IsConnection;
            Name = tileTypeData.Name;
            TileMoveCost = tileTypeData.TileMoveCost;
        }
    }
}