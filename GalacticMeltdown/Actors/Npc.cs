using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using System;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor
{
    public HashSet<Actor> Targets { get; set; }
    public Actor CurrentTarget { get; set; }

    private readonly string _id;

    private SortedSet<Behavior> Behaviors { get; init; }

    protected Npc(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level,
        SortedSet<Behavior> behaviors) : base(maxHp, maxEnergy, dex, def, viewRange, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
        foreach (Behavior behavior in behaviors)
        {
            behavior.SetTarget(this);
        }

        Behaviors = behaviors;
    }

    protected bool IsPointVisible(int x, int y)
    {
        if ((int) Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2)) > _viewRange)
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
        return !Algorithms
            .BresenhamGetPointsOnLine(X, Y, x, y)
            .Any(coord => Level.GetTile(coord.x, coord.y).IsTransparent is false);

        //return true;
    }

    public void MoveNpcTo(int x, int y)
    {
        MoveTo(x, y);
        Energy -= Level.GetTile(x, y).MoveCost;
    }

    public override void TakeAction()
    {
        Behaviors.Any(behavior => behavior.TryAct());
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