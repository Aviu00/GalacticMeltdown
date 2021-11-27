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
                    //Console.SetCursorPosition(0, 0);
                    //Console.Write(value);
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

        /// <summary>
        /// Reset player field of view
        /// </summary>
        public void ResetVisibleObjects()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            VisibleObjects.Clear();
            VisibleObjects.Add((X, Y),this);
            foreach (var cords in Utility.GetPointsOnSquareBorder(X, Y, _viewRange))
            {
                (int, int)? lastTileCords = null;
                foreach (var tileCords in Utility.GetPointsOnLine(X, Y, cords.Item1, cords.Item2))
                {
                    CheckAdjacentWallsForPoint(lastTileCords);
                    int tx = tileCords.Item1;
                    int ty = tileCords.Item2;
                    int difX = X - tx;
                    int difY = Y - ty;
                    if (difX * difX + difY * difY > _viewRange * _viewRange -1)//maybe should do better calculations
                    {
                        break;
                    }
                    Tile tile = GameManager.Map.GetTile(tx, ty);
                    if (tile == null)
                        continue;
                    if (!VisibleObjects.ContainsKey(tileCords))
                    {
                        VisibleObjects.Add(tileCords, tile);
                        tile.WasSeenByPlayer = true;
                    }
                    if (!tile.Obj.IsTransparent)
                    {
                        break;
                    }

                    if (tx != X || ty != Y)
                        lastTileCords = tileCords;
                }
            }
            GameManager.ConsoleManager.RedrawMap();
            watch.Stop();
            Console.SetCursorPosition(0,0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{watch.ElapsedMilliseconds} ms");
        }

        private void CheckAdjacentWallsForPoint((int, int)? cords)
        {
            if(cords == null)
                return;
            int x = cords.Value.Item1;
            int y = cords.Value.Item2;
            int difX = x - X;
            int dX ;
            int difY = y - Y;
            int dY;
            if (difX == 0)
            {
                dY = difY / Math.Abs(difY);
                CheckWall(x + 1, y + dY);
                CheckWall(x, y + dY);
                CheckWall(x - 1, y + dY);
            }
            else if (difY == 0)
            {
                dX = difX / Math.Abs(difX);
                CheckWall(x + dX, y + 1);
                CheckWall(x + dX, y);
                CheckWall(x + dX, y - 1);
            }
            else
            {
                dY = difY / Math.Abs(difY);
                dX = difX / Math.Abs(difX);
                CheckWall(x + dX, y + dY);
            }
        }
        private void CheckWall(int x, int y)
        {
            if (!VisibleObjects.ContainsKey((x, y)))
            {
                Tile tile = GameManager.Map.GetTile(x, y);
                if (tile is {Obj: {IsTransparent: false}})
                {
                    tile.WasSeenByPlayer = true;
                    VisibleObjects.Add((x, y), tile);
                }
            }
        }
        public void Move(int relX, int relY)
        {
            if (MoveBehavior.Move(relX, relY))
            {
                ResetVisibleObjects();
            }
        }
    }
}