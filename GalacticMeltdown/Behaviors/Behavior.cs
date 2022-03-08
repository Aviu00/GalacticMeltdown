using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Behaviors;

public abstract class Behavior
{
    protected Npc Target { get; private set;  }

    public void SetTarget(Npc target)
    {
        Target ??= target;
    }

    public abstract bool TryAct();
}