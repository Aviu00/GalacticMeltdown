using System;
using System.Text;

namespace GalacticMeltdown;

public class ConsoleManager
{
    public Entity FocusPoint;
    private int focusX;
    private int focusY;
    public int overlayWidth = 1;

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
        DrawArea(0, 0, maxX, maxY, DrawFunctions.ScreenCordsMapDrawFunc, true);
    }

    public void ResetFocus()
    {
        FocusPoint ??= GameManager.Player;
        focusX = (Console.WindowWidth - overlayWidth) / 2;
        focusY = Console.WindowHeight / 2;
    }

    public void DrawArea
        (int startX, int startY, int maxX, int maxY, DrawFunctions.DrawFunc drawFunc, bool appendNewLine = false)
    {
        StringBuilder sb = new StringBuilder();
        ConsoleColor? lastFgColor = null;
        ConsoleColor? lastBgColor = null;
        Console.SetCursorPosition(startX, startY);
        for (int y = maxY; y >= startY; y--)
        {
            int i = FlipYScreenCord(y);
            if(!appendNewLine)
                Console.SetCursorPosition(startX, i);
            for (int x = startX; x <= maxX; x++)
            {
                DrawFunctions.SymbolData symbolData = drawFunc(x, y);

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

            if(!appendNewLine)
            {
                Console.Write(sb);
                sb.Clear();
                lastBgColor = null;
                lastFgColor = null;
            }
            else if(sb.Length != 0)
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
    }
        
    /// <summary>
    /// Draws a single obj
    /// </summary>
    public void DrawObj
        (int x, int y, DrawFunctions.DrawFunc drawFunc, bool ignoreOverlay = false, bool glCords = false)
    {
        DrawFunctions.SymbolData symbolData = drawFunc(x, y);
        if (glCords)
        {
            (x, y) = ConvertGlobalToScreenCords(x, y);
        }

        if (!ignoreOverlay && x > Console.WindowWidth - overlayWidth - 1)
        {
            return;
        }
        int i = FlipYScreenCord(y);
        Console.SetCursorPosition(x,i);
        SetConsoleForegroundColor(symbolData.FGColor);
        SetConsoleBackgroundColor(symbolData.BGColor);
        Console.Write(symbolData.Symbol);
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
        
    public (int x, int y) ConvertGlobalToScreenCords(int x, int y)
    {
        ResetFocus();
        (int x, int y) relCords = Utility.ConvertGlobalToRelativeCords(x, y, FocusPoint.X, FocusPoint.Y);
        return Utility.ConvertRelativeToGlobalCords(relCords.x, relCords.y, focusX, focusY);
    }

    public (int x, int y) ConvertScreenToGlobalCords(int x, int y)
    {
        ResetFocus();
        var relCords = Utility.ConvertGlobalToRelativeCords(x, y, focusX, focusY);
        return Utility.ConvertRelativeToGlobalCords(relCords.x, relCords.y, FocusPoint.X, FocusPoint.Y);
    }

    private int FlipYScreenCord(int y)
    {
        return Console.WindowHeight - 1 - y;
    }
}