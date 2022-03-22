using System;

namespace GalacticMeltdown.Events;

public class DoorChangeEventArgs : EventArgs
{
    public (int x, int y) Coords { get; }

    public DoorChangeEventArgs((int, int) coords)
    {
        Coords = coords;
    }
}