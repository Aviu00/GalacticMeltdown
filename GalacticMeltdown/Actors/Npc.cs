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

    private Behavior[] Behaviors { get; init; }

    protected Npc(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level, Behavior[] behaviors) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
        foreach (Behavior behavior in behaviors)
        {
            behavior.SetTarget(this);
        }
        Behaviors = behaviors;
    }
    
    public void MoveNpcTo(int x, int y) => MoveTo(x, y);
    
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