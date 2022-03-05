using System.Collections;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public class MeleeEnemy : Enemy, IMoveStrategy
{
    public MoveStrategy MoveStrategy { get; set; }



    public MeleeEnemy(int x, int y, Map map, Player player) : base(x, y, map, player)
    {
        MoveStrategy = new MoveStrategy(this, Map);
    }

    protected override void UpdateLastSeenPosition()
    {
        if (SeePlayer())
        {
            LastSeenPlayerX = Player.X;
            LastSeenPlayerY = Player.Y;
        }
    }

    private void MoveToGoal(IEnumerable pathToPlayer)
    {
        int costOfWay = 0;
        int diffX = 0, diffY = 0;
        foreach ((int x, int y) coords in pathToPlayer)
        {
            costOfWay += (Map.GetTile(coords.x, coords.y)).TileMoveCost;
            if (costOfWay > this.Energy)
            {
                diffX = coords.x - this.X;
                diffY = coords.y - this.Y;
                break;
            }
        }
        MoveStrategy.Move(diffX, diffY);
    }
    
    protected override void TakeAction(int movePoints)
    {
        this.Energy = movePoints;
        IEnumerable pathToPlayer;
        if (SeePlayer())
        {
            //pathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, this.Player.X, this.Player.Y);
            pathToPlayer = MoveStrategy.AStar(X, Y, LastSeenPlayerX, LastSeenPlayerY);
            MoveToGoal(pathToPlayer);
        }
        else
        {
            //pathToPlayer = Algorithms.BresenhamGetPointsOnLine(X, Y, _lastSeenPlayerX, _lastSeenPlayerY);
            pathToPlayer =MoveStrategy.AStar(X, Y, LastSeenPlayerX, LastSeenPlayerY);
            MoveToGoal(pathToPlayer);
        }
    }
}