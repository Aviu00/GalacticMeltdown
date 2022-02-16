using System;

namespace GalacticMeltdown;

public abstract class Enemy : IEntity
{
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Energy = 100;
    public int _viewRadius;

    protected readonly Map Map;
    protected readonly Player Player;

    public Enemy(int x, int y, Map map, Player player)
    {
        Map = map;
        Player = player;
        X = x;
        Y = y;
        player.PerformedAction += TakeAction;
        map.UpdateEnemyPosition(this, -1, -1);
        _viewRadius = 10;
        Symbol = 'W';
        FgColor = ConsoleColor.Red;
        BgColor = ConsoleColor.Black;
    }

    protected abstract void TakeAction(int movePoints);
}