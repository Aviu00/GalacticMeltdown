using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class ViewPositioner
{
    protected int Width;
    protected int Height;
    protected List<View> _views;
    protected Dictionary<View, (int minX, int minY, int maxX, int maxY)> ViewPositions;

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public (int minX, int minY, int maxX, int maxY) GetPos(View view)
    {
        return ViewPositions[view];
    }
}