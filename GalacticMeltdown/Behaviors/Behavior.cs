using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Utility;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

[JsonConverter(typeof(JsonSubtypes), "Strategy")]
[JsonSubtypes.KnownSubType(typeof(MovementStrategy), "Movement")]
[JsonSubtypes.KnownSubType(typeof(MeleeAttackStrategy), "MeleeAttack")]
[JsonSubtypes.KnownSubType(typeof(RangeAttackStrategy), "RangeAttack")]
public abstract class Behavior
{
    [JsonProperty] protected abstract string Strategy { get; }
    [JsonProperty] private readonly int _priority;

    protected Behavior()
    {
    }
    protected Behavior(int priority)
    {
        _priority = priority;
    }
    
    [JsonProperty] protected Npc ControlledNpc { get; init; }


    public abstract bool TryAct();

    public class BehaviorComparer : IComparer<Behavior>
    {
        public int Compare(Behavior x, Behavior y)
        {
            return Comparer<int>.Default.Compare(x._priority, y._priority);
        }
    }
}