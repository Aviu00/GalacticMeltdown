using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticMeltdown.Rendering;

public class Renderer
{
    public IHasCoords FocusPoint;
    private int _screenCenterX;
    private int _screenCenterY;
    private const int OverlayWidth = 1;
    private const int TotalWidth = 1000;
    private List<(View, int, int, int, int)> _views;  // View, top-left and bottom-right corner coords (rel) 

    public Renderer(IHasCoords focusPoint)
    {
        FocusPoint = focusPoint;
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
    }

    public void AddView(View view, int x0, int y0, int x1, int y1)
    {
        _views.Add((view, x0, y0, x1, y1));
    }

    public void RemoveLastView()
    {
        if (_views.Any())
        {
            _views.RemoveAt(_views.Count - 1);
        }
    }

    public void ClearViews()
    {
        _views.Clear();
    }

    public void Redraw()
    {
        foreach (var (view, x0, y0, x1, y1) in _views)
        {
            int x0Real = (int) Math.Round(Console.WindowWidth * x0 / (double) TotalWidth);
            int y0Real = (int) Math.Round(Console.WindowWidth * y0 / (double) TotalWidth);
            int x1Real = Math.Min((int) Math.Round(Console.WindowWidth * x1 / (double) TotalWidth), 
                Console.WindowWidth - 1);
            int y1Real = Math.Min((int) Math.Round(Console.WindowWidth * y1 / (double) TotalWidth),
                Console.WindowWidth - 1);
            int width = x1Real - x0Real, height = y1Real - y0Real;
            view.Resize(width, height);
            DrawArea(x0, y0, x0 + width, y0 + height, 
                (x, y) => view.GetSymbol(x - x0, y - y0));
        }
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
            if (startX != 0)  // Can't append new line at end of drawn line
                Console.SetCursorPosition(startX, ConvertToConsoleY(areaY));
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

    public void PutSymbolFrom(int x, int y, DrawFunctions.GetSymbolAt getSymbolAt)
    {
        UpdateConsoleCenterCoords();
        SymbolData symbolData = getSymbolAt(x, y);
        (x, y) = ConvertGlobalToScreenCoords(x, y);
        if (x > Console.WindowWidth - OverlayWidth - 1) return;  // Not visible on the map
        PutSymbolAt(x, y, symbolData);
    }

    public void PutSymbolAt(int x, int y, SymbolData symbolData)
    {
        Console.SetCursorPosition(x, ConvertToConsoleY(y));
        SetConsoleColor(symbolData.FgColor, symbolData.BgColor);
        Console.Write(symbolData.Symbol);
    }
    
    private void UpdateConsoleCenterCoords()
    {
        _screenCenterX = (Console.WindowWidth - OverlayWidth) / 2;
        _screenCenterY = Console.WindowHeight / 2;
    }
    
    public static void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor)
            Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor)
            Console.BackgroundColor = bgColor;
    }

    public (int x, int y) ConvertGlobalToScreenCoords(int x, int y)
    {
        (int x, int y) relCoords = Utility.ConvertAbsoluteToRelativeCoords(x, y, FocusPoint.X, FocusPoint.Y);
        return Utility.ConvertRelativeToAbsoluteCoords(relCoords.x, relCoords.y, _screenCenterX, _screenCenterY);
    }

    public (int x, int y) ConvertScreenToGlobalCoords(int x, int y)
    {
        var relCoords = Utility.ConvertAbsoluteToRelativeCoords(x, y, _screenCenterX, _screenCenterY);
        return Utility.ConvertRelativeToAbsoluteCoords(relCoords.x, relCoords.y, FocusPoint.X, FocusPoint.Y);
    }

    private int ConvertToConsoleY(int y)
    {
        return Console.WindowHeight - 1 - y;
    }
}