using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using System;

namespace GalacticMeltdown.Actors;

public class Enemy : Npc
{
    public string LootTableId { get; init; }
    protected Enemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level, params Behavior[] behaviors) 
        : base(maxHp, maxEnergy, dex, def, x, y, level, behaviors)
    {
        //temporary stuff
        SymbolData = ('W', ConsoleColor.Red);
        BgColor = null;
    }

    public override void TakeAction()
    {
        //calculate target (CurrentTarget)
        base.TakeAction();
    }
}