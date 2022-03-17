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
    // for debugging 
    /*public Enemy(int maxHp, int maxEnergy, int dex, int def, int viewRange, int x, int y, Level level, 
        SortedSet<Behavior> behaviors) : base(maxHp, maxEnergy, dex, def, viewRange, x, y, level, behaviors)
    {
        //temporary stuff
        SymbolData = ('W', ConsoleColor.Red);
        BgColor = ConsoleColor.Blue;
    }*/
    

    public override (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    // for debugging 
    //public (char symbol, ConsoleColor color) SymbolData { get; set; }
    public string Name => _typeData.Name;
    public override ConsoleColor? BgColor => _typeData.BgColor;
    // for debugging 
    //public ConsoleColor BgColor { get; set; }

    public override void TakeAction()
    {
        if (!IsActive) return;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        base.TakeAction();
    }
}