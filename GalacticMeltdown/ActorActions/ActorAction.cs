using System.Collections.Generic;

namespace GalacticMeltdown.ActorActions;

public class ActorAction
{
    public List<(int, int)> AffectedCells { get; set; }

    public ActorAction(List<(int, int)> affectedCells)
    {
        AffectedCells = affectedCells;
    }
}