using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

internal record struct ScreenCellData(char Symbol, ConsoleColor FgColor, ConsoleColor BgColor);

internal record struct ViewInfo(View View, (double, double, double, double) ScreenPart);

public class Renderer
{
    private const ConsoleColor DefaultBackgroundColor = DataHolder.Colors.DefaultBackgroundColor;

    private OrderedSet<ViewPositioner> _viewPositioners;
    private LinkedList<Func<ViewCellData>>[,] _pixelFuncs;
    private Dictionary<View, (int, int, int, int)> _viewBoundaries;
    private LinkedList<(View view, int x, int y, ViewCellData cellData, int delay)> _animQueue;

    private Dictionary<object, ViewPositioner> _objectViewPositioners = new();

    public void SetView(object obj, ViewPositioner viewPositioner)
    {
        if (_objectViewPositioners.ContainsKey(obj))
        {
            ViewPositioner oldView = _objectViewPositioners[obj];
            _viewPositioners.Remove(oldView);
        }
        
        _objectViewPositioners[obj] = viewPositioner;
        AddViewPositioner(viewPositioner);
    }

    private void AddViewPositioner(ViewPositioner viewPositioner)
    {
        _viewPositioners.Add(viewPositioner);
        RecalcAndRedraw(Console.WindowWidth, Console.WindowHeight);
        foreach (var (view, _, _, _, _) in viewPositioner.ViewPositions)
        {
            view.CellChanged += AddCellChange;
            view.CellsChanged += AddAnimation;
            view.NeedRedraw += NeedRedrawHandler;
        }
    }
    
    public void RemoveViewPositioner(object obj)
    {
        if (!_objectViewPositioners.ContainsKey(obj)) return;
        ViewPositioner viewPositioner = _objectViewPositioners[obj];
        foreach (var (view, _, _, _, _) in viewPositioner.ViewPositions)
        {
            view.CellChanged -= AddCellChange;
            view.CellsChanged -= AddAnimation;
            view.NeedRedraw -= NeedRedrawHandler;
        }
        _viewPositioners.Remove(_objectViewPositioners[obj]);
        _objectViewPositioners.Remove(obj);
        RecalcAndRedraw(Console.WindowWidth, Console.WindowHeight);
    }

    public Renderer()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        _viewPositioners = new OrderedSet<ViewPositioner>();
        _viewBoundaries = new Dictionary<View, (int, int, int, int)>();
        _animQueue = new LinkedList<(View, int, int, ViewCellData, int)>();
    }

    public void Redraw()
    {
        if (RedrawOnScreenSizeChange()) return;
        _animQueue.Clear();
        OutputAllCells();
    }

    public void PlayAnimations()
    {
        foreach (var (view, viewX, viewY, viewCellData, delay) in _animQueue)
        {
            if (RedrawOnScreenSizeChange())
            {
                Redraw();
                return;
            }

            var (viewBottomLeftScreenX, viewBottomLeftScreenY, _, _) = _viewBoundaries[view];
            var (screenX, screenY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(viewX, viewY,
                viewBottomLeftScreenX, viewBottomLeftScreenY);
            ScreenCellData screenCellData = GetCellAnimation(screenX, screenY, viewCellData, view);
            Console.SetCursorPosition(screenX, ConvertToConsoleY(screenY));
            SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
            Console.Write(screenCellData.Symbol);
            if (delay != 0) Thread.Sleep(delay);
        }

        _animQueue.Clear();
    }

    public void CleanUp()
    {
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, 0);
    }

    private bool RedrawOnScreenSizeChange()
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        if (_pixelFuncs is null
            || !(windowWidth == _pixelFuncs.GetLength(0) && windowHeight == _pixelFuncs.GetLength(1)))
        {
            RecalcAndRedraw(windowWidth, windowHeight);
            return true;
        }

        return false;
    }

    private void RecalcAndRedraw(int windowWidth, int windowHeight)
    {
        _animQueue.Clear();
        InitPixelFuncArr(windowWidth, windowHeight);
        _viewBoundaries.Clear();
        foreach (ViewPositioner viewPositioner in _viewPositioners)
        {
            viewPositioner.SetScreenSize(windowWidth, windowHeight);
            foreach (var (view, minX, minY, maxX, maxY) in viewPositioner.ViewPositions)
            {
                _viewBoundaries.Add(view, (minX, minY, maxX, maxY));
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        int saveX = x, saveY = y; // x and y are modified outside this closure, so they need to be saved
                        _pixelFuncs[x, y].AddFirst(() => view.GetSymbol(saveX - minX, saveY - minY));
                    }
                }
            }
        }

        OutputAllCells();
    }

    private void OutputAllCells()
    {
        Console.SetCursorPosition(0, 0);
        // do first step outside the loop to avoid working with nulls
        ScreenCellData screenCellData = GetCell(0, ConvertToConsoleY(0));
        StringBuilder currentSequence = new StringBuilder($"{screenCellData.Symbol}");
        ConsoleColor curFgColor = screenCellData.FgColor, curBgColor = screenCellData.BgColor;
        int x = 1;
        // We want Y coordinate increasing by 1 to mean an object going up.
        // However, in the console it means an object going down. This is why we go backwards here.  
        for (int consoleY = _pixelFuncs.GetLength(1) - 1; consoleY >= 0; consoleY--)
        {
            for (; x < _pixelFuncs.GetLength(0); x++)
            {
                screenCellData = GetCell(x, consoleY);
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

    private void InitPixelFuncArr(int windowWidth, int windowHeight)
    {
        _pixelFuncs = new LinkedList<Func<ViewCellData>>[windowWidth, windowHeight];
        for (int x = 0; x < _pixelFuncs.GetLength(0); x++)
        {
            for (int y = 0; y < _pixelFuncs.GetLength(1); y++)
            {
                _pixelFuncs[x, y] = new LinkedList<Func<ViewCellData>>();
            }
        }
    }

    private ScreenCellData GetCellAnimation(int x, int y, ViewCellData cellData, View view)
    {
        (char symbol, ConsoleColor color)? symbolData = null;
        ConsoleColor? backgroundColor = null;
        foreach (var func in _pixelFuncs[x, y])
        {
            ViewCellData viewCellData = ReferenceEquals(func.Target, view) ? cellData : func();
            symbolData ??= viewCellData.SymbolData;
            if ((backgroundColor = viewCellData.BackgroundColor) is not null)
            {
                break;
            }
        }

        symbolData ??= (' ', ConsoleColor.Black);
        backgroundColor ??= ConsoleColor.Black;
        return new ScreenCellData(symbolData.Value.symbol, symbolData.Value.color, backgroundColor.Value);
    }

    private ScreenCellData GetCell(int x, int y)
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

        symbolData ??= (' ', DefaultBackgroundColor);
        backgroundColor ??= DefaultBackgroundColor;
        return new ScreenCellData(symbolData.Value.symbol, symbolData.Value.color, backgroundColor.Value);
    }

    private void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor) Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor) Console.BackgroundColor = bgColor;
    }

    private int ConvertToConsoleY(int y)
    {
        return _pixelFuncs.GetLength(1) - 1 - y;
    }

    private void AddCellChange(object sender, CellChangedEventArgs e)
    {
        _animQueue.AddLast(((View) sender, e.CellInfo.x, e.CellInfo.y, e.CellInfo.cellData, e.CellInfo.delay));
    }

    private void AddAnimation(object sender, CellsChangedEventArgs e)
    {
        foreach (var cellInfo in e.Cells)
        {
            _animQueue.AddLast(((View) sender, cellInfo.x, cellInfo.y, cellInfo.cellData, cellInfo.delay));
        }
    }
    
    private void NeedRedrawHandler(object sender, EventArgs _)
    {
        Redraw(); // The renderer is fast enough
    }
}