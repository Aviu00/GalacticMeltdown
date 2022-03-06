using System.Collections.Generic;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown;

public abstract class Npc : Actor, IMoveStrategy
{
    public HashSet<Actor> Targets;
    public (int x, int y)? LastKnownTargetPosition;
    public Actor CurrentlyChasing;
    
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

    protected Npc(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        _id = Utility.RandomString(16);
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
    
    protected abstract void TakeAction();
}