using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public partial class Level
{
    public HashSet<(int, int)> GetPointsVisibleAround(int x0, int y0, int radius, bool xray = false)
    {
        var visiblePoints = new HashSet<(int, int)> {(x0, y0)};
        foreach ((int x, int y) in Algorithms.GetPointsOnSquareBorder(x0, y0, radius))
        {
            (int x, int y)? prevTileCoords = null;
            foreach (var pointCoords in Algorithms.BresenhamGetPointsOnLine(x0, y0, x, y, radius))
            {
                AddVisibleAdjacentWalls(prevTileCoords);
                if (!visiblePoints.Contains(pointCoords)) visiblePoints.Add(pointCoords);
                if (!(pointCoords.x == x0 && pointCoords.y == y0)) prevTileCoords = pointCoords;
                Tile tile = GetTile(pointCoords.x, pointCoords.y);
                if (tile is null) continue;
                if (!(tile.IsTransparent || xray)) break;
            }
        }

        return visiblePoints;
        
        void AddVisibleAdjacentWalls((int x, int y)? coords)
        {
            if (coords is null)
                return;
            var (x, y) = coords.Value;
            int posToPlayerX = Math.Sign(x - x0);  // -1 - right, 0 - same X, 1 - left  
            int posToPlayerY = Math.Sign(y - y0);  // -1 - above, 0 - same Y, 1 - below
            if (posToPlayerX == 0)
            {
                AddVisibleWall(x + 1, y + posToPlayerY);
                AddVisibleWall(x, y + posToPlayerY);      //  .
                AddVisibleWall(x - 1, y + posToPlayerY);  // +*+
            }
            else if (posToPlayerY == 0)
            {
                AddVisibleWall(x + posToPlayerX, y + 1);  //  +
                AddVisibleWall(x + posToPlayerX, y);      // .*
                AddVisibleWall(x + posToPlayerX, y - 1);  //  +
            }
            else
            {
                AddVisibleWall(x + posToPlayerX, y + posToPlayerY);
            }
        }

        void AddVisibleWall(int x, int y)
        {
            if (visiblePoints.Contains((x, y))) return;
            Tile tile = GetTile(x, y);
            if (tile is null || tile.IsTransparent) return;
            visiblePoints.Add((x, y));
        }
    }
}