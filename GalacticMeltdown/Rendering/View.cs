namespace GalacticMeltdown.Rendering;

public abstract class View
{
    public abstract SymbolData GetSymbol(int x, int y);
    public abstract event ViewChangedEventHandler ViewChanged;

    protected int Width;
    protected int Height;

    public virtual void Resize(int width, int height)  // Note: should not fire ViewChanged events
    {
        Width = width;
        Height = height;
    }
}