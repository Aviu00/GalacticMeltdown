using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public static class Algorithms
{
        /// <summary>
        /// Bresenham's line algorithm
        /// </summary>
        /// <param name="maxLength">max possible length of the line; negative value or 0 for no restrictions</param>
        /// <returns></returns>
        public static IEnumerable<(int x, int y)> GetPointsOnLine(int x0, int y0, int x1, int y1, int maxLength = 0)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int addX = dx >= dy ? 1 : 0;
            int xSide = x1 > x0 ? 1 : -1;
            int ySide = y1 > y0 ? 1 : -1;
            int addY;
            int dif;
            int startingPoint;
            if (addX == 1)
            {
                addY = 0;
                dif = x1 > x0 ? x1 : 2 * x0 - x1;
                startingPoint = x0;
            }
            else
            {
                addY = 1;
                (dx, dy) = (dy, dx);
                dif = y1 > y0 ? y1 : 2 * y0 - y1;
                startingPoint = y0;
            }
            int error = dx / 2;
      
            for (int i = startingPoint, x = x0, y = y0; i <= dif; i++, x += xSide * addX, y += ySide * addY)
            {
                if (maxLength > 0)
                {
                    int difX = x - x0;
                    int difY = y - y0;
                    if(difX * difX + difY * difY >= maxLength * maxLength)
                        yield break;
                }
                yield return (x, y);
                
                error -= dy;
                if (error < 0)
                {
                    y += ySide * addX;
                    x += xSide * addY;
                    error += dx;
                }
            }
        }

        public static IEnumerable<(int x, int y)> GetPointsOnSquareBorder(int x0, int y0, int radius)
        {
            List<(int x, int y)> cords = new List<(int x, int y)>();
            int xMin = x0 - radius;
            int xMax = x0 + radius;
            int yMin = y0 - radius;
            int yMax = y0 + radius;
            for (int x = xMin; x <= xMax; x++)
            {
                cords.Add((x, yMin));
                cords.Add((x, yMax));
            }
            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                cords.Add((xMin, y));
                cords.Add((xMax, y));
            }
            
            return cords;
        }

        /// <summary>
        /// Bresenham's circle algorithm
        /// </summary>
        public static IEnumerable<(int x, int y)> GetPointsOnCircleBorder(int x0, int y0, int radius)
        {
            int y = radius;
            int d = 3 - 2 * radius;
            List<(int, int)> cords = new List<(int, int)>();
            FillCirclePoints(cords, x0, y0, 0, y);
            for (int x = 1; x < y; x++)
            {
                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                    d = d + 4 * x + 6;
                FillCirclePoints(cords, x0, y0, x, y);
            }

            return cords;
        }

        
        private static void FillCirclePoints(List<(int x, int y)> cords, int x0, int y0, int x, int y)
        {
            TransposePointsToList(cords, x0 + x, y0 + y);
            TransposePointsToList(cords, x0 - x, y0 + y);
            TransposePointsToList(cords, x0 + x, y0 - y);
            TransposePointsToList(cords, x0 - x, y0 - y);
            TransposePointsToList(cords, x0 + y, y0 + x);
            TransposePointsToList(cords, x0 - y, y0 + x);
            TransposePointsToList(cords, x0 + y, y0 - x);
            TransposePointsToList(cords, x0 - y, y0 - x);
        }

        private static void TransposePointsToList(List<(int, int)> cords, int x, int y)
        {
            if (!cords.Contains((x, y)))
            {
                cords.Add((x, y));
            }
        }
}