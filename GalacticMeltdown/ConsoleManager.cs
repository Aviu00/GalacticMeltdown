using System;
using System.Text;

namespace GalacticMeltdown;

public class ConsoleManager
{
    public IHasCoords FocusPoint;
    private int _screenCenterX;
    private int _screenCenterY;
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
        var watch = System.Diagnostics.Stopwatch.StartNew();
        int maxX = Console.WindowWidth - overlayWidth - 1;
        int maxY = Console.WindowHeight - 1;
        DrawArea(0, 0, maxX, maxY, DrawFunctions.ScreenCoordsMapDrawFunc, true);
        watch.Stop();
        Console.SetCursorPosition(0, 0);
        GameManager.ConsoleManager.SetConsoleBackgroundColor(ConsoleColor.Black);
        GameManager.ConsoleManager.SetConsoleForegroundColor(ConsoleColor.White);
        Console.Write($"{watch.ElapsedMilliseconds} ms");
    }

    public void UpdateConsoleCenterCoords()
    {
        FocusPoint ??= GameManager.Player;
        _screenCenterX = (Console.WindowWidth - overlayWidth) / 2;
        _screenCenterY = Console.WindowHeight / 2;
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
            if (!appendNewLine)
                Console.SetCursorPosition(startX, i);
            for (int x = startX; x <= maxX; x++)
            {
                DrawFunctions.SymbolData symbolData = drawFunc(x, y);

                SetConsoleForegroundColor(symbolData.FgColor);
                SetConsoleBackgroundColor(symbolData.BgColor);
                lastFgColor ??= Console.ForegroundColor;
                lastBgColor ??= Console.BackgroundColor;
                if ((lastFgColor == Console.ForegroundColor || symbolData.Symbol == ' ')
                    && lastBgColor == Console.BackgroundColor)
                {
                    Console.ForegroundColor = (ConsoleColor) lastFgColor;
                    sb.Append(symbolData.Symbol);
                }
                else
                {
                    ConsoleColor newFgColor = Console.ForegroundColor;
                    ConsoleColor newBgColor = Console.BackgroundColor;
                    SetConsoleForegroundColor((ConsoleColor) lastFgColor);
                    SetConsoleBackgroundColor((ConsoleColor) lastBgColor);
                    Console.Write(sb);
                    sb.Clear();
                    lastFgColor = newFgColor;
                    lastBgColor = newBgColor;
                    SetConsoleForegroundColor(newFgColor);
                    SetConsoleBackgroundColor(newBgColor);
                    Console.SetCursorPosition(x, i);
                    sb.Append(symbolData.Symbol);
                }
            }

            if (!appendNewLine)
            {
                Console.Write(sb);
                sb.Clear();
                lastBgColor = null;
                lastFgColor = null;
            }
            else if (sb.Length != 0)
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
        (int x, int y, DrawFunctions.DrawFunc drawFunc, bool ignoreOverlay = false, bool glCoords = false)
    {
        DrawFunctions.SymbolData symbolData = drawFunc(x, y);
        if (glCoords)
        {
            (x, y) = ConvertGlobalToScreenCoords(x, y);
        }

        if (!ignoreOverlay && x > Console.WindowWidth - overlayWidth - 1)
        {
            return;
        }

        int i = FlipYScreenCord(y);
        Console.SetCursorPosition(x, i);
        SetConsoleForegroundColor(symbolData.FgColor);
        SetConsoleBackgroundColor(symbolData.BgColor);
        Console.Write(symbolData.Symbol);
    }
    
    private void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor)
            Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor)
            Console.BackgroundColor = bgColor;
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

    public (int x, int y) ConvertGlobalToScreenCoords(int x, int y)
    {
        UpdateConsoleCenterCoords();
        (int x, int y) relCoords = Utility.ConvertGlobalToRelativeCoords(x, y, FocusPoint.X, FocusPoint.Y);
        return Utility.ConvertRelativeToGlobalCoords(relCoords.x, relCoords.y, _screenCenterX, _screenCenterY);
    }

    public (int x, int y) ConvertScreenToGlobalCoords(int x, int y)
    {
        UpdateConsoleCenterCoords();
        var relCoords = Utility.ConvertGlobalToRelativeCoords(x, y, _screenCenterX, _screenCenterY);
        return Utility.ConvertRelativeToGlobalCoords(relCoords.x, relCoords.y, FocusPoint.X, FocusPoint.Y);
    }

    private int FlipYScreenCord(int y)
    {
        return Console.WindowHeight - 1 - y;
    }
}