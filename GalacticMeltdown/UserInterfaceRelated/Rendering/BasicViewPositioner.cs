using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class BasicViewPositioner : ViewPositioner
{
    private View _view;
    
    public BasicViewPositioner(View view)
    {
        _view = view;
    }

    public override void SetScreenSize(int width, int height)
    {
        base.SetScreenSize(width, height);
        _view.Resize(Width, Height);
        ViewPositions = new List<(View, int minX, int minY, int maxX, int maxY)> {(_view, 0, 0, Width, Height)};
    }
}