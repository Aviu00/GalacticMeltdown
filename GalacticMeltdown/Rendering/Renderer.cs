using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticMeltdown.Rendering;

public delegate void ViewChangedEventHandler(View sender);

public class Renderer
{
    private LinkedList<(View, double, double, double, double)> _views;  // View, top-left and bottom-right corner coords (rel)
    private LinkedList<Func<ScreenCellData>>[,] _pixelFuncs;
    private ScreenCellData[,] _screenCells;

    public Renderer()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        _views = new LinkedList<(View, double, double, double, double)>();
    }

    private void RecalculatePixelArrays()
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        _screenCells = new ScreenCellData[windowWidth, windowHeight];
        _pixelFuncs = new LinkedList<Func<ScreenCellData>>[windowWidth, windowHeight];
        foreach (var (view, x0Portion, y0Portion, x1Portion, y1Portion) in _views)
        {
            int x0Screen = (int) Math.Round(windowWidth * x0Portion);
            int y0Screen = (int) Math.Round(windowHeight * y0Portion);
            int x1Screen = (int) Math.Round(windowWidth * x1Portion);
            int y1Screen = (int) Math.Round(windowHeight * y1Portion);
            view.Resize(x1Screen - x0Screen, y1Screen - y0Screen);
            for (int x = x0Screen; x < x1Screen; x++)
            {
                for (int y = y0Screen; y < y1Screen; y++)
                {
                    //_pixelFuncs[x, y].AddFirst(() => view.GetSymbol(x - x0Screen, y - y0Screen));
                }
            }
        }
    }

    public void AddView(View view, double x0Portion, double y0Portion, double x1Portion, double y1Portion)
    {
        view.ViewChanged += ViewChangedHandler;
        _views.AddFirst((view, x0Portion, y0Portion, x1Portion, y1Portion));
    }

    private void ViewChangedHandler(View sender)
    {
        Redraw();  // The renderer is fast enough
    }

    public void RemoveLastView()
    {
        if (_views.Any())
        {
            _views.RemoveFirst();
        }
    }

    public void ClearViews()
    {
        _views.Clear();
    }

    public void Redraw()
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        foreach (var (view, x0Portion, y0Portion, x1Portion, y1Portion) in _views)
        {
            int x0Screen = (int) Math.Round(windowWidth * x0Portion);
            int y0Screen = (int) Math.Round(windowHeight * y0Portion);
            int x1Screen = Math.Min((int) Math.Round(windowWidth * x1Portion), windowWidth - 1);
            int y1Screen = Math.Min((int) Math.Round(windowHeight * y1Portion), windowHeight - 1);
            int width = x1Screen - x0Screen, height = y1Screen - y0Screen;
            view.Resize(width, height);
            DrawArea(x0Screen, y0Screen, x1Screen, y1Screen, 
                (x, y) => view.GetSymbol(x - x0Screen, y - y0Screen));
        }
    }

    public void DrawArea
        (int startX, int startY, int maxX, int maxY, GetSymbolAt getSymbolAt)
    {
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

    public void PutSymbolAt(int x, int y, SymbolData symbolData)
    {
        Console.SetCursorPosition(x, ConvertToConsoleY(y));
        SetConsoleColor(symbolData.FgColor, symbolData.BgColor);
        Console.Write(symbolData.Symbol);
    }
    
    public static void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor)
            Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor)
            Console.BackgroundColor = bgColor;
    }

    private int ConvertToConsoleY(int y)
    {
        return Console.WindowHeight - y;
    }

    public static void CleanUp()
    {
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, 0);
    }
}