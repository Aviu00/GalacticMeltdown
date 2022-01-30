using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.EntityBehaviors;

namespace GalacticMeltdown
{
    public class Player : Entity, IMovable
    {
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
                (int x, int y)? lastTileCords = null;
                foreach (var tileCords in Algorithms.GetPointsOnLine(X, Y, x, y, _viewRange))
                {
                    FovCheckAdjacentWalls(lastTileCords);
                    Tile tile = GameManager.Map.GetTile(tileCords.x, tileCords.y);
                    if (tile == null)
                        continue;
                    if (!VisibleObjects.ContainsKey(tileCords))
                    {
                        VisibleObjects.Add(tileCords, tile);
                        tile.WasSeenByPlayer = true;
                    }
                    if (!tile.IsTransparent)
                    {
                        break;
                    }

                    if (tileCords.x != X || tileCords.y != Y)
                        lastTileCords = tileCords;
                }
            }
            GameManager.ConsoleManager.RedrawMap();
            
            watch.Stop();
            Console.SetCursorPosition(0,0);
            GameManager.ConsoleManager.SetConsoleBackgroundColor(ConsoleColor.Black);
            GameManager.ConsoleManager.SetConsoleForegroundColor(ConsoleColor.White);
            Console.Write($"{watch.ElapsedMilliseconds} ms");
        }

        private void FovCheckAdjacentWalls((int x, int y)? cords)
        {
            if(cords == null)
                return;
            int x = cords.Value.x;
            int y = cords.Value.y;
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