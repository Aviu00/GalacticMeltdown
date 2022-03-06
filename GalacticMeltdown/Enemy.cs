namespace GalacticMeltdown;

public abstract class Enemy : Npc
{
    protected readonly Level Level;
    protected readonly Player Player;

    public Enemy(int maxHp, int maxEnergy, int dex, int def, int x, int y, Level level) 
        : base(maxHp, maxEnergy, dex, def, x, y, level)
    {
        Level = level;
        Player = level.Player;
        X = x;
        Y = y;
        level.UpdateEnemyPosition(this, -1, -1);
    }

    protected abstract void TakeAction(int movePoints);
}