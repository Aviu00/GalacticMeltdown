namespace GalacticMeltdown
{
    public class Tile : Entity
    {
        public bool WasSeenByPlayer = false;

        public TerrainData.TerrainObject Obj { get; }
        
        public Tile(int x, int y, TerrainData.TerrainObject obj)
        {
            X = x;
            Y = y;
            Symbol = obj.Symbol;
            Color = obj.Color;
            Obj = obj;
        }
    }
}