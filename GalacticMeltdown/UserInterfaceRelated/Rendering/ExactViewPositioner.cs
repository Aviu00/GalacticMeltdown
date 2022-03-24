using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class ExactViewPositioner : ViewPositioner
{
    private (double minX, double minY, double maxX, double maxY) _position;
    private View _view;
    
    public ExactViewPositioner(View view, (double minX, double minY, double maxX, double maxY) position)
    {
        _position = position;
        _view = view;
    }

    public override void SetScreenSize(int width, int height)
    {
        base.SetScreenSize(width, height);
        int minX = Convert.ToInt32(Width * _position.minX);
        int minY = Convert.ToInt32(Height * _position.minY);
        int maxX = Convert.ToInt32(Width * _position.maxX);
        int maxY = Convert.ToInt32(Height * _position.maxY);
        _view.Resize(maxX - minX, maxY - minY);
        ViewPositions = new List<(View, int minX, int minY, int maxX, int maxY)>
        {
            (_view, minX, minY, maxX, maxY)
        };
    }
}