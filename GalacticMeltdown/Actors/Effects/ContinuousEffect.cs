using System;
using System.Runtime.Serialization;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors.Effects;

public class ContinuousEffect : Effect
{
    [JsonProperty] protected override string EffectType => "Continuous";

    private void NextTurn(object _, EventArgs __)
    {
        EffectAction(AffectedActor, Power, Duration);
    }

    [JsonConstructor]
    private ContinuousEffect()
    {
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        Level.TurnFinished += NextTurn;
        EffectInit();
    }

    public ContinuousEffect (Actor actor, int power, int duration, DataHolder.ActorStateChangerType effectTypeId)
        : base(actor, power, duration, effectTypeId)
    {
        Level.TurnFinished += NextTurn;
        EffectAction(actor, power, duration);
    }
}