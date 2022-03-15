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

    public Enemy(EnemyTypeData typeData, int x, int y, Level level, SortedSet<Behavior> behaviors) : base(
        typeData.MaxHp, typeData.MaxEnergy, typeData.Dex, typeData.Def, typeData.ViewRange, x, y, level, behaviors)
    {
        _typeData = typeData;
    }

    public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public string Name => _typeData.Name;
    public ConsoleColor BgColor => _typeData.BgColor;

    public override void TakeAction()
    {
        if (!IsActive) return;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        base.TakeAction();
    }
}