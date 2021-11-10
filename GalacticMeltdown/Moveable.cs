namespace GalacticMeltdown
{
    public abstract class Moveable : Entity
    {
        public virtual bool Move(int relX, int relY, bool redraw = true)
        {
            Tile tile = GameManager.Map.GetTile(X + relX, Y + relY);
            if (tile is {Obj: {IsWalkable: false}})
            {
                return false;
            }
            X += relX;
            Y += relY;
            if (redraw)
            {
                GameManager.ConsoleManager.Redraw();
            }

            return true;
        }
    }
}