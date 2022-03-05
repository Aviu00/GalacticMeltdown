using System;

namespace GalacticMeltdown;

public abstract class Enemy : IEntity
{
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Energy = 20;
    public int _viewRadius;    
    public int _lastSeenPlayerX;
    public int _lastSeenPlayerY;

    protected readonly Map Map;
    protected readonly Player Player;

    public Enemy(int x, int y, Map map, Player player)
    {
        Map = map;
        Player = player;
        X = x;
        Y = y;
        player.PerformedAction += TakeAction;
        player.PositionChangedForEnemy += UpdateLastSeenPosition;
        map.UpdateEnemyPosition(this, -1, -1);
        _viewRadius = 20;
        Symbol = 'W';
        FgColor = ConsoleColor.Red;
        BgColor = ConsoleColor.Black;
    }
    protected bool SeePlayer()
    {
        if ((int)Math.Sqrt(Math.Pow(X - Player.X, 2) + Math.Pow(Y - Player.Y, 2)) > _viewRadius)
        {
            return false;
        }
        foreach (var coords in Algorithms.BresenhamGetPointsOnLine(this.X, this.Y, Player.X, Player.Y))
        {
            if (!Map.GetTile(coords.x, coords.y).IsTransparent)
            {
                return false;
            }
        }
        return true;
    }
    protected abstract void TakeAction(int movePoints);
    protected abstract void UpdateLastSeenPosition();
}