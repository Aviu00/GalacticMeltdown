using System.Collections.Generic;
using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Behaviors;

public abstract class Behavior
{
    private readonly int _priority;

    protected Behavior(int priority)
    {
        _priority = priority;
    }

    protected Npc ControlledNpc { get; init; }


    public abstract bool TryAct();

    public class BehaviorComparer : IComparer<Behavior>
    {
        public int Compare(Behavior x, Behavior y)
        {
            return Comparer<int>.Default.Compare(x._priority, y._priority);
        }
    }
}