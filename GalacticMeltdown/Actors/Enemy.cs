using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    private EnemyTypeData _typeData;
    [JsonIgnore] public override (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    [JsonIgnore] public string Name => _typeData.Name;
    [JsonIgnore] public override ConsoleColor? BgColor => _typeData.BgColor;
    [JsonProperty] public readonly string Id;

    [JsonConstructor]
    private Enemy()
    {
    }
    
    public Enemy(EnemyTypeData typeData, int x, int y, Level level) : base(
        typeData.MaxHp, typeData.MaxEnergy, typeData.Dexterity, typeData.Defence, typeData.ViewRange, x, y, level)
    {
        _typeData = typeData;
        Id = typeData.Id;
        Targets = new() {Level.Player}; //temp
        Init();
    }

    private void Init()
    {
        if (_typeData.Behaviors == null) return;
        Behaviors = new SortedSet<Behavior>(new Behavior.BehaviorComparer());
        foreach (BehaviorData behaviorData in _typeData.Behaviors)
        {
            Behavior behavior = behaviorData switch
            {
                MovementStrategyData movementStrategyData => new MovementStrategy(movementStrategyData, this),
                MeleeAttackStrategyData meleeAttackStrategyData => null, //not yet implemented
                _ => null
            };

            if (behavior is null) continue;
            Behaviors.Add(behavior);
        }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _typeData = DataHolder.EnemyTypes[Id];
        Init();
    }
    
    public override void TakeAction()
    {
        if (!IsActive) return;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        base.TakeAction();
    }
}