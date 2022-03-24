using System;
using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class CellChangeEventArgs : EventArgs
{
    public List<(int, int, ViewCellData)> Cells { get; }

    public CellChangeEventArgs(List<(int, int, ViewCellData)> cells)
    {
        Cells = cells;
    }
}