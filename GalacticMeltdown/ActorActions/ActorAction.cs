using System.Collections.Generic;

namespace GalacticMeltdown.ActorActions;

public enum ActorAction
{
    Move,
    InteractWithDoor,
    Shoot,
    MeleeAttackHit,
    MeleeAttackMissed,
    ApplyEffect,
    StopTurn,
}

public record ActorActionInfo(ActorAction Action, List<(int, int)> AffectedCells);