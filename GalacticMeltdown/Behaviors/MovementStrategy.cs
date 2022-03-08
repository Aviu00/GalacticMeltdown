using GalacticMeltdown.LevelRelated;

namespace GalacticMeltdown.Behaviors;

public class MovementStrategy : Behavior
{
    private readonly Level _level;
    private (int x, int y)? _wantsToGoTo = null;

    public MovementStrategy(Level level)
    {
        _level = level;
    }

    public override bool TryAct()
    {
        //if CurrentTarget is not null, then move towards CurrentTarget;
        //else if _wantsToGoTo is not null, then move there; else Idle movement
        Target.MoveNpcTo(Target.X + 1, Target.Y + 1);
        return true;
    }
}