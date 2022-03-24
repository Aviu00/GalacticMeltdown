using System;
using System.Linq;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class MainScreenView : View
{
    private const double LevelViewWidth = 0.8;
    private const double MinimapHeight = 0.2;

    private int _levelViewWidth;
    private int _minimapHeight;

    private LevelView _levelView;
    private OverlayView _overlayView;
    private MinimapView _minimapView;

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;

    public override (double, double, double, double)? WantedPosition => null;

    public MainScreenView(LevelView levelView, OverlayView overlayView, MinimapView minimapView)
    {
        _levelView = levelView;
        _overlayView = overlayView;
        _minimapView = minimapView;
        
        _levelView.NeedRedraw += (_, _) => NeedRedraw?.Invoke(this, EventArgs.Empty);
        _levelView.CellsChanged += (_, args) => CellsChanged?.Invoke(this, args);
        _levelView.CellChanged += (_, args) => CellChanged?.Invoke(this, args);
        
        _overlayView.NeedRedraw += (_, _) => NeedRedraw?.Invoke(this, EventArgs.Empty);
        _overlayView.CellsChanged += (_, args) =>
        {
            CellsChanged?.Invoke(this, new CellsChangedEventArgs(args.Cells.Select(data =>
                {
                    var (x, y, viewCell, delay) = data;
                    return (x - _levelViewWidth, y, viewCell, delay);
                })
                .ToList()));
        };
        _overlayView.CellChanged += (_, args) => CellChanged?.Invoke(this, new CellChangedEventArgs((
            args.CellInfo.x - _levelViewWidth, args.CellInfo.y, args.CellInfo.cellData, args.CellInfo.delay)));
        
        _minimapView.NeedRedraw += (_, _) => NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        if (x < _levelViewWidth)
        {
            return _levelView.GetSymbol(x, y);
        }
        return y < _minimapHeight
            ? _minimapView.GetSymbol(x - _levelViewWidth, y)
            : _overlayView.GetSymbol(x - _levelViewWidth, y - _minimapHeight);
    }

    public override void Resize(int width, int height)
    {
        _levelViewWidth = Convert.ToInt32(width * LevelViewWidth);
        _minimapHeight = Convert.ToInt32(height * MinimapHeight);
        _levelView.Resize(_levelViewWidth, height);
        _overlayView.Resize(width - _levelViewWidth, height - _minimapHeight);
        _minimapView.Resize(width - _levelViewWidth, _minimapHeight);
        base.Resize(width, height);
    }
}