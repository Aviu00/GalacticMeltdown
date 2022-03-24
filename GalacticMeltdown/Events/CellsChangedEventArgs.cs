using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class CellsChangedEventArgs : EventArgs
{
    public List<(int x, int y, ViewCellData cellData, int delay)> Cells { get; }

    public CellsChangedEventArgs(List<(int, int, ViewCellData, int)> cells)
    {
        Cells = cells;
    }
}