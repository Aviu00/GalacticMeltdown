namespace GalacticMeltdown.Actors.Enemies;

public abstract class Enemy : Npc
{
    public string LootTableId { get; init; }
    protected Enemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, LevelRelated.Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
    }
}