namespace GalacticMeltdown.EntityBehaviors;

public class MoveStrategy : Behavior
{
    private readonly Level _level;
    public MoveStrategy(Enemy target, Level level)
    {
        Target = target;
        _level = level;
    }
    public void Move(int relX, int relY)
    {
        Target.X += relX;
        Target.Y += relY;
        _level.UpdateEnemyPosition(Target, Target.X - relX, Target.Y - relY);
    }

}