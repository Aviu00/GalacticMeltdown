using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class MainViewPositioner : ViewPositioner
{
    private const double LevelViewWidth = 0.8;
    private const double MinimapHeight = 0.2;
    
    private LevelView _levelView;
    private StatusView _statusView;
    private MinimapView _minimapView;
    
    public MainViewPositioner(LevelView levelView, StatusView statusView, MinimapView minimapView)
    {
        _levelView = levelView;
        _statusView = statusView;
        _minimapView = minimapView;
    }

    public override void SetScreenSize(int width, int height)
    {
        base.SetScreenSize(width, height);
        var levelViewWidth = Convert.ToInt32(Width * LevelViewWidth);
        var minimapHeight = Convert.ToInt32(Height * MinimapHeight);
        _levelView.Resize(levelViewWidth, Height);
        _statusView.Resize(Width - levelViewWidth, Height - minimapHeight);
        _minimapView.Resize(Width - levelViewWidth, minimapHeight);
        ViewPositions = new List<(View, int minX, int minY, int maxX, int maxY)>
        {
            (_minimapView, levelViewWidth, 0, Width, minimapHeight),
            (_statusView, levelViewWidth, minimapHeight, Width, Height),
            (_levelView, 0, 0, levelViewWidth, Height),
        };
    }
}