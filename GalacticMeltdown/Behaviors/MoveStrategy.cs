using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Behaviors;

public class MoveStrategy : Behavior
{
    private readonly LevelRelated.Level _level;

    public MoveStrategy(Npc target, LevelRelated.Level level)
    {
        Target = target;
        _level = level;
    }

    public void Move(int deltaX, int deltaY)
    {
        Target.MoveNpcTo(Target.X + deltaX, Target.Y + deltaY);
    }
}