using System;

namespace GalacticMeltdown.Events;

public class MoveEventArgs : EventArgs
{
    public int OldX { get; }
    public int OldY { get; }

    public MoveEventArgs(int x0, int y0)
    {
        OldX = x0;
        OldY = y0;
    }
}