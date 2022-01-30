using GalacticMeltdown.data;

namespace GalacticMeltdown;

public class Tile : GameObject
{
    public bool WasSeenByPlayer = false;

    public TerrainData.TerrainObject Obj { get; }
        
    public Tile(TerrainData.TerrainObject obj, int x, int y)
    {
        X = x;
        Y = y;
        Symbol = obj.Symbol;
        FGColor = obj.Color;
        Obj = obj;
    }
}