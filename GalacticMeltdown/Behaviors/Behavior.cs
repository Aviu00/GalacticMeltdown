using System;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

[JsonConverter(typeof(JsonSubtypes), "Strategy")]
[JsonSubtypes.KnownSubType(typeof(MovementStrategy), "Movement")]
[JsonSubtypes.KnownSubType(typeof(MeleeAttackStrategy), "MeleeAttack")]
[JsonSubtypes.KnownSubType(typeof(RangeAttackStrategy), "RangeAttack")]
[JsonSubtypes.KnownSubType(typeof(RangeAttackStrategy), "SelfEffect")]
public abstract class Behavior : IComparable<Behavior>
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


    public abstract ActorActionInfo TryAct();
    
    public int CompareTo(Behavior obj)
    {
        return _priority.CompareTo(obj._priority);
    }
}