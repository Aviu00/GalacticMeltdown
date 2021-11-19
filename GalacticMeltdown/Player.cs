using System;
using System.Collections.Generic;

namespace GalacticMeltdown
{
    public class Player : Moveable
    {
        public Dictionary<(int, int), Entity> VisibleObjects = new();
        private int viewRange = 10;
        public Player()
        {
            X = 48;
            Y = 48;
            Symbol = '@';
            //ResetVisibleObjects();
        }

        /// <summary>
        /// Just for testing, proper fov calculation method needs to be implemented
        /// </summary>
        public void ResetVisibleObjects()
        {
            VisibleObjects.Clear();
            //for (int y = -viewRange; y <= viewRange; y++)
            //{
            //    for (int x = -viewRange; x <= viewRange; x++)
            //    {
            //        (int, int) globalCords = Utility.GetGlobalCoordinates(x, y, this.X, this.Y);
            //        Tile tile = GameManager.Map.GetTile(globalCords.Item1, globalCords.Item2);
            //        if (tile == null)
            //            continue;
            //        tile.WasSeenByPlayer = true;
            //        VisibleObjects.Add(globalCords, tile);
            //    }
            //}

            GameManager.ConsoleManager.Redraw();
        }

        private void FovCheckQuadrant(int relX, int relY)
        {
            
        }

        private void FovCheckLine(int x1, int y1)
        {
            foreach (var tile in GameManager.Map.GetPointsOnLine(X,Y,x1, y1))
            {
                VisibleObjects.Add((tile.X, tile.Y), tile);
                if (!tile.Obj.IsTransparent)
                {
                    return;
                }
            }
        }
        /*private void FovCheckQuadrant(int relX, int relY)
        {
            int blockedCord = FovCheckLine(relX, relY);
            FovCheckOctant(relX + relX, relY, blockedCord);
            FovCheckOctant(relX, relY + relY, blockedCord);
        }

        private void FovCheckOctant(int relX, int relY, int blockedCord)
        {
            int innerLoopCycles = 1;
            double slope;
            for (int i = 0, y = relY; i < viewRange - 1; i++, y++)
            {
                for (int j = 0, x = relX - innerLoopCycles + 1; j < innerLoopCycles; j++, x++)
                {
                    //slope = (x - X) / (y - Y);
                    //if (x == blockedCord && y == blockedCord + 1)
                    //{
                    //}
                }
                innerLoopCycles++;
            }
        }

        /// <summary>
        /// Fov only for vertical and horizontal lines
        /// </summary>
        private int FovCheckLine(int relX, int relY)
        {
            int curX = X + relX;
            int curY = Y + relY;
            for (int i = 0; i < viewRange; i++)
            {
                Tile tile = GameManager.Map.GetTile(curX, curY);
                if (tile == null)
                    continue;
                VisibleObjects.Add((curX, curY), tile);
                if (!tile.Obj.IsTransparent)
                    return curX;
            }

            return curX;
        }*/

        public sealed override bool Move(int relX, int relY, bool redraw = true)
        {
            if (base.Move(relX, relY, false))
            {
                ResetVisibleObjects();
                return true;
            }
            return false;
        }
    }
}