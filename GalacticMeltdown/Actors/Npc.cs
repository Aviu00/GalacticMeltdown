using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor
{
    public HashSet<Actor> Targets { get; set; }
    public Actor CurrentTarget { get; set; }

    private readonly string _id;

    protected SortedSet<Behavior> Behaviors { get; init; }

    protected Npc(int maxHp, int maxEnergy, int dexterity, int defence, int viewRange, int x, int y, Level level)
        : base(maxHp, maxEnergy, dexterity, defence, viewRange, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
    }

    protected bool IsPointVisible(int x, int y)
    {
        if (UtilityFunctions.GetDistance(x, y, X, Y) > _viewRange)
        {
            return false;
        }

        /*
        foreach (var coords in Algorithms.BresenhamGetPointsOnLine(this.X, this.Y, x, y))
        {
            if (!Level.GetTile(coords.x, coords.y).IsTransparent)
            {
                return false;
            }
        }*/
        return Algorithms.BresenhamGetPointsOnLine(X, Y, x, y)
            .All(coord => Level.GetTile(coord.x, coord.y).IsTransparent);

        //return true;
    }

    public void MoveNpcTo(int x, int y)
    {
        if (!Level.InteractWithDoor(x, y, this, true))
            MoveTo(x, y);
    }

    public override void TakeAction()
    {
        Behaviors?.Any(behavior => behavior.TryAct());
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }
}