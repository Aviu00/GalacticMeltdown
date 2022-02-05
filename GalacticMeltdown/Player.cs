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
    
    public delegate void TakeAction(int movePoints);

    public event TakeAction OnPlayerMove;

    public bool TryMove(int deltaX, int deltaY)
    {
        if (!NoClip && !_tileAt(X + deltaX, Y + deltaY).IsWalkable) return false;
        X += deltaX;
        Y += deltaY;
        OnPlayerMove?.Invoke(100);
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
                AddVisibleAdjacentWalls(lastTileCoords);
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

    private void AddVisibleAdjacentWalls((int x, int y)? coords)
    {
        if (coords == null)
            return;
        var (x, y) = coords.Value;
        int posToPlayerX = Math.Sign(x - X);  // -1 - right, 0 - same X, 1 - left  
        int posToPlayerY = Math.Sign(y - Y);  // -1 - above, 0 - same Y, 1 - below
        if (posToPlayerX == 0)
        {
            AddVisibleWall(x + 1, y + posToPlayerY);
            AddVisibleWall(x, y + posToPlayerY);        //  .
            AddVisibleWall(x - 1, y + posToPlayerY);  // +*+
        }
        else if (posToPlayerY == 0)
        {
            AddVisibleWall(x + posToPlayerX, y + 1);  //  +
            AddVisibleWall(x + posToPlayerX, y);        // .*
            AddVisibleWall(x + posToPlayerX, y - 1);  //  +
        }
        else
        {
            AddVisibleWall(x + posToPlayerX, y + posToPlayerY);
        }
    }

    private void AddVisibleWall(int x, int y)
    {
        if (VisibleObjects.ContainsKey((x, y))) return;
        Tile tile = _tileAt(x, y);
        if (tile is null || tile.IsTransparent) return;
        tile.WasSeenByPlayer = true;
        VisibleObjects.Add((x, y), tile);
    }
}
