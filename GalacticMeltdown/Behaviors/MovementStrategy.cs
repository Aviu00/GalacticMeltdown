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

    public override bool TryAct()
    {
        Target.MoveNpcTo(Target.X + 1, Target.Y + 1);
        return true;
    }
}