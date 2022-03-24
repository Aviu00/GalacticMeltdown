using System;

namespace GalacticMeltdown.Events;

public class TileChangeEventArgs : EventArgs
{
    public (int x, int y) Coords { get; }

    public TileChangeEventArgs((int, int) coords)
    {
        Coords = coords;
    }
}