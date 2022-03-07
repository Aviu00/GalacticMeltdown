using System.Collections.Generic;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor, IMoveStrategy
{
    protected HashSet<Actor> Targets { get; set; }
    public (int x, int y)? LastKnownTargetPosition { get; set; }
    public Actor CurrentlyChasing { get; set; }

    private readonly string _id;

    public MoveStrategy MoveStrategy { get; set; }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }

    protected Npc(int maxHp, int maxEnergy, int dex, int def, int x, int y, LevelRelated.Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
        DoAction = TakeAction;
    }

    public void Reset()
    {
        Targets = null;
        LastKnownTargetPosition = null;
        CurrentlyChasing = null;
    }

    public void NotifyChange(Actor actor)
    {
    }

    public void MoveNpcTo(int x, int y) => MoveTo(x, y);

    protected abstract void TakeAction();
}