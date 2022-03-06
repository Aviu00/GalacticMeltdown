namespace GalacticMeltdown.EntityBehaviors;

public class MoveStrategy : Behavior
{
    private readonly Level _level;
    public MoveStrategy(Npc target, Level level)
    {
        Target = target;
        _level = level;
    }
    public void Move(int deltaX, int deltaY)
    {
        Target.MoveNpcTo(Target.X + deltaX, Target.Y + deltaY);
    }

}