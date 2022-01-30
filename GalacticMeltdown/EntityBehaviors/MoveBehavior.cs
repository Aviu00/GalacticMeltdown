namespace GalacticMeltdown.EntityBehaviors
{
    public class MoveBehavior : Behavior
    {
        public MoveBehavior(Entity target)
        {
            Target = target;
        }
        public bool Move(int relX, int relY)
        {
            Tile tile = GameManager.Map.GetTile(Target.X + relX, Target.Y + relY);
            if (tile is not null && !tile.IsWalkable)
            {
                return false;
            }
            Target.X += relX;
            Target.Y += relY;
            return true;
        }
    }
}