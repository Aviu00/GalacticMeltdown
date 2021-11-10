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
            for (int y = 0; y < 3; y++)
            {
                submaps.Add(new List<SubMap>());
                for (int x = 0; x < 3; x++)
                {
                    SubMap subMap = new SubMap(x, y);
                    submaps[y].Add(subMap);
                }
            }
        }

        /// <summary>
        /// Find a tile from coordinates
        /// </summary>
        public Tile GetTile(int x, int y)
        {
            int globalX = x / 24;
            int globalY = y / 24;
            int localX = x % 24;
            int localY = y % 24;
            Tile tile;
            try
            {
                tile = submaps[globalY][globalX].Tiles[localX, localY];
            }
            catch (ArgumentOutOfRangeException)
            {
                tile = null;
            }
            catch (IndexOutOfRangeException)
            {
                tile = null;
            }
            return tile;
        }
    }
}