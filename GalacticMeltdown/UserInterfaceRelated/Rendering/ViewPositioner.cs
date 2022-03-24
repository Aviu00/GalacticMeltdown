using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class ViewPositioner
{
    protected int Width;
    protected int Height;
    protected List<(View, int minX, int minY, int maxX, int maxY)> ViewPositions;

    public virtual void SetScreenSize(int width, int height)
    {
        Width = width;
        Height = height;
    }
}