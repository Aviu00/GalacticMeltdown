namespace GalacticMeltdown
{
    public class Tile : GameObject
    {
        public bool WasSeenByPlayer = false;

        public TerrainData.TerrainObject Obj { get; }
        
        public Tile(TerrainData.TerrainObject obj)
        {
            //X = x;
            //Y = y;
            Symbol = obj.Symbol;
            Color = obj.Color;
            Obj = obj;
        }
    }
}