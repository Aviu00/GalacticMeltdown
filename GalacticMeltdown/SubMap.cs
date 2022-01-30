namespace GalacticMeltdown
{
    public class SubMap
    {
        public const int Size = 24;
        public Tile[,] Tiles { get; private set; }

        public int MapX { get; }
        public int MapY { get; }

        public SubMap(int x, int y)
        {
            MapX = x;
            MapY = y;
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
                    int newX = MapX * 24 + x;
                    int newY = MapY * 24 + y;
                    if(x == 1)
                        Tiles[x, y] = new Tile(GameManager.TileTypesExtractor.Data["fog"], newX, newY);
                    else if(y == 1 || x== 15 && y == 15)
                        Tiles[x, y] = new Tile(GameManager.TileTypesExtractor.Data["wall"], newX, newY);
                    else
                        Tiles[x, y] = new Tile(GameManager.TileTypesExtractor.Data["floor"], newX, newY);
                }
            }
        }
    }
}