using System;
using System.Collections.Generic;

namespace GalacticMeltdown
{
    public class Map
    {
        //multi dimensional list, remove later
        private List<List<SubMap>> submaps = new();

        public Map()
        {
            Fill();
        }

        /// <summary>
        /// Just for testing
        /// </summary>
        private void Fill()
        {
            for (int x = 0; x < 3; x++)
            {
                submaps.Add(new List<SubMap>());
                for (int y = 0; y < 3; y++)
                {
                    SubMap subMap = new SubMap(x, y);
                    submaps[x].Add(subMap);
                }
            }
        }

        /// <summary>
        /// Find a tile from coordinates
        /// </summary>
        public Tile GetTile(int x, int y)
        {
            int mapX = x / 24;
            int mapY = y / 24;
            int localX = x % 24;
            int localY = y % 24;
            if (x < 0 || mapX >= 3 || y < 0 || mapY >= 3)
            {
                return null;
            }
            return submaps[mapX][mapY].Tiles[localX, localY];
        }
    }
}