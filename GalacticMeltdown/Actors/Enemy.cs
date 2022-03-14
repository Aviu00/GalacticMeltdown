using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    public string LootTableId { get; init; }

    private readonly EnemyTypeData _typeData;

    public Enemy(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level,
        SortedSet<Behavior> behaviors, EnemyTypeData typeData) : base(typeData.MaxHp, typeData.MaxEnergy, typeData.Dex,
        typeData.Def, typeData.ViewRange, x, y, level, behaviors)
    {
        _typeData = typeData;
    }

    public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public string Name => _typeData.Name;
    public ConsoleColor BgColor => _typeData.BgColor;

    public override void TakeAction()
    {
        CurrentTarget = null;
        int minDistanceToEnemy = Int32.MaxValue;
        foreach (var target in Targets)
        {
            if (IsPointVisible(target.X, target.Y) &&
                Algorithms.GetDistance(target.X, target.Y, X, Y) < minDistanceToEnemy)
            {
                CurrentTarget = target;
                minDistanceToEnemy = Algorithms.GetDistance(target.X, target.Y, X, Y);
                break;
            }
        }

        if (!IsActive) return;
        base.TakeAction();
    }
}