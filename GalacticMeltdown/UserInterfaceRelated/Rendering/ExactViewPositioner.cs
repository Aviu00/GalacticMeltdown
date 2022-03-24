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
        _view.Resize(Convert.ToInt32(Width * (_position.maxX - _position.minX)),
            Convert.ToInt32(Height * (_position.maxY - _position.minY)));
        ViewPositions = new List<(View, int minX, int minY, int maxX, int maxY)>
        {
            (_view, Convert.ToInt32(Width * _position.minX), Convert.ToInt32(Height * _position.minY),
                Convert.ToInt32(Width * _position.maxX), Convert.ToInt32(Height * _position.maxY))
        };
    }
}