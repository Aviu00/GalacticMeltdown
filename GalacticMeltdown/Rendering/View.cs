using System;

namespace GalacticMeltdown.Rendering;

public abstract class View
{
    public abstract ViewCellData GetSymbol(int x, int y);
    public abstract event ViewChangedEventHandler ViewChanged;
    public abstract event CellsChangedEventHandler CellsChanged;

    protected View()
    {
        _id = Utility.RandomString(16);
    }

    protected int Width;
    protected int Height;
    private readonly string _id;

    public virtual void Resize(int width, int height)  // Note: should not fire ViewChanged events
    {
        Width = width;
        Height = height;
    }

    public override int GetHashCode() => _id.GetHashCode();

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }
}