namespace GalacticMeltdown.EntityBehaviors;

public interface IMovable
{
    MoveBehavior MoveBehavior { get; }

    public void Move(int relX, int relY);
}