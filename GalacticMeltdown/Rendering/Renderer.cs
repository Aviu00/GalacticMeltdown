using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticMeltdown.Rendering;

public delegate void ViewChangedEventHandler(View sender);

public class Renderer
{
    private LinkedList<(View, double, double, double, double)> _views;  // View, top-left and bottom-right corner coords (rel)
    private LinkedList<Func<ViewCellData>>[,] _pixelFuncs;
    private ScreenCellData[,] _screenCells;

    public Renderer()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        _views = new LinkedList<(View, double, double, double, double)>();
    }

    private void RecalculatePixelArrays(int windowWidth, int windowHeight)
    {
        FillPixelArrays(windowWidth, windowHeight);
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
                    int saveX = x, saveY = y;  // x and y are modified in the loop above, so they need to be saved
                    _pixelFuncs[x, y].AddFirst(() => view.GetSymbol(saveX - x0Screen, saveY - y0Screen));
                }
            }
        }
    }

    private void FillPixelArrays(int windowWidth, int windowHeight)
    {
        _screenCells = new ScreenCellData[windowWidth, windowHeight];
        _pixelFuncs = new LinkedList<Func<ViewCellData>>[windowWidth, windowHeight];
        for (int x = 0; x < _pixelFuncs.GetLength(0); x++)
        {
            for (int y = 0; y < _pixelFuncs.GetLength(1); y++)
            {
                _pixelFuncs[x, y] = new LinkedList<Func<ViewCellData>>();
            }
        }
    }

    private void UpdateCurrentCells()
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        if (_screenCells is null || 
            !(windowWidth == _screenCells.GetLength(0) && windowHeight == _screenCells.GetLength(1))) 
            RecalculatePixelArrays(windowWidth, windowHeight);
        for (int x = 0; x < _pixelFuncs.GetLength(0); x++)
        {
            for (int y = 0; y < _pixelFuncs.GetLength(1); y++)
            {
                (char symbol, ConsoleColor color)? symbolData = null;
                ConsoleColor? backgroundColor = null;
                foreach (var func in _pixelFuncs[x, y])
                {
                    ViewCellData viewCellData = func();
                    symbolData ??= viewCellData.SymbolData;
                    if ((backgroundColor ??= viewCellData.BackgroundColor) is not null)
                    {
                        break;
                    }
                }

                symbolData ??= (' ', ConsoleColor.Black);
                _screenCells[x, y] = new ScreenCellData(symbolData.Value.symbol, symbolData.Value.color,
                    backgroundColor ?? ConsoleColor.Black);
            }
        }
    }

    private void OutputCells()
    {
        Console.SetCursorPosition(0, 0);
        ScreenCellData screenCellData = _screenCells[0, 0];
        StringBuilder currentSequence = new StringBuilder($"{screenCellData.Symbol}");
        ConsoleColor curFgColor = screenCellData.FgColor, curBgColor = screenCellData.BgColor;
        int x = 1;
        for (int areaY = _screenCells.GetLength(1) - 1; areaY >= 0; areaY--)
        {
            for (; x < _screenCells.GetLength(0); x++)
            {
                screenCellData = _screenCells[x, areaY];
                if (screenCellData.FgColor != curFgColor && screenCellData.Symbol != ' ' 
                    || screenCellData.BgColor != curBgColor)
                {
                    SetConsoleColor(curFgColor, curBgColor);
                    Console.Write(currentSequence);
                    currentSequence.Clear();
                    curFgColor = screenCellData.FgColor;
                    curBgColor = screenCellData.BgColor;
                }

                currentSequence.Append(screenCellData.Symbol);
            }

            x = 0;
        }
        SetConsoleColor(curFgColor, curBgColor);
        Console.Write(currentSequence);
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
        UpdateCurrentCells();
        OutputCells();
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

    public void PutSymbolAt(int x, int y, ScreenCellData screenCellData)
    {
        Console.SetCursorPosition(x, ConvertToConsoleY(y));
        SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
        Console.Write(screenCellData.Symbol);
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