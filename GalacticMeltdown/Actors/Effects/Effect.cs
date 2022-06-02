using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using JsonSubTypes;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors.Effects;


[JsonConverter(typeof(JsonSubtypes), "EffectType")]
[JsonSubtypes.KnownSubType(typeof(ContinuousEffect), "Continuous")]
[JsonSubtypes.KnownSubType(typeof(FlatEffect), "Flat")]
public abstract class Effect : Counter
{
    [JsonProperty] protected abstract string EffectType { get; }
    [JsonProperty] protected Actor AffectedActor;
    [JsonProperty] protected StateChangerType EffectActionId;
    protected Action<Actor, int, int> EffectAction;
    [JsonProperty] public readonly int Power;
    [JsonIgnore] protected int Duration => Timer.Value;


    protected Effect()
    {
    }

    protected Effect(Actor actor, int power, int duration, StateChangerType effectActionId)
        : base(actor.Level, duration, duration)
    {
        if(duration <= 0) return;
        Power = power;
        AffectedActor = actor;
        EffectActionId = effectActionId;
        EffectInit();
    }

    protected void EffectInit()
    {
        EffectAction = StateChangerData.StateChangers[EffectActionId];        
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