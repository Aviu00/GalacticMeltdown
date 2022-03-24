using System;
using System.Collections.Generic;
using GalacticMeltdown.ActorActions;

namespace GalacticMeltdown.Events;

public class ActorActionEventArgs : EventArgs
{
    public ActorAction Action { get; }
    public List<(int x, int y)> AffectedCells { get; }

    public ActorActionEventArgs(ActorAction action, List<(int, int)> affectedCells)
    {
        Action = action;
        AffectedCells = affectedCells;
    }
}