using System.Runtime.Serialization;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc, IHasDescription
{
    [JsonProperty] protected override string ActorName => "Enemy";

    [JsonConstructor]
    private Enemy()
    {
    }

    public Enemy(NpcTypeData typeData, int x, int y, Level level) : base(typeData, x, y, level)
    {
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        TypeData = MapData.NpcTypes[TypeId];
        Died += AlertCounter.RemoveCounter;
    }
}