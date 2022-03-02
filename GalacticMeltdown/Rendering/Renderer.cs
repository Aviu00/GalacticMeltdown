using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticMeltdown.Rendering;

public delegate void ViewChangedEventHandler(View sender);

public delegate void CellsChangedEventHandler(View sender, HashSet<(int, int, ViewCellData)> data);

public static class Renderer
{
    private static LinkedList<(View, double, double, double, double)> _views;  // View, top-left and bottom-right corner coords (rel)
    private static LinkedList<Func<ViewCellData>>[,] _pixelFuncs;
    private static ScreenCellData[,] _screenCells;
    private static Dictionary<View, (int, int, int, int)> _viewBoundaries;

    static Renderer()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        _views = new LinkedList<(View, double, double, double, double)>();
        _viewBoundaries = new Dictionary<View, (int, int, int, int)>();
    }

    private static void RecalculatePixelArrays(int windowWidth, int windowHeight)
    {
        FillPixelArrays(windowWidth, windowHeight);
        foreach (var (view, x0Portion, y0Portion, x1Portion, y1Portion) in _views)
        {
            int x0Screen = (int) Math.Round(windowWidth * x0Portion);
            int y0Screen = (int) Math.Round(windowHeight * y0Portion);
            int x1Screen = (int) Math.Round(windowWidth * x1Portion);
            int y1Screen = (int) Math.Round(windowHeight * y1Portion);
            view.Resize(x1Screen - x0Screen, y1Screen - y0Screen);
            _viewBoundaries.Add(view, (x0Screen, y0Screen, x1Screen, y1Screen));
            for (int x = x0Screen; x < x1Screen; x++)
            {
                for (int y = y0Screen; y < y1Screen; y++)
                {
                    int saveX = x, saveY = y;  // x and y are modified outside this closure, so they need to be saved
                    _pixelFuncs[x, y].AddFirst(() => view.GetSymbol(saveX - x0Screen, saveY - y0Screen));
                }
            }
        }
    }

    private static void FillPixelArrays(int windowWidth, int windowHeight)
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

    private static void UpdateCurrentCells()
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
                    if ((backgroundColor = viewCellData.BackgroundColor) is not null)
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

    private static void OutputAllCells()
    {
        Console.SetCursorPosition(0, 0);
        // do first step outside the loop to avoid working with nulls
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

    public static void AddView(View view, double x0Portion, double y0Portion, double x1Portion, double y1Portion)
    {
        view.NeedRedraw += NeedRedrawHandler;
        _views.AddFirst((view, x0Portion, y0Portion, x1Portion, y1Portion));
    }

    private static void NeedRedrawHandler(View sender)
    {
        Redraw();  // The renderer is fast enough
    }

    public static void RemoveLastView()
    {
        if (_views.Any())
        {
            _views.RemoveFirst();
        }
    }

    public static void ClearViews()
    {
        _views.Clear();
    }

    public static void Redraw()
    {
        UpdateCurrentCells();
        OutputAllCells();
    }
    
    public static void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor)
            Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor)
            Console.BackgroundColor = bgColor;
    }

    private static int ConvertToConsoleY(int y)
    {
        return _screenCells.GetLength(1) - 1 - y;
    }

    public static void CleanUp()
    {
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, 0);
    }
}