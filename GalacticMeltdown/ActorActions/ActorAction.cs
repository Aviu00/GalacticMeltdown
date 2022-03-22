using System.Collections.Generic;

namespace GalacticMeltdown.ActorActions;

public enum ActorAction
{
    Move,
    InteractWithDoor,
    Shoot,
    MeleeAttack,
    ApplyEffect,
    StopTurn,
}

public class ActorActionInfo
{
    public ActorAction Action { get; }
    public List<(int, int)> AffectedCells { get; }

    public ActorActionInfo(ActorAction action, List<(int, int)> affectedCells)
    {
        Action = action;
        AffectedCells = affectedCells;
    }
}