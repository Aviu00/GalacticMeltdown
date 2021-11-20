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
                    GameObject drawableGameObject;
                    var relCords = Utility.GetRelativeCoordinates(x, y, focusX, focusY);
                    var glCords = Utility.GetGlobalCoordinates(relCords, FocusPoint.X, FocusPoint.Y);
                    if(GameManager.Player.X == glCords.Item1 && GameManager.Player.Y == glCords.Item2)//draw player
                    {
                        drawableGameObject = GameManager.Player;
                        Console.ForegroundColor = drawableGameObject.Color;
                    }
                    else if (GameManager.Player.VisibleObjects.ContainsKey(glCords))//if currently visible by player
                    {
                        drawableGameObject = GameManager.Player.VisibleObjects[glCords];
                        Console.ForegroundColor = drawableGameObject.Color;
                    }
                    else//draw tiles outside of fov
                    {
                        drawableGameObject = GameManager.Map.GetTile(glCords.Item1, glCords.Item2);
                        if (drawableGameObject != null && ((Tile) drawableGameObject).WasSeenByPlayer)
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        else
                            nullEntity = true;
                    }
                    
                    if (nullEntity || drawableGameObject.Symbol == ' ')
                    {
                        sb.Append(' ');
                        continue;
                    }
                    lastFgColor ??= Console.ForegroundColor;
                    if(lastFgColor == Console.ForegroundColor)
                    {
                        sb.Append(drawableGameObject.Symbol);
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
                        sb.Append(drawableGameObject.Symbol);
                    }
                }

                if (sb.Length != 0)
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