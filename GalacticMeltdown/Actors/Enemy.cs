using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc, IHasDescription
{
    [JsonProperty] protected override string ActorName => "Enemy";
    private NpcTypeData _typeData;
    [JsonIgnore] public override (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    [JsonIgnore] public string Name => _typeData.Name;
    [JsonIgnore] public override ConsoleColor? BgColor => _typeData.BgColor;
    [JsonIgnore] private int AlertRadius => _typeData.AlertRadius;
    [JsonProperty] public readonly string TypeId;
    [JsonProperty] private Counter _alertCounter;

    [JsonConstructor]
    private Enemy()
    {
    }

    public Enemy(NpcTypeData typeData, int x, int y, Level level) : base(typeData, x, y, level)
    {
        _typeData = typeData;
        TypeId = typeData.Id;
        Targets = new HashSet<Actor> {Level.Player};
        _alertCounter = new Counter(Level, 1, 30);
        Died += _alertCounter.RemoveCounter;

        if (_typeData.Behaviors == null) return;

        Behaviors = new SortedSet<Behavior>();
        foreach (BehaviorData behaviorData in _typeData.Behaviors)
        {
            Behavior behavior = behaviorData switch
            {
                MovementStrategyData movementStrategyData => new MovementStrategy(movementStrategyData, this),
                MeleeAttackStrategyData meleeAttackStrategyData => new MeleeAttackStrategy(meleeAttackStrategyData,
                    this),
                RangeAttackStrategyData rangeAttackStrategyData => new RangeAttackStrategy(rangeAttackStrategyData,
                    this),
                SelfEffectStrategyData selfEffectStrategyData => new SelfEffectStrategy(selfEffectStrategyData, this),
                _ => null
            };

            if (behavior is null) continue;
            Behaviors.Add(behavior);
        }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _typeData = MapData.EnemyTypes[TypeId];
        Died += _alertCounter.RemoveCounter;
    }

    public override ActorActionInfo TakeAction()
    {
        if (!IsActive) return null;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        if (CurrentTarget is null || !_alertCounter.FinishedCounting) return base.TakeAction();
        
        Level.AlertEnemies(X, Y, AlertRadius, CurrentTarget);
        _alertCounter.ResetTimer();
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