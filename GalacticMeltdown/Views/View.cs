using System;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public abstract class View
{
    protected int Width;
    protected int Height;
    
    public abstract event EventHandler NeedRedraw;

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public abstract ViewCellData GetSymbol(int x, int y);

    public abstract ViewCellData[,] GetAllCells();
}

public interface IOneCellAnim
{
    public event EventHandler<OneCellAnimEventArgs> OneCellAnim;
}

public interface IMultiCellAnim
{
    public event EventHandler<MultiCellAnimEventArgs> MultiCellAnim;
}

public interface IOneCellUpdate
{
    public event EventHandler<OneCellUpdateEventArgs> OneCellUpdate;
    /* It is guaranteed that a renderer will not ask a view about a cell that
     isn't inside of it. It is also guaranteed that a view will not report a 
     cell change in a position outside of its rectangle. However, if the window 
     size changed, it is possible that a view should report change inside of it 
     but thinks it happened outside of it. That's why it signals the renderer 
     that a change has happened but it thinks it has happened outside of it. The 
     event below is that signal. */
    public event EventHandler CellChangeOutside;
}