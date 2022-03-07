using System;
using System.Collections.Generic;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public class CellChangeEventArgs : EventArgs
{
    public HashSet<(int, int, ViewCellData)> Cells { get; }

    public CellChangeEventArgs(HashSet<(int, int, ViewCellData)> cells)
    {
        Cells = cells;
    }
}