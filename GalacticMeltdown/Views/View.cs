using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Views;

public abstract class View
{
    private readonly string _id;
    
    protected int Width;
    protected int Height;
    
    public abstract event EventHandler NeedRedraw;
    public abstract event EventHandler<CellChangeEventArgs> CellsChanged;

    [JsonIgnore]
    public abstract (double, double, double, double)? WantedPosition { get; }

    protected View()
    {
        _id = UtilityFunctions.RandomString(16);
    }

    public virtual void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }
    
    public override int GetHashCode() => _id.GetHashCode();
    
    public abstract ViewCellData GetSymbol(int x, int y);
}