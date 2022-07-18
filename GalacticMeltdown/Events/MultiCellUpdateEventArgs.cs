using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class MultiCellUpdateEventArgs : EventArgs
{
    public List<(int x, int y, ViewCellData cellData)> Cells { get; }

    public MultiCellUpdateEventArgs(List<(int, int, ViewCellData)> cells)
    {
        Cells = cells;
    }
}