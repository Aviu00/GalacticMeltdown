namespace GalacticMeltdown.Rendering;

public abstract class View
{
    public abstract SymbolData GetSymbol(int x, int y);

    protected int Width;
    protected int Height;

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    protected View(int initialWidth, int initialHeight)
    {
        Resize(initialWidth, initialHeight);
    }
}