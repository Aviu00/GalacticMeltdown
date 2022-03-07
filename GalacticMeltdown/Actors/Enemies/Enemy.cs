namespace GalacticMeltdown.Actors.Enemies;

public abstract class Enemy : Npc
{
    protected readonly Player Player;

    protected Enemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, LevelRelated.Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        Player = level.Player;
    }
}