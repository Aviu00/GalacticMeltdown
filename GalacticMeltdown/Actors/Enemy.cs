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
        if (CurrentTarget is not null)
        {
            AlertEnemiesAboutPlayerPosition(X, Y);
        }

        base.TakeAction();
    }

    private void AlertEnemiesAboutPlayerPosition(int x, int y)
    {
        (int chunkCoordX, int chunkCoordY) = Level.GetChunkCoords(x,y);
        foreach (var chunk in Level.GetChunksAround(chunkCoordX, chunkCoordY, AlertRadius))
        {
            foreach (var enemy in chunk.GetNpcs().Where(npc => npc is Enemy && npc.CurrentTarget is null).ToList())
            {
                enemy.CurrentTarget = CurrentTarget;
            }
        }
    }
}