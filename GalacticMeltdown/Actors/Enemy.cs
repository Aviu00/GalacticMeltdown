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
    [JsonProperty] protected override string ActorName => "Enemy";
    private EnemyTypeData _typeData;
    [JsonIgnore] public override (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    [JsonIgnore] public string Name => _typeData.Name;
    [JsonIgnore] public override ConsoleColor? BgColor => _typeData.BgColor;
    [JsonIgnore] private int AlertRadius => _typeData.AlertRadius;
    [JsonProperty] public readonly string Id;
    [JsonProperty] private Counter _alertCounter;

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
        _alertCounter = new Counter(Level, 1, 20);
        Died += _alertCounter.RemoveCounter;
        
        if (_typeData.Behaviors == null) return;
        
        Behaviors = new SortedSet<Behavior>(new Behavior.BehaviorComparer());
        foreach (BehaviorData behaviorData in _typeData.Behaviors)
        {
            Behavior behavior = behaviorData switch
            {
                MovementStrategyData movementStrategyData => new MovementStrategy(movementStrategyData, this),
                MeleeAttackStrategyData meleeAttackStrategyData => new MeleeAttackStrategy(meleeAttackStrategyData, this), 
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
        Died += _alertCounter.RemoveCounter;
    }

    public override void TakeAction()
    {
        if (!IsActive) return;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        if (CurrentTarget is not null && _alertCounter.FinishedCounting)
        {
            AlertEnemiesAboutTarget(X, Y);
            _alertCounter.ResetTimer();
        }

        base.TakeAction();
    }
    
    private void AlertEnemiesAboutTarget(int x, int y)
    {
        (int chunkCoordX, int chunkCoordY) = Level.GetChunkCoords(x, y);
        foreach (var chunk in Level.GetChunksAround(chunkCoordX, chunkCoordY, AlertRadius / DataHolder.ChunkSize + 1))
        {
            foreach (var enemy in chunk.Enemies.Where(enemy =>
                         UtilityFunctions.GetDistance(enemy.X, enemy.Y, X, Y) <= AlertRadius &&
                         enemy.Behaviors is not null))
            {
                foreach (var behavior in enemy.Behaviors.OfType<MovementStrategy>())
                {
                    behavior.PreviousTarget = CurrentTarget;
                }
            }
        }
    }
}