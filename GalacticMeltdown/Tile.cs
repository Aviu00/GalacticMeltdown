using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    public class Tile : GameObject
    {
        public bool WasSeenByPlayer = false;
        public bool IsTransparent { get; }
        public bool IsWalkable { get; }

        public Tile(TileTypeData tileTypeData, int x, int y)
        {
            X = x;
            Y = y;
            Symbol = tileTypeData.Symbol;
            FgColor = tileTypeData.Color;
            IsTransparent = tileTypeData.IsTransparent;
            IsWalkable = tileTypeData.IsWalkable;
        }
    }
}