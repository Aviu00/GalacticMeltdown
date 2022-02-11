namespace GalacticMeltdown.EntityBehaviors;

public class MoveStrategy : Behavior
{
    private readonly Map _map;
    public MoveStrategy(Enemy target, Map map)
    {
        Target = target;
        _map = map;
    }
    public void Move(int relX, int relY)
    {
        Target.X += relX;
        Target.Y += relY;
        _map.UpdateEnemyPosition(Target, Target.X - relX, Target.Y - relY);
    }

}