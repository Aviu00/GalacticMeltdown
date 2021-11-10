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
            for (int y = -viewRange; y <= viewRange; y++)
            {
                for (int x = -viewRange; x <= viewRange; x++)
                {
                    (int, int) globalCords = Utility.GetGlobalCoordinates(x, y, this.X, this.Y);
                    Tile tile = GameManager.Map.GetTile(globalCords.Item1, globalCords.Item2);
                    if (tile == null)
                        continue;
                    tile.WasSeenByPlayer = true;
                    VisibleObjects.Add(globalCords, tile);
                }
            }
            GameManager.ConsoleManager.Redraw();
        }

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