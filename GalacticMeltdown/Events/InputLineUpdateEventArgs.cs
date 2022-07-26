using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class InputLineUpdateEventArgs
{
    public List<(int x, ViewCellData)> Cells { get; }

    public InputLineUpdateEventArgs(List<(int x, ViewCellData)> cells)
    {
        Cells = cells;
    } 
}