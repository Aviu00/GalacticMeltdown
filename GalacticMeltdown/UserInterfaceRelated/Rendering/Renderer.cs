using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GalacticMeltdown.Collections;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

internal record struct ScreenCellData(char Symbol, ConsoleColor FgColor, ConsoleColor BgColor);

public class Renderer
{
    private const ConsoleColor DefaultBackgroundColor = DataHolder.Colors.DefaultBackgroundColor;

    private OrderedSet<ViewPositioner> _viewPositioners;
    private LinkedList<(View view, int viewX, int viewY)>[,] _pixelInfos;
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
        viewPositioner.SetScreenSize(_pixelInfos.GetLength(0), _pixelInfos.GetLength(1));
        foreach (var (view, _, _, _, _) in viewPositioner.ViewPositions)
        {
            view.CellChanged += AddCellChange;
            view.CellsChanged += AddAnimation;
            view.NeedRedraw += NeedRedrawHandler;
        }
        RecalcAndRedraw(Console.WindowWidth, Console.WindowHeight);
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
        InitPixelFuncArr(Console.WindowWidth, Console.WindowHeight);
    }

    public void PlayAnimations()
    {
        foreach (var (view, viewX, viewY, viewCellData, delay) in _animQueue)
        {
            if (RedrawOnScreenSizeChange()) return;

            var (viewMinScreenX, viewMinScreenY, _, _) = _viewBoundaries[view];
            var (screenX, screenY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(viewX, viewY,
                viewMinScreenX, viewMinScreenY);
            ScreenCellData screenCellData = GetCellAnimation(screenX, screenY, viewCellData, view);
            Console.SetCursorPosition(screenX, ConvertToConsoleY(screenY));
            SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
            Console.Write(screenCellData.Symbol);
            if (delay != 0) Thread.Sleep(delay);
        }

        OutputAllCells();
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
        if (!(windowWidth == _pixelInfos.GetLength(0) && windowHeight == _pixelInfos.GetLength(1)))
        {
            RecalcAndRedraw(windowWidth, windowHeight);
            return true;
        }

        return false;
    }

    private void RecalcAndRedraw(int windowWidth, int windowHeight)
    {
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
                        _pixelInfos[x, y].AddFirst((view, x - minX, y - minY));
                    }
                }
            }
        }

        OutputAllCells();
    }

    private void OutputAllCells()
    {
        _animQueue.Clear();
        Console.SetCursorPosition(0, 0);
        // do first step outside the loop to avoid working with nulls
        ScreenCellData screenCellData = GetCell(0, ConvertToConsoleY(0));
        StringBuilder currentSequence = new($"{screenCellData.Symbol}");
        ConsoleColor curFgColor = screenCellData.FgColor, curBgColor = screenCellData.BgColor;
        int x = 1;
        // We want Y coordinate increasing by 1 to mean an object going up.
        // However, in the console it means an object going down. This is why we go backwards here.  
        for (int y = ConvertToConsoleY(0); y >= 0; y--)
        {
            for (; x < _pixelInfos.GetLength(0); x++)
            {
                screenCellData = GetCell(x, y);
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
        _pixelInfos = new LinkedList<(View view, int viewX, int viewY)>[windowWidth, windowHeight];
        for (int x = 0; x < _pixelInfos.GetLength(0); x++)
        {
            for (int y = 0; y < _pixelInfos.GetLength(1); y++)
            {
                _pixelInfos[x, y] = new LinkedList<(View view, int viewX, int viewY)>();
            }
        }
    }

    private ScreenCellData GetCellAnimation(int x, int y, ViewCellData cellData, View animView)
    {
        (char symbol, ConsoleColor color)? symbolData = null;
        ConsoleColor? backgroundColor = null;
        foreach (var (view, viewX, viewY) in _pixelInfos[x, y])
        {
            ViewCellData viewCellData = ReferenceEquals(view, animView) ? cellData : view.GetSymbol(viewX, viewY);
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
        foreach (var (view, viewX, viewY) in _pixelInfos[x, y])
        {
            ViewCellData viewCellData = view.GetSymbol(viewX, viewY);
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
        return _pixelInfos.GetLength(1) - 1 - y;
    }

    private void AddCellChange(object sender, CellChangedEventArgs e)
    {
        _animQueue.AddLast(((View) sender, e.CellInfo.x, e.CellInfo.y, e.CellInfo.cellData, e.CellInfo.delay));
    }

    private void AddAnimation(object sender, CellsChangedEventArgs e)
    {
        foreach (var cellInfo in e.Cells)
        {
            _animQueue.AddLast(((View) sender, cellInfo.x, cellInfo.y, cellInfo.cellData, 0));
        }

        if (e.Cells.Any())
        {
            _animQueue.Last!.Value = ((View) sender, _animQueue.Last.Value.x, _animQueue.Last.Value.y,
                _animQueue.Last.Value.cellData, e.Delay);
        }
    }
    
    private void NeedRedrawHandler(object sender, EventArgs _)
    {
        if (RedrawOnScreenSizeChange()) return;
        
        _animQueue =
            new LinkedList<(View view, int x, int y, ViewCellData cellData, int delay)>(
                _animQueue.Where(info => info.view != sender));

        var (minX, minY, maxX, maxY) = _viewBoundaries[(View) sender];
        maxY -= 1;
        Console.SetCursorPosition(minX, ConvertToConsoleY(maxY));
        ScreenCellData screenCellData = GetCell(minX, ConvertToConsoleY(maxY));
        StringBuilder currentSequence = new();
        ConsoleColor curFgColor = screenCellData.FgColor, curBgColor = screenCellData.BgColor;
        for (int y = maxY; y >= minY; y--)
        {
            if (minX != 0) Console.SetCursorPosition(minX, ConvertToConsoleY(y));

            for (int x = minX; x < maxX; x++)
            {
                screenCellData = GetCell(x, y);
                if (screenCellData.FgColor != curFgColor && screenCellData.Symbol != ' '
                    || screenCellData.BgColor != curBgColor)
                {
                    WriteSequenceOut();
                    curFgColor = screenCellData.FgColor;
                    curBgColor = screenCellData.BgColor;
                }

                currentSequence.Append(screenCellData.Symbol);
            }

            if (minX == 0)
            {
                if (maxX != _pixelInfos.GetLength(0)) currentSequence.Append('\n');
            }
            else
            {
                WriteSequenceOut();
            }
        }

        if (minX == 0)
        {
            if (maxX != _pixelInfos.GetLength(0)) currentSequence.Length--;
            WriteSequenceOut();
        }

        void WriteSequenceOut()
        {
            SetConsoleColor(curFgColor, curBgColor);
            Console.Write(currentSequence);
            currentSequence.Clear();
        }
    }
}