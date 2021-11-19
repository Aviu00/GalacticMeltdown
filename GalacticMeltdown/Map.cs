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
        
        /// <summary>
        /// Bresenham's line algorithm
        /// </summary>
        public IEnumerable<Tile> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int yStep = y0 < y1 ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return GetTile(steep ? y : x, steep ? x : y);
                error = error - dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
    }
}