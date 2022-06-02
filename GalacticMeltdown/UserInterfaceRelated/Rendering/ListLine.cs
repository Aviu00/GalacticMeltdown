using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class ListLine
{
    protected const ConsoleColor DefaultColor = Colors.DefaultMain;
    protected const ConsoleColor TextColor = Colors.MenuLine.Text;
    
    protected int Width;

    public virtual void SetWidth(int width)
    {
        Width = width;
    }

    public abstract ViewCellData this[int x] { get; }
}