using System;
using System.Collections.Generic;

namespace GalacticMeltdown;

public class Player : IEntity, IControllable
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; }
    public ConsoleColor FgColor { get; }
    public ConsoleColor BgColor { get; }
    public Dictionary<(int, int), IDrawable> VisibleObjects = new();
    private int _viewRange = 15;
    private Func<int, int, Tile> _tileAt;

    public bool NoClip;//Temporary implementation of debugging "cheat codes"
    private bool _xray;
    public bool Xray
    {
        get => _xray;
        set
        {
            _xray = value;
            ResetVisibleObjects();
        }
    }
    public int ViewRange
    {
        get => _viewRange;
        set
        {
            if (value > 0)
            {
                _viewRange = value;
                ResetVisibleObjects();
            }
        }
    }

    public bool TryMove(int deltaX, int deltaY)
    {
        if (!NoClip && !_tileAt(X + deltaX, Y + deltaY).IsWalkable) return false;
        X += deltaX;
        Y += deltaY;
        ResetVisibleObjects();
        return true;
    }

    public Player(int x, int y, Func<int, int, Tile> tileAt)
    {
        X = x;
        Y = y;
        _tileAt = tileAt;
        Symbol = '@';
        FgColor = ConsoleColor.White;
        BgColor = ConsoleColor.Black;
        ResetVisibleObjects();
    }


    /// <summary>
    /// Reset player field of view
    /// </summary>
    private void ResetVisibleObjects()
    {
        VisibleObjects.Clear();
        VisibleObjects.Add((X, Y), this);
        foreach ((int x, int y) in Algorithms.GetPointsOnSquareBorder(X, Y, _viewRange))
        {
            (int x, int y)? lastTileCoords = null;
            foreach (var tileCoords in Algorithms.GetPointsOnLine(X, Y, x, y, _viewRange))
            {
                FovCheckAdjacentWalls(lastTileCoords);
                Tile tile = _tileAt(tileCoords.x, tileCoords.y);
                if (tile == null)
                {
                    if (tileCoords.x != X || tileCoords.y != Y)
                        lastTileCoords = tileCoords;
                    continue;
                }
                if (!VisibleObjects.ContainsKey(tileCoords))
                {
                    VisibleObjects.Add(tileCoords, tile);
                    tile.WasSeenByPlayer = true;
                }

                if (!Xray && !tile.IsTransparent)
                {
                    break;
                }

                if (tileCoords.x != X || tileCoords.y != Y)
                    lastTileCoords = tileCoords;
            }
        }
    }

    private void FovCheckAdjacentWalls((int x, int y)? coords)
    {
        if (coords == null)
            return;
        int x = coords.Value.x;
        int y = coords.Value.y;
        int difX = x - X;
        int dX;
        int difY = y - Y;
        int dY;
        if (difX == 0)
        {
            dY = difY / Math.Abs(difY);
            FovCheckWall(x + 1, y + dY);
            FovCheckWall(x, y + dY);
            FovCheckWall(x - 1, y + dY);
        }
        else if (difY == 0)
        {
            dX = difX / Math.Abs(difX);
            FovCheckWall(x + dX, y + 1);
            FovCheckWall(x + dX, y);
            FovCheckWall(x + dX, y - 1);
        }
        else
        {
            dY = difY / Math.Abs(difY);
            dX = difX / Math.Abs(difX);
            FovCheckWall(x + dX, y + dY);
        }
    }

    private void FovCheckWall(int x, int y)
    {
        if (!VisibleObjects.ContainsKey((x, y)))
        {
            Tile tile = _tileAt(x, y);
            if (tile is not null && !tile.IsTransparent)
            {
                tile.WasSeenByPlayer = true;
                VisibleObjects.Add((x, y), tile);
            }
        }
    }
}
