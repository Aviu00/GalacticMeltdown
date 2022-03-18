using System;
using System.Linq;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class MainScreenView : View
{
    private const double LevelViewWidth = 0.8; 
    
    private int _levelViewWidth;
    
    private LevelView _levelView;
    private OverlayView _overlayView;
    
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public override (double, double, double, double)? WantedPosition => null;
    
    public MainScreenView(LevelView levelView, OverlayView overlayView)
    {
        _levelView = levelView;
        _overlayView = overlayView;
        _levelView.NeedRedraw += (_, _) => NeedRedraw?.Invoke(this, EventArgs.Empty);
        _levelView.CellsChanged += (_, args) => CellsChanged?.Invoke(this, args);
        _overlayView.NeedRedraw += (_, _) => NeedRedraw?.Invoke(this, EventArgs.Empty);
        _overlayView.CellsChanged += (_, args) =>
        {
            CellsChanged?.Invoke(this, new CellChangeEventArgs(args.Cells.Select(data =>
                {
                    var (x, y, viewCell) = data;
                    return (x - _levelViewWidth, y, viewCell);
                })
                .ToHashSet()));
        };
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        return x < _levelViewWidth ? _levelView.GetSymbol(x, y) : _overlayView.GetSymbol(x - _levelViewWidth, y);
    }

    public override void Resize(int width, int height)
    {
        _levelViewWidth = Convert.ToInt32(width * LevelViewWidth);
        _levelView.Resize(_levelViewWidth, height);
        _overlayView.Resize(width - _levelViewWidth, height);
        base.Resize(width, height);
    }
}