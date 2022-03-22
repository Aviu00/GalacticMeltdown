using System;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public class MinimapView : View
{
    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

    public override (double, double, double, double)? WantedPosition => (0.8, 0.8, 1, 1);

    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData();
    }
}