namespace GalacticMeltdown;
public class SubMap
{
    public double Difficulty { get; }
    public Tile[,] Tiles { get; }
    public int MapX { get; }
    public int MapY { get; }

    public SubMap(Tile[,] tiles, double difficulty, int x, int y)
    {
        MapX = x;
        MapY = y;
        Tiles = tiles;
        Difficulty = difficulty;
    }
}