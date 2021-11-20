using System;

namespace GalacticMeltdown
{
    public class SubMap
    {
        public const int Size = 24;
        public Tile[,] Tiles { get; private set; }

        public int GlobalX { get; }
        public int GlobalY { get; }

        public SubMap(int x, int y)
        {
            GlobalX = x;
            GlobalY = y;
            Fill();
        }
        
        /// <summary>
        /// Just for testing
        /// </summary>
        void Fill()
        {
            Tiles = new Tile[Size, Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int newX = GlobalX * 24 + x;
                    int newY = GlobalY * 24 + y;
                    if(x == 1)
                        Tiles[x, y] = new Tile(GameManager.TerrainData.Data["fog"], newX, newY);
                    else if(y == 1 || x== 15 && y == 15)
                        Tiles[x, y] = new Tile(GameManager.TerrainData.Data["wall"], newX, newY);
                    else
                        Tiles[x, y] = new Tile(GameManager.TerrainData.Data["floor"], newX, newY);
                }
            }
        }
    }
}