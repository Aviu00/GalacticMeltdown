namespace GalacticMeltdown.Rendering;

public class WorldView : View
{
    private Map _map;
    private IHasCoords _focusObject;
    
    public WorldView(int initialWidth, int initialHeight, Map map)
        : base(initialWidth, initialHeight)
    {
        _map = map;
        _focusObject = map.Player;
    }

    public override SymbolData GetSymbol(int x, int y)
    {
        
    }
}