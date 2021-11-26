using System;
using System.Text;

namespace GalacticMeltdown
{
    public class ConsoleManager
    {
        public Entity FocusPoint;
        private int focusX;
        private int focusY;

        public ConsoleManager()
        {
            Console.CursorVisible = false;
            Console.Clear();
        }
        
        /// <summary>
        /// Draws tiles, player and visible objects
        /// </summary>
        public void RedrawMap()
        {
            FocusPoint ??= GameManager.Player;
            int maxX = Console.WindowWidth;
            int maxY = Console.WindowHeight;
            focusX = maxX / 2;
            focusY = maxY / 2;
            Draw(0,0,maxX,maxY,MapDrawFunc);
        }

        private void Draw(int startX, int startY, int maxX, int maxY, DrawFunc drawFunc)
        {
            StringBuilder sb = new StringBuilder();
            ConsoleColor? lastFgColor = null;
            ConsoleColor? lastBgColor = null;
            //Console.SetCursorPosition(0,0);
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.BackgroundColor = ConsoleColor.Black;
            for (int i = startY, y = maxY - 1; i < maxY; i++, y--)
            {
                Console.SetCursorPosition(startX, i);
                for (int x = startX; x < maxX; x++)
                {
                    SymbolData symbolData = drawFunc(x, y);

                    Console.ForegroundColor = symbolData.FGColor;
                    Console.BackgroundColor = symbolData.BGColor;
                    lastFgColor ??= Console.ForegroundColor;
                    lastBgColor ??= Console.BackgroundColor;
                    if((lastFgColor == Console.ForegroundColor || symbolData.Symbol == ' ') 
                       && lastBgColor == Console.BackgroundColor)
                    {
                        sb.Append(symbolData.Symbol);
                    }
                    else
                    {
                        ConsoleColor newFgColor = Console.ForegroundColor;
                        ConsoleColor newBgColor = Console.BackgroundColor;
                        Console.ForegroundColor = (ConsoleColor)lastFgColor;
                        Console.BackgroundColor = (ConsoleColor)lastBgColor;
                        Console.Write(sb.ToString());
                        sb.Clear();
                        lastFgColor = newFgColor;
                        lastBgColor = newBgColor;
                        Console.ForegroundColor = newFgColor;
                        Console.BackgroundColor = newBgColor;
                        Console.SetCursorPosition(x,i);
                        sb.Append(symbolData.Symbol);
                    }
                }
                Console.Write(sb.ToString());
                sb.Clear();
                lastFgColor = null;
                lastBgColor = null;
            }
        }

        private SymbolData MapDrawFunc(int x, int y)
        {
            //bool nullEntity = false;
            GameObject drawableObj;
            var relCords = Utility.GetRelativeCoordinates(x, y, focusX, focusY);
            var glCords = Utility.GetGlobalCoordinates(relCords, FocusPoint.X, FocusPoint.Y);
            if(GameManager.Player.X == glCords.Item1 && GameManager.Player.Y == glCords.Item2)//draw player
            {
                drawableObj = GameManager.Player;
                return new SymbolData(drawableObj.Symbol, drawableObj.FGColor, ConsoleColor.Green);
                //Console.ForegroundColor = drawableGameObject.Color;
            }
            if (GameManager.Player.VisibleObjects.ContainsKey(glCords))//if currently visible by player
            {
                drawableObj = GameManager.Player.VisibleObjects[glCords];
                return new SymbolData(drawableObj.Symbol, drawableObj.FGColor, drawableObj.BGColor);
                //Console.ForegroundColor = drawableGameObject.Color;
            } 
            //draw tiles outside of fov
            drawableObj = GameManager.Map.GetTile(glCords.Item1, glCords.Item2);
            if (drawableObj != null && ((Tile) drawableObj).WasSeenByPlayer)
            {
                return new SymbolData(drawableObj.Symbol, ConsoleColor.DarkYellow, drawableObj.BGColor);
            }

            return new SymbolData(' ', Console.ForegroundColor, ConsoleColor.Black);
        }

        delegate SymbolData DrawFunc(int x, int y);

        private struct SymbolData
        {
            public char Symbol;
            public ConsoleColor FGColor;
            public ConsoleColor BGColor;

            public SymbolData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
            {
                Symbol = symbol;
                BGColor = bgColor;
                FGColor = fgColor;
            }
        }
    }
}