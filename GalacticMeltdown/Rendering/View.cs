using System;

namespace GalacticMeltdown.Rendering;

public abstract class View
{
    public abstract ViewCellData GetSymbol(int x, int y);
    public abstract event ViewChangedEventHandler ViewChanged;
    public abstract event CellsChangedEventHandler CellsChanged;

    protected int Width;
    protected int Height;

    public virtual void Resize(int width, int height)  // Note: should not fire ViewChanged events
    {
        Width = width;
        Height = height;
    }
}