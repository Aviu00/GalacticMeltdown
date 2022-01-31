namespace GalacticMeltdown.EntityBehaviors;

public abstract class Behavior //strategy pattern base class
{
    protected IEntity Target { get; init; }
}