using System;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class OneCellUpdateEventArgs : EventArgs
{
    public (int x, int y, ViewCellData cellData) CellInfo { get; }

    public OneCellUpdateEventArgs((int x, int y, ViewCellData cellData) cellInfo)
    {
        CellInfo = cellInfo;
    }
}