namespace GalacticMeltdown.EntityBehaviors;

public class AttackStrategy : Behavior
{
    private readonly Map _map;
    
    public AttackStrategy(Enemy target, Map map)
    {
        Target = target;
        _map = map;
    }

    public void Attack()
    {
        // attack logic
    }
}