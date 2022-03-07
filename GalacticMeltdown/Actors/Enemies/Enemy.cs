namespace GalacticMeltdown.Actors.Enemies;

public abstract class Enemy : Npc
{
    protected readonly LevelRelated.Level Level;
    protected readonly Player Player;

    public Enemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, LevelRelated.Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        Level = level;
        Player = level.Player;
        X = x;
        Y = y;
    }
}