using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class ListLine
{
    protected int Width;

    public virtual void SetWidth(int width)
    {
        Width = width;
    }

    public abstract ViewCellData this[int x] { get; }
}