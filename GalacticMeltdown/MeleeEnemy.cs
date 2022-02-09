using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }

    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
    }

    protected override void TakeAction(int movePoints)
    {
        //calculate actions
        if (Map.GetPointsVisibleAround(this.X, this.Y, this._viewRadius).Contains((this.Player.X, this.Player.Y)))
        {
            // temporary move logic
            if (this.X > Player.X)
            {
                MoveStrategy.Move(-1, 0);
            }
            else if (this.Y > Player.Y)
            {
                MoveStrategy.Move(0, -1);
            }
            else if(this.Y < Player.Y)
            {
                MoveStrategy.Move(0, 1);
            }
            else
            {
                MoveStrategy.Move(1, 0);
            }

        }
    }
}