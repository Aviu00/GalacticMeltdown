using System.Collections.Generic;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Events;

public class InputLineUpdateEventArgs
{
    public List<(int x, ViewCellData cell)> Cells { get; }

    public InputLineUpdateEventArgs(List<(int x, ViewCellData)> cells)
    {
        Cells = cells;
    } 
}