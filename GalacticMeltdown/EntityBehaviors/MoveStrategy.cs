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
        var tile = _map.GetTile(Target.X + relX, Target.Y + relY); 
        if (tile.IsWalkable)
        {
            Target.X += relX;
            Target.Y += relY;
            Target.Energy -= tile.TileMoveCost;
        }
        _map.UpdateEnemyPosition(Target, Target.X - relX, Target.Y - relY);
    }

}