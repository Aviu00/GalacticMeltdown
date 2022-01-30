using GalacticMeltdown.data;

namespace GalacticMeltdown
{
    public class Tile : GameObject
    {
        public bool WasSeenByPlayer = false;

        public TerrainData.TileData Obj { get; }
        
        public Tile(TerrainData.TileData obj, int x, int y)
        {
            X = x;
            Y = y;
            Symbol = obj.Symbol;
            FGColor = obj.Color;
            Obj = obj;
        }
    }
}