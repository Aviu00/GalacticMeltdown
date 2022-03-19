using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    public string LootTableId { get; init; }

    private readonly EnemyTypeData _typeData;
    public override (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public string Name => _typeData.Name;
    // TODO: AlertRadius to XML
    private int AlertRadius => _typeData.AlertRadius;
    public override ConsoleColor? BgColor => _typeData.BgColor;

    public Enemy(EnemyTypeData typeData, int x, int y, Level level) : base(
        typeData.MaxHp, typeData.MaxEnergy, typeData.Dexterity, typeData.Defence, typeData.ViewRange, x, y, level)
    {
        _typeData = typeData;

        Targets = new() {level.Player};//temp
        
        if(typeData.Behaviors == null) return;
        Behaviors = new SortedSet<Behavior>(new Behavior.BehaviorComparer());
        foreach (BehaviorData behaviorData in _typeData.Behaviors)
        {
            Behavior behavior = behaviorData switch
            {
                MovementStrategyData movementStrategyData => new MovementStrategy(movementStrategyData, this),
                MeleeAttackStrategyData meleeAttackStrategyData => null, //not yet implemented
                _ => null
            };
            
            if(behavior is null) continue;
            Behaviors.Add(behavior);
        }
    }

    public override void TakeAction()
    {
        if (!IsActive) return;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        Counter alertCounter = new Counter(Level, 20);
        if (CurrentTarget is not null && alertCounter.FinishedCounting)
        {
            AlertEnemiesAboutTarget(X, Y);
            alertCounter.ResetTimer();
        }

        base.TakeAction();
    }

    private void AlertEnemiesAboutTarget(int x, int y)
    {
        (int chunkCoordX, int chunkCoordY) = Level.GetChunkCoords(x,y);
        foreach (var chunk in Level.GetChunksAround(chunkCoordX, chunkCoordY, AlertRadius / DataHolder.ChunkSize + 1))
        {
            foreach (var enemy in chunk.Enemies.
                         Where(enemy => UtilityFunctions.GetDistance(enemy.X, enemy.Y, X, Y) <= AlertRadius))
            {
                if (enemy.Behaviors is not null)
                {
                    foreach (var behavior in enemy.Behaviors)
                    {
                        if (behavior is MovementStrategy)
                        {
                            (behavior as MovementStrategy).PreviousTarget = CurrentTarget;
                        }
                    }
                }
            }
        }
    }
}