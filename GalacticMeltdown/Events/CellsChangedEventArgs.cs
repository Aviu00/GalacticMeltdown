using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class CellsChangedEventArgs : EventArgs
{
    public List<(int x, int y, ViewCellData cellData)> Cells { get; }
    public int Delay { get; }

    public CellsChangedEventArgs(List<(int, int, ViewCellData)> cells, int delay)
    {
        Delay = delay;
        Cells = cells;
    }
}