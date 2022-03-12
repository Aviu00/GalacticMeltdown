using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    public string LootTableId { get; init; }
    
    private readonly EnemyTypeData _typeData;
    public Enemy(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level, 
        SortedSet<Behavior> behaviors, EnemyTypeData typeData) : base(maxHp, maxEnergy, dex, def, viewRange, x, y, level, behaviors)
    {
         _typeData = typeData;
         maxHp = _typeData.MaxHp;
         maxEnergy = _typeData.MaxEnergy;
         def = _typeData.Def;
         dex = _typeData.Dex;
         viewRange = _typeData.ViewRange;
    }
    public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public string Name => _typeData.Name;
    public ConsoleColor BgColor => _typeData.BgColor;
    
    public override void TakeAction()
    {
        CurrentTarget = null;
        foreach (var target in Targets)
        {
            if (SeePoint(target.X, target.Y))
            {
                CurrentTarget = target;
                break;
            }
        }
        base.TakeAction();
    }
}