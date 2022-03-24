using System;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class CellChangedEventArgs : EventArgs
{
    public (int x, int y, ViewCellData cellData, int delay) CellInfo { get; }

    public CellChangedEventArgs((int x, int y, ViewCellData cellData, int delay) cellInfo)
    {
        CellInfo = cellInfo;
    }
}