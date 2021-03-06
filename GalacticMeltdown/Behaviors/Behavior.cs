using System;
using System.Collections.Generic;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.LevelRelated;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Behaviors;

[JsonConverter(typeof(JsonSubtypes), "Strategy")]
[JsonSubtypes.KnownSubType(typeof(MovementStrategy), "Movement")]
[JsonSubtypes.KnownSubType(typeof(MeleeAttackStrategy), "MeleeAttack")]
[JsonSubtypes.KnownSubType(typeof(RangeAttackStrategy), "RangeAttack")]
[JsonSubtypes.KnownSubType(typeof(SelfEffectStrategy), "SelfEffect")]
public abstract class Behavior : IComparable<Behavior>, IHasDescription
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

    public abstract List<string> GetDescription();
}