using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class MultiCellAnimEventArgs : EventArgs
{
    public List<(int x, int y, ViewCellData cellData)> Cells { get; }
    public int Delay { get; }

    public MultiCellAnimEventArgs(List<(int, int, ViewCellData)> cells, int delay)
    {
        Delay = delay;
        Cells = cells;
    }
}