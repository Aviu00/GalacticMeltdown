using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors.Effects;


[JsonConverter(typeof(JsonSubtypes), "EffectType")]
[JsonSubtypes.KnownSubType(typeof(ContinuousEffect), "Continuous")]
[JsonSubtypes.KnownSubType(typeof(ContinuousEffect), "Flat")]
public abstract class Effect : Counter
{
    [JsonProperty] protected abstract string EffectType { get; }
    [JsonProperty] protected Actor AffectedActor;
    [JsonProperty] protected DataHolder.ActorStateChangerType EffectActionId;
    protected Action<Actor, int, int> EffectAction;
    [JsonProperty] public readonly int Power;
    [JsonIgnore] public int Duration => Timer.Value;


    protected Effect()
    {
    }

    protected Effect(Actor actor, int power, int duration, DataHolder.ActorStateChangerType effectActionId)
        : base(actor.Level, duration, duration)
    {
        Power = power;
        AffectedActor = actor;
        EffectActionId = effectActionId;
        EffectInit();
    }

    protected void EffectInit()
    {
        EffectAction = DataHolder.ActorStateChangers[EffectActionId];        
        Action = _ => RemoveEffect();    
    }
    protected virtual void RemoveEffect()
    {
        RemoveCounter();
        EffectAction = null;
        Action = null;
        AffectedActor.RemoveEffect(this);
    }
}