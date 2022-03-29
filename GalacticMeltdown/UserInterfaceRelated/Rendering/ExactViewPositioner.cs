using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class ExactViewPositioner : ViewPositioner
{
    private (double minX, double minY, double maxX, double maxY)? _position;
    private View _view;
    
    public ExactViewPositioner(View view, (double minX, double minY, double maxX, double maxY)? position = null)
    {
        _view = view;
        _position = position;
    }

    public override void SetScreenSize(int width, int height)
    {
        base.SetScreenSize(width, height);
        int minX = _position is null ? 0 : Convert.ToInt32(Width * _position.Value.minX);
        int minY = _position is null ? 0 : Convert.ToInt32(Height * _position.Value.minY);
        int maxX = _position is null ? Width : Convert.ToInt32(Width * _position.Value.maxX);
        int maxY = _position is null ? Height : Convert.ToInt32(Height * _position.Value.maxY);
        _view.Resize(maxX - minX, maxY - minY);
        ViewPositions = new List<(View, int minX, int minY, int maxX, int maxY)>
        {
            (_view, minX, minY, maxX, maxY)
        };
    }
}