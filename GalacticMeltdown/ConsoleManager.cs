using System;
using System.Text;

namespace GalacticMeltdown;

public class ConsoleManager
{
    public IHasCoords FocusPoint;
    private int _screenCenterX;
    private int _screenCenterY;
    private const int OverlayWidth = 1;

    public ConsoleManager(IHasCoords focusPoint)
    {
        FocusPoint = focusPoint;
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
        int maxX = Console.WindowWidth - OverlayWidth - 1;
        int maxY = Console.WindowHeight - 1;
        DrawArea(0, 0, maxX, maxY, DrawFunctions.GetScreenSymbol);
        watch.Stop();
        Console.SetCursorPosition(0, 0);
        SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
        Console.Write($"{watch.ElapsedMilliseconds} ms");
    }

    public void DrawArea
        (int startX, int startY, int maxX, int maxY, DrawFunctions.GetSymbolAt getSymbolAt)
    {
        UpdateConsoleCenterCoords();
        Console.SetCursorPosition(startX, startY);
        var symbolData = getSymbolAt(startX, startY);
        StringBuilder currentSequence = new StringBuilder($"{symbolData.Symbol}");
        ConsoleColor curFgColor = symbolData.FgColor, curBgColor = symbolData.BgColor;
        int x = startX + 1;
        for (int areaY = maxY; areaY >= startY; areaY--)
        {
            int consoleY = ConvertToConsoleY(areaY);
            if (startX != 0)  // Can append new line at end of drawn line
                Console.SetCursorPosition(startX, consoleY);
            for (; x <= maxX; x++)
            {
                symbolData = getSymbolAt(x, areaY);
                if ((symbolData.FgColor != curFgColor && symbolData.Symbol != ' ' || symbolData.BgColor != curBgColor) 
                    && currentSequence.Length != 0)  // When new line isn't appended length can be 0
                {
                    SetConsoleColor(curFgColor, curBgColor);
                    Console.Write(currentSequence);
                    currentSequence.Clear();
                    curFgColor = symbolData.FgColor;
                    curBgColor = symbolData.BgColor;
                }

                currentSequence.Append(symbolData.Symbol);
            }

            x = startX;

            if (startX == 0)  // Can append new line at end of drawn line
            {
                currentSequence.Append('\n');
            }
            else
            {
                SetConsoleColor(curFgColor, curBgColor);
                Console.Write(currentSequence);
                currentSequence.Clear();
            }
        }

        if (currentSequence.Length != 0)
        {
            if (startX == 0) currentSequence.Length--; // remove new line
            SetConsoleColor(curFgColor, curBgColor);
            Console.Write(currentSequence);
        }
    }

    public void PutSymbol
        (int x, int y, DrawFunctions.GetSymbolAt getSymbolAt, bool ignoreOverlay = false, bool glCoords = false)
    {
        UpdateConsoleCenterCoords();
        DrawFunctions.SymbolData symbolData = getSymbolAt(x, y);
        if (glCoords)
        {
            (x, y) = ConvertGlobalToScreenCoords(x, y);
        }

        if (!ignoreOverlay && x > Console.WindowWidth - OverlayWidth - 1)
        {
            return;  // hidden by overlay
        }

        Console.SetCursorPosition(x, ConvertToConsoleY(y));
        SetConsoleColor(symbolData.FgColor, symbolData.BgColor);
        Console.Write(symbolData.Symbol);
    }
    
    private void UpdateConsoleCenterCoords()
    {
        _screenCenterX = (Console.WindowWidth - OverlayWidth) / 2;
        _screenCenterY = Console.WindowHeight / 2;
    }
    
    private void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor)
            Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor)
            Console.BackgroundColor = bgColor;
    }

    public (int x, int y) ConvertGlobalToScreenCoords(int x, int y)
    {
        (int x, int y) relCoords = Utility.ConvertGlobalToRelativeCoords(x, y, FocusPoint.X, FocusPoint.Y);
        return Utility.ConvertRelativeToGlobalCoords(relCoords.x, relCoords.y, _screenCenterX, _screenCenterY);
    }

    public (int x, int y) ConvertScreenToGlobalCoords(int x, int y)
    {
        var relCoords = Utility.ConvertGlobalToRelativeCoords(x, y, _screenCenterX, _screenCenterY);
        return Utility.ConvertRelativeToGlobalCoords(relCoords.x, relCoords.y, FocusPoint.X, FocusPoint.Y);
    }

    private int ConvertToConsoleY(int y)
    {
        return Console.WindowHeight - 1 - y;
    }
}