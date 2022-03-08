using System.Collections.Generic;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor
{
    public HashSet<Actor> Targets { get; set; }
    public (int x, int y)? WantsToGoTo { get; set; }
    public Actor CurrentTarget { get; set; }

    private readonly string _id;

    protected abstract List<Behavior> Behaviors { get; }

    protected Npc(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
    }
    
    public void MoveNpcTo(int x, int y) => MoveTo(x, y);
    
    public override void TakeAction()
    {
        foreach (var behavior in Behaviors)
        {
            if (behavior.TryAct())
            {
                return;
            }
        }
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