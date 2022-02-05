namespace GalacticMeltdown.Rendering;

public abstract class View
{
    public abstract SymbolData GetSymbol(int x, int y);

    private int _width;
    private int _height;

    protected void Resize(int width, int height)
    {
        _width = width;
        _height = height;
    }

    protected View(int initialWidth, int initialHeight)
    {
        Resize(initialWidth, initialHeight);
    }
}