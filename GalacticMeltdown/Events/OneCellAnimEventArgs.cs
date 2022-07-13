using System;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class OneCellAnimEventArgs : EventArgs
{
    public (int x, int y, ViewCellData cellData, int delay) CellInfo { get; }

    public OneCellAnimEventArgs((int x, int y, ViewCellData cellData, int delay) cellInfo)
    {
        CellInfo = cellInfo;
    }
}