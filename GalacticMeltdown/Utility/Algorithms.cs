using System;
using System.Collections.Generic;

namespace GalacticMeltdown.Utility;

public static class Algorithms
{
    /// <summary>
    /// Bresenham's line algorithm
    /// </summary>
    /// <param name="maxLength">max possible length of the line; negative value or 0 for no restrictions</param>
    /// <returns></returns>
    public static IEnumerable<(int x, int y)> BresenhamGetPointsOnLine(int x0, int y0, int x1, int y1,
        int maxLength = 0)
    {
        if (maxLength > 0) maxLength++;
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
                if (difX * difX + difY * difY >= maxLength * maxLength) yield break;
            }

            yield return (x, y);

            error -= dy;
            if (error >= 0) continue;
            y += ySide * addX;
            x += xSide * addY;
            error += dx;
        }
    }

    public static IEnumerable<(int x, int y)> GetPointsOnSquareBorder(int x0, int y0, int radius)
    {
        for (int i = 0; i < radius * 2; i++)
        {
            yield return (x0 - radius + i, y0 + radius);
            yield return (x0 + radius - i, y0 - radius);
            yield return (x0 + radius, y0 + radius - i);
            yield return (x0 - radius, y0 - radius + i);
        }
    }

    /// <summary>
    /// Bresenham's circle algorithm; target for termination
    /// </summary>
    public static IEnumerable<(int x, int y)> GetPointsOnCircleBorder(int x0, int y0, int radius)
    {
        int y = radius;
        int d = 3 - 2 * radius;
        List<(int, int)> coords = new List<(int, int)>();
        FillCirclePoints(coords, x0, y0, 0, y);
        for (int x = 1; x < y; x++)
        {
            if (d > 0)
            {
                y--;
                d = d + 4 * (x - y) + 10;
            }
            else
                d = d + 4 * x + 6;

            FillCirclePoints(coords, x0, y0, x, y);
        }

        return coords;
    }

    public static void TransposeMatrix<T>(T[,] matrix)
    {
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            for (int x = y; x < matrix.GetLength(0); x++)
            {
                if (x == y) continue;
                (matrix[x, y], matrix[y, x]) = (matrix[y, x], matrix[x, y]);
            }
        }
    }

    public static void ReverseMatrixRows<T>(T[,] matrix)
    {
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            for (int x = 0; x < matrix.GetLength(0) / 2; x++)
            {
                (matrix[x, y], matrix[matrix.GetLength(0) - 1 - x, y]) =
                    (matrix[matrix.GetLength(0) - 1 - x, y], matrix[x, y]);
            }
        }
    }

    public static void ReverseMatrixCols<T>(T[,] matrix)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1) / 2; y++)
            {
                (matrix[x, y], matrix[x, matrix.GetLength(1) - 1 - y]) =
                    (matrix[x, matrix.GetLength(1) - 1 - y], matrix[x, y]);
            }
        }
    }

    //target for termination
    private static void FillCirclePoints(List<(int x, int y)> coords, int x0, int y0, int x, int y)
    {
        TransposePointsToList(coords, x0 + x, y0 + y);
        TransposePointsToList(coords, x0 - x, y0 + y);
        TransposePointsToList(coords, x0 + x, y0 - y);
        TransposePointsToList(coords, x0 - x, y0 - y);
        TransposePointsToList(coords, x0 + y, y0 + x);
        TransposePointsToList(coords, x0 - y, y0 + x);
        TransposePointsToList(coords, x0 + y, y0 - x);
        TransposePointsToList(coords, x0 - y, y0 - x);
    }

    //target for termination
    private static void TransposePointsToList(List<(int, int)> coords, int x, int y)
    {
        if (!coords.Contains((x, y)))
        {
            coords.Add((x, y));
        }
    }
    
    private static int GetDistance(int x0, int y0, int x1, int y1)
    {
        return (int)(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));
    }
    public  static LinkedList<(int, int)> AStar(int x0, int y0, int x1, int y1,
        Func<int, int, List<((int, int), int)>> getNeighbors)
    {
        LinkedList<(int, int)> path = new LinkedList<(int, int)>();
        PriorityQueue<(int, int), int> pendingPoints = new PriorityQueue<(int, int), int>();
        Dictionary<(int, int), (int, int)?> previousNodes = new Dictionary<(int, int), (int, int)?>();
        Dictionary<(int, int), int> minCosts = new Dictionary<(int, int), int>();
        pendingPoints.Enqueue((x0, y0), 0);
        previousNodes[(x0, y0)] = null;
        minCosts[(x0, y0)] = 0;
        while (pendingPoints.Count > 0)
        {
            (int x, int y) currenPoint = pendingPoints.Dequeue();
            if (currenPoint == (x1, y1))
            {
                (int, int) goal = (x1, y1);
                path.AddFirst(goal);
                while (goal != (x0, y0))
                {
                    goal = (previousNodes[goal].Value.Item1, previousNodes[goal].Value.Item2);
                    path.AddFirst(goal);
                }
            }
            foreach (((int x, int y), int moveCost) in getNeighbors.Invoke(currenPoint.x, currenPoint.y))
            {
                int newCost = moveCost + minCosts[currenPoint];
                if (!minCosts.TryGetValue((x, y), out int oldCost) || newCost < oldCost)
                {
                    minCosts[(x, y)] = newCost;
                    int priority = newCost + GetDistance(x, y, x1, y1);
                    pendingPoints.Enqueue((x, y), priority);
                    previousNodes[(x, y)] = currenPoint;
                }
            }
        }
        return null;
    }
}