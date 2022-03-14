using System.Collections.Generic;
using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Behaviors;

public abstract class Behavior
{
    protected int _priority;
    protected Npc Target { get; private set; }

    public void SetTarget(Npc target)
    {
        Target ??= target;
    }

    public abstract bool TryAct();

    public class BehaviorComparer : IComparer<Behavior>
    {
        public int Compare(Behavior x, Behavior y)
        {
            return Comparer<int>.Default.Compare(x._priority, y._priority);
        }
    }
}