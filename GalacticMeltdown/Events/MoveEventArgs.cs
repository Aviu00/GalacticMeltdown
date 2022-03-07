using System;

namespace GalacticMeltdown.Events;

public class MoveEventArgs : EventArgs
{
    public int X0 { get; }
    public int Y0 { get; }
    public int X1 { get; }
    public int Y1 { get; }

    public MoveEventArgs(int x0, int y0, int x1, int y1)
    {
        X0 = x0;
        Y0 = y0;
        X1 = x1;
        Y1 = y1;
    }
}