using System;
using System.Text;

namespace GalacticMeltdown
{
    public class ConsoleManager
    {
        public Entity FocusPoint;

        public ConsoleManager()
        {
            Console.CursorVisible = false;
            Console.Clear();
        }
        
        /// <summary>
        /// Draws tiles and player currently, background colors not yet implemented
        /// </summary>
        public void Redraw()
        {
            FocusPoint ??= GameManager.Player;
            int maxX = Console.WindowWidth;
            int maxY = Console.WindowHeight;
            int focusX = maxX / 2;
            int focusY = maxY / 2;
            StringBuilder sb = new StringBuilder();
            ConsoleColor? lastFgColor = null;
            Console.SetCursorPosition(0,0);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0, y = maxY - 1; i < maxY; i++, y--)
            {
                for (int x = 0; x < maxX; x++)
                {
                    bool nullEntity = false;
                    Entity drawableEntity;
                    var relCords = Utility.GetRelativeCoordinates(x, y, focusX, focusY);
                    var glCords = Utility.GetGlobalCoordinates(relCords, FocusPoint.X, FocusPoint.Y);
                    if(GameManager.Player.X == glCords.Item1 && GameManager.Player.Y == glCords.Item2)//draw player
                    {
                        drawableEntity = GameManager.Player;
                        Console.ForegroundColor = drawableEntity.Color;
                    }
                    else if (GameManager.Player.VisibleObjects.ContainsKey(glCords))//if currently visible by player
                    {
                        drawableEntity = GameManager.Player.VisibleObjects[glCords];
                        Console.ForegroundColor = drawableEntity.Color;
                    }
                    else//draw tiles outside of fov
                    {
                        drawableEntity = GameManager.Map.GetTile(glCords.Item1, glCords.Item2);
                        if (drawableEntity != null && ((Tile) drawableEntity).WasSeenByPlayer)
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        else
                            nullEntity = true;
                    }
                    
                    if (nullEntity || drawableEntity.Symbol == ' ')
                    {
                        sb.Append(' ');
                        continue;
                    }
                    lastFgColor ??= Console.ForegroundColor;
                    if(lastFgColor == Console.ForegroundColor)
                    {
                        sb.Append(drawableEntity.Symbol);
                    }
                    else
                    {
                        ConsoleColor newColor = Console.ForegroundColor;
                        Console.ForegroundColor = (ConsoleColor)lastFgColor;
                        Console.Write(sb.ToString());
                        sb.Clear();
                        lastFgColor = newColor;
                        Console.ForegroundColor = newColor;
                        Console.SetCursorPosition(x,i);
                        sb.Append(drawableEntity.Symbol);
                    }
                }

                if (sb.ToString().Length != 0)
                {
                    if (i == maxY - 1)
                    {
                        Console.Write(sb.ToString());
                        sb.Clear();
                    }
                    else
                        sb.Append('\n');
                }
            }
        }
    }
}