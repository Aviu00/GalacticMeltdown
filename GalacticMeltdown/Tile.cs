using System;
using GalacticMeltdown.Data;

namespace GalacticMeltdown
{
    public class Tile : IDrawable
    {
        public (char symbol, ConsoleColor color) SymbolData { get; }
        public ConsoleColor? BgColor { get; }
        public bool Seen = false;
        public bool IsTransparent { get; }
        public bool IsWalkable { get; }
        public bool ConnectToWalls { get; } //used in level generation
        
        public string Name { get; }

        public Tile(TileTypeData tileTypeData)
        {
            SymbolData = (tileTypeData.Symbol, tileTypeData.Color);
            BgColor = null;
            IsTransparent = tileTypeData.IsTransparent;
            IsWalkable = tileTypeData.IsWalkable;
            ConnectToWalls = tileTypeData.IsConnection;
            Name = tileTypeData.Name;
        }
    }
}