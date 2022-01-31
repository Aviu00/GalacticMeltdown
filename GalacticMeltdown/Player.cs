using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown
{
    public class Player : IEntity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; }
        public ConsoleColor FgColor { get; }
        public ConsoleColor BgColor { get; }
        public Dictionary<(int, int), GameObject> VisibleObjects = new();
        public MoveBehavior MoveBehavior { get; }
        private int _viewRange = 15;

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
        public Player()
        {
            MoveBehavior = new MoveBehavior(this);
            X = 48;
            Y = 48;
            Symbol = '@';
            FgColor = ConsoleColor.White;
            BgColor = ConsoleColor.Black;
        }
        
        public void Move(int relX, int relY)
        {
            if (MoveBehavior.Move(relX, relY))
            {
                ResetVisibleObjects();
            }
        }
        

        /// <summary>
        /// Reset player field of view
        /// </summary>
        public void ResetVisibleObjects()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            VisibleObjects.Clear();
            VisibleObjects.Add((X, Y),this);
            foreach ((int x, int y) in Algorithms.GetPointsOnSquareBorder(X, Y, _viewRange))
            {
                (int x, int y)? lastTileCoords = null;
                foreach (var tileCoords in Algorithms.GetPointsOnLine(X, Y, x, y, _viewRange))
                {
                    FovCheckAdjacentWalls(lastTileCoords);
                    Tile tile = GameManager.Map.GetTile(tileCoords.x, tileCoords.y);
                    if (tile == null)
                        continue;
                    if (!VisibleObjects.ContainsKey(tileCoords))
                    {
                        VisibleObjects.Add(tileCoords, tile);
                        tile.WasSeenByPlayer = true;
                    }
                    if (!tile.IsTransparent)
                    {
                        break;
                    }

                    if (tileCoords.x != X || tileCoords.y != Y)
                        lastTileCoords = tileCoords;
                }
            }
            GameManager.ConsoleManager.RedrawMap();
            
            watch.Stop();
            Console.SetCursorPosition(0,0);
            GameManager.ConsoleManager.SetConsoleBackgroundColor(ConsoleColor.Black);
            GameManager.ConsoleManager.SetConsoleForegroundColor(ConsoleColor.White);
            Console.Write($"{watch.ElapsedMilliseconds} ms");
        }

        private void FovCheckAdjacentWalls((int x, int y)? coords)
        {
            if(coords == null)
                return;
            int x = coords.Value.x;
            int y = coords.Value.y;
            int difX = x - X;
            int dX ;
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
                Tile tile = GameManager.Map.GetTile(x, y);
                if (tile is not null && !tile.IsTransparent)
                {
                    tile.WasSeenByPlayer = true;
                    VisibleObjects.Add((x, y), tile);
                }
            }
        }
    }
}