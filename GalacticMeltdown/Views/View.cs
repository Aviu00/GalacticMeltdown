using System;
using GalacticMeltdown.Events;
using Newtonsoft.Json;

namespace GalacticMeltdown.Views;

public abstract class View
{
    protected int Width;
    protected int Height;
    
    public abstract event EventHandler NeedRedraw;
    public abstract event EventHandler<CellChangeEventArgs> CellsChanged;

    [JsonIgnore]
    public abstract (double, double, double, double)? WantedPosition { get; }

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public abstract ViewCellData GetSymbol(int x, int y);
}