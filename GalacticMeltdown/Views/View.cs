using System;
using GalacticMeltdown.Events;

namespace GalacticMeltdown.Views;

public abstract class View
{
    protected int Width;
    protected int Height;

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public abstract ViewCellData GetSymbol(int x, int y);

    public abstract ViewCellData[,] GetAllCells();
}

public interface IFullRedraw
{
    public event EventHandler NeedRedraw;
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
}

public interface IMultiCellUpdate
{
    public event EventHandler<MultiCellUpdateEventArgs> MultiCellUpdate;
}

public interface ILineUpdate
{
    public event EventHandler<LineUpdateEventArgs> LineUpdate;
}