using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Behaviors;

public abstract class Behavior //strategy pattern base class
{
    protected Npc Target { get; init; }
}