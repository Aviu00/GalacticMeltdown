using System;
using System.Text;

namespace GalacticMeltdown
{
    public class ConsoleManager
    {
        public Entity FocusPoint;
        private int focusX;
        private int focusY;
        public int overlayWidth = 0;

        public ConsoleManager()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }
        
        /// <summary>
        /// Draws tiles, player and visible objects
        /// </summary>
        public void RedrawMap()
        {
            //ResetFocus();
            int maxX = Console.WindowWidth - overlayWidth-1;
            int maxY = Console.WindowHeight-1;
            DrawArea(0, 0, maxX, maxY, ScreenToMapDrawFunc, true);
        }

        public void ResetFocus()
        {
            FocusPoint ??= GameManager.Player;
            focusX = (Console.WindowWidth - overlayWidth) / 2;
            focusY = Console.WindowHeight / 2;
        }

        public void DrawArea(int startX, int startY, int maxX, int maxY, DrawFunc drawFunc, bool appendNewLine = false)
        {
            StringBuilder sb = new StringBuilder();
            ConsoleColor? lastFgColor = null;
            ConsoleColor? lastBgColor = null;
            Console.SetCursorPosition(startX, startY);
            for (int y = maxY; y >= startY; y--)
            {
                int i = Console.WindowHeight - 1 - y;
                if(!appendNewLine)
                    Console.SetCursorPosition(startX, i);
                for (int x = startX; x <= maxX; x++)
                {
                    SymbolData symbolData = drawFunc(x, y);

                    SetConsoleForegroundColor(symbolData.FGColor);
                    SetConsoleBackgroundColor(symbolData.BGColor);
                    lastFgColor ??= Console.ForegroundColor;
                    lastBgColor ??= Console.BackgroundColor;
                    if((lastFgColor == Console.ForegroundColor || symbolData.Symbol == ' ') 
                       && lastBgColor == Console.BackgroundColor)
                    {
                        Console.ForegroundColor = (ConsoleColor)lastFgColor;
                        sb.Append(symbolData.Symbol);
                    }
                    else
                    {
                        ConsoleColor newFgColor = Console.ForegroundColor;
                        ConsoleColor newBgColor = Console.BackgroundColor;
                        SetConsoleForegroundColor((ConsoleColor)lastFgColor);
                        SetConsoleBackgroundColor((ConsoleColor)lastBgColor);
                        Console.Write(sb);
                        sb.Clear();
                        lastFgColor = newFgColor;
                        lastBgColor = newBgColor;
                        SetConsoleForegroundColor(newFgColor);
                        SetConsoleBackgroundColor(newBgColor);
                        Console.SetCursorPosition(x,i);
                        sb.Append(symbolData.Symbol);
                    }
                }

                if (appendNewLine)
                {
                    if (sb.Length != 0)
                    {
                        if (i == maxY)
                        {
                            Console.Write(sb);
                            sb.Clear();
                        }
                        else
                            sb.Append('\n');
                    }
                }
                else
                {
                    Console.Write(sb);
                    sb.Clear();
                    lastBgColor = null;
                    lastFgColor = null;
                }
            }
        }
        
        /// <summary>
        /// Draws a single obj
        /// </summary>
        public void DrawObj(int x, int y, DrawFunc drawFunc, bool glCords = false)
        {
            SymbolData symbolData = drawFunc(x, y);
            if (glCords)
            {
                (x, y) = GlobalCordsToScreen(x, y);
            }
            int i = Console.WindowHeight - 1 - y;
            Console.SetCursorPosition(x,i);
            SetConsoleForegroundColor(symbolData.FGColor);
            SetConsoleBackgroundColor(symbolData.BGColor);
            Console.Write(symbolData.Symbol);
        }

        public SymbolData ScreenToMapDrawFunc(int x, int y)
        {
            (int, int) glCords = ScreenCordsToGlobal(x, y);
            return GlCordsDrawFunc(glCords.Item1, glCords.Item2);
        }

        public SymbolData GlCordsDrawFunc(int x, int y)
        {
            GameObject drawableObj;
            if (GameManager.Player.VisibleObjects.ContainsKey((x,y)))//if currently visible by player
            {
                drawableObj = GameManager.Player.VisibleObjects[(x,y)];
                return new SymbolData(drawableObj.Symbol, drawableObj.FGColor, drawableObj.BGColor);
                //Console.ForegroundColor = drawableGameObject.Color;
            } 
            //draw tiles outside of fov
            drawableObj = GameManager.Map.GetTile(x, y);
            if (drawableObj != null && ((Tile) drawableObj).WasSeenByPlayer)
            {
                return new SymbolData(drawableObj.Symbol, ConsoleColor.DarkYellow, ConsoleColor.Black);
            }

            return new SymbolData(' ', Console.ForegroundColor, ConsoleColor.Black);
        }

        public delegate SymbolData DrawFunc(int x, int y);

        public (int, int) GlobalCordsToScreen(int x, int y)
        {
            ResetFocus();
            (int, int) relCords = Utility.GetRelativeCoordinates(x, y, FocusPoint.X, FocusPoint.Y);
            return Utility.GetGlobalCoordinates(relCords, focusX, focusY);
        }

        public void SetConsoleForegroundColor(ConsoleColor color)
        {
            if (Console.ForegroundColor != color)
                Console.ForegroundColor = color;
        }
        public void SetConsoleBackgroundColor(ConsoleColor color)
        {
            if (Console.BackgroundColor != color)
                Console.BackgroundColor = color;
        }

        public (int, int) ScreenCordsToGlobal(int x, int y)
        {
            ResetFocus();
            var relCords = Utility.GetRelativeCoordinates(x, y, focusX, focusY);
            return Utility.GetGlobalCoordinates(relCords, FocusPoint.X, FocusPoint.Y);
        }
        
        public struct SymbolData
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