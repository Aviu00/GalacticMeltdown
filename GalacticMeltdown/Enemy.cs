using System;

namespace GalacticMeltdown;

public abstract class Enemy : IEntity
{
    public (char symbol, ConsoleColor color) SymbolData { get; }
    public ConsoleColor? BgColor { get; }
    public int X { get; set; }
    public int Y { get; set; }

    protected readonly Level Level;
    protected readonly Player Player;

    public Enemy(int x, int y, Level level, Player player)
    {
        Level = level;
        Player = player;
        X = x;
        Y = y;
        player.PerformedAction += TakeAction;
        level.UpdateEnemyPosition(this, -1, -1);

        SymbolData = ('W', ConsoleColor.Red);
        BgColor = null;
    }

    protected abstract void TakeAction(int movePoints);
}