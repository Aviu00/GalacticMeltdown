using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
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
        TypeData = MapData.EnemyTypes[TypeId];
        Died += AlertCounter.RemoveCounter;
    }

    public override ActorActionInfo TakeAction()
    {
        if (!IsActive) return null;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        if (CurrentTarget is null || !AlertCounter.FinishedCounting) return base.TakeAction();
        
        Level.AlertEnemies(X, Y, AlertRadius, CurrentTarget);
        AlertCounter.ResetTimer();
        return base.TakeAction();
    }

    public List<string> GetDescription()
    {
        List<string> description = new()
        {
            Name,
            "",
            $"HP: {Hp}/{MaxHp}",
            $"Energy: {Energy}/{MaxEnergy}",
            $"Def: {Defence}",
            $"Dex: {Dexterity}",
            $"View Range: {ViewRange}",
            $"Alert Radius: {AlertRadius}"
        };
        if (Behaviors is null) return description;
        foreach (var behavior in Behaviors)
        {
            description.AddRange(behavior.GetDescription());
        }
        return description;
    }
}