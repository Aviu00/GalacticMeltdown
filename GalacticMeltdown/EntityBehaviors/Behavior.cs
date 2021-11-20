namespace GalacticMeltdown.EntityBehaviors
{
    public abstract class Behavior//strategy pattern base class
    {
        protected Entity Target { get; init; }
    }
}