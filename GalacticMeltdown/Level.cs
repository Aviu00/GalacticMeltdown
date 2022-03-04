using GalacticMeltdown.Data;

namespace GalacticMeltdown;

public partial class Level
{
    public Player Player { get; }
    
    private Chunk[,] _chunks;

    private readonly Tile[] _southernWall;
    private readonly Tile[] _westernWall;
    private readonly Tile _cornerTile;
    
    public Level(Chunk[,] chunks, Chunk startPoint, Tile[] southernWall, Tile[] westernWall)
    {
        _cornerTile = new Tile(DataHolder.TileTypes["wall_nesw"]);
        _chunks = chunks;
        _southernWall = southernWall;
        _westernWall = westernWall;
        Player = new Player(startPoint.MapX * 25 + 12, startPoint.MapY * 25 + 12, GetTile, GetEntity);
        Enemy enemy = new MeleeEnemy(startPoint.MapX * 25 + 13, startPoint.MapY * 25 + 13, this, Player);
    }


    public Tile GetTile(int x, int y)
    {
        switch (x, y)
        {
            case(-1, -1):
                return _cornerTile;
            case(-1, >= 0):
                return y >= _westernWall.Length ? null : _westernWall[y];
            case(>= 0, -1):
                return x >= _southernWall.Length ? null : _southernWall[x];
        }
        int mapX = x / 25;
        int mapY = y / 25;
        int localX = x % 25;
        int localY = y % 25;
        if (!(x >= 0 && mapX < _chunks.GetLength(0) && y >= 0 && mapY < _chunks.GetLength(1)))
        {
            return null;
        }

        return _chunks[mapX, mapY].Tiles[localX, localY];
    }

    public IEntity GetEntity(int x, int y)
    {
        if (x == Player.X && y == Player.Y) return Player;
        int mapX = x / 25;
        int mapY = y / 25;
        if (!(x >= 0 && mapX < _chunks.GetLength(0) && y >= 0 && mapY < _chunks.GetLength(1)))
        {
            return null;
        }
        return _chunks[mapX, mapY].GetEntity(x, y);
    }

    public IDrawable GetDrawable(int x, int y)
    {
        return (IDrawable) GetEntity(x, y) ?? GetTile(x, y);
    }

    public void UpdateEnemyPosition(Enemy enemy, int oldX, int oldY)
    {
        int mapX = enemy.X / 25;
        int mapY = enemy.Y / 25;
        int oldMapX = oldX / 25;
        int oldMapY = oldY / 25;
        if (!(0 <= mapX && mapX < _chunks.GetLength(0) && 0 <= mapY && mapY < _chunks.GetLength(1))) return;
        if (mapX != oldMapX || oldY != oldMapY)
        {
            if (oldX >= 0 && oldY >= 0)
            {
                _chunks[oldMapX, oldMapY].Enemies.Remove(enemy);
            }
            _chunks[mapX, mapY].Enemies.Add(enemy);
        }
    }
}