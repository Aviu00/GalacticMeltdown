using GalacticMeltdown.Actors;
using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private readonly Level _level;

    public MovementStrategy(Npc target, Level level)
    {
        Target = target;
        _level = level;
    }

    public void Move(int deltaX, int deltaY)
    {
        Target.MoveNpcTo(Target.X + deltaX, Target.Y + deltaY);
    }
}