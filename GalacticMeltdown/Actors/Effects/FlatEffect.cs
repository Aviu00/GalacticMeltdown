using System.Runtime.Serialization;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors.Effects;

public class FlatEffect : Effect
{
    [JsonProperty] protected override string EffectType => "Flat";
    
    [JsonConstructor]
    private FlatEffect()
    {
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        EffectInit();
    }

    public FlatEffect(Actor actor, int power, int duration, DataHolder.ActorStateChangerType effectTypeId)
        : base(actor, power, duration, effectTypeId)
    {
        EffectAction(actor, power, duration);
    }

    protected override void RemoveEffect()
    {
        EffectAction(AffectedActor, -Power, Duration);
        base.RemoveEffect();
    }
}