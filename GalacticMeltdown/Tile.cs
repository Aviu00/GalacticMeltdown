using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    public class Tile : GameObject
    {
        public bool WasSeenByPlayer = false;
        public bool IsTransparent { get; }
        public bool IsWalkable { get; }

        public Tile(TerrainData.TileData tileData, int x, int y)
        {
            X = x;
            Y = y;
            Symbol = tileData.Symbol;
            FGColor = tileData.Color;
            IsTransparent = tileData.IsTransparent;
            IsWalkable = tileData.IsWalkable;
        }
    }
}