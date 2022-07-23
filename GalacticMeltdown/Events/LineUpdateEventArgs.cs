using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class LineUpdateEventArgs : EventArgs
{
    public int Y { get; }
    public List<ViewCellData> Cells { get; }

    public LineUpdateEventArgs(int y, List<ViewCellData> cells)
    {
        Y = y;
        Cells = cells;
    }
}