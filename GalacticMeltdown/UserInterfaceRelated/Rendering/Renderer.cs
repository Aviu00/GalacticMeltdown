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

public class Renderer
{
    private const ConsoleColor DefaultColor = Colors.DefaultMain;

    private struct ScreenCellData
    {
        public char Symbol;
        public ConsoleColor FgColor;
        public ConsoleColor BgColor;

        public ScreenCellData()
        {
            Symbol = ' ';
            FgColor = DefaultColor;
            BgColor = DefaultColor;
        }
        
        public ScreenCellData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
        {
            Symbol = symbol;
            FgColor = fgColor;
            BgColor = bgColor;
        }
    }

    private OrderedSet<ViewPositioner> _viewPositioners;
    private LinkedList<View> _views;
    private LinkedList<(View view, int viewX, int viewY)>[,] _cellInfos;
    private Dictionary<View, (int minX, int minY)> _viewCornerCoords;
    private LinkedList<(View view, int viewX, int viewY, ViewCellData cellData, int delay)> _animQueue;

    private int _maxAnimationTime = 100;

    private Dictionary<object, ViewPositioner> _objectViewPositioners;

    public int MaxAnimationTime
    {
        get => _maxAnimationTime;
        set
        {
            if (value >= 0) _maxAnimationTime = value;
        }
    }

    public void SetView(object obj, ViewPositioner viewPositioner)
    {
        if (_objectViewPositioners.ContainsKey(obj))
        {
            ViewPositioner oldPositioner = _objectViewPositioners[obj];
            _viewPositioners.Remove(oldPositioner);
        }
        
        _objectViewPositioners[obj] = viewPositioner;
        AddViewPositioner(viewPositioner);
    }

    private void AddViewPositioner(ViewPositioner viewPositioner)
    {
        if (!RedrawOnScreenSizeChange()) PlayAnimations();
        _viewPositioners.Add(viewPositioner);
        viewPositioner.SetScreenSize(_cellInfos.GetLength(0), _cellInfos.GetLength(1));
        foreach (var (view, _, _, _, _) in viewPositioner.ViewPositions)
        {
            if (view is IOneCellAnim oca) oca.OneCellAnim += AddOneCellAnim;
            if (view is IMultiCellAnim mca) mca.MultiCellAnim += AddMultiCellAnim;
            if (view is IOneCellUpdate ocu) ocu.OneCellUpdate += UpdateCell;
            if (view is IMultiCellUpdate mcu) mcu.MultiCellUpdate += UpdateCells;
            view.NeedRedraw += NeedRedrawHandler;
        }
        RecalcAndRedraw(Console.WindowWidth, Console.WindowHeight);
    }

    public void RemoveViewPositioner(object obj)
    {
        if (!_objectViewPositioners.ContainsKey(obj)) return;
        if (!RedrawOnScreenSizeChange()) PlayAnimations();
        ViewPositioner viewPositioner = _objectViewPositioners[obj];
        foreach (var (view, _, _, _, _) in viewPositioner.ViewPositions)
        {
            if (view is IOneCellAnim oca) oca.OneCellAnim -= AddOneCellAnim;
            if (view is IMultiCellAnim mca) mca.MultiCellAnim -= AddMultiCellAnim;
            if (view is IOneCellUpdate ocu) ocu.OneCellUpdate -= UpdateCell;
            if (view is IMultiCellUpdate mcu) mcu.MultiCellUpdate -= UpdateCells;
            view.NeedRedraw -= NeedRedrawHandler;
        }
        _viewPositioners.Remove(_objectViewPositioners[obj]);
        _objectViewPositioners.Remove(obj);
        RecalcAndRedraw(Console.WindowWidth, Console.WindowHeight);
    }

    public Renderer()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = DefaultColor;
        Console.Clear();
        _viewPositioners = new OrderedSet<ViewPositioner>();
        _viewCornerCoords = new Dictionary<View, (int, int)>();
        _animQueue = new LinkedList<(View, int, int, ViewCellData, int)>();
        _views = new LinkedList<View>();
        _objectViewPositioners = new Dictionary<object, ViewPositioner>();
        InitPixelFuncArr(Console.WindowWidth, Console.WindowHeight);
    }

    public void PlayAnimations()
    {
        if (!_animQueue.Any()) return;
        int totalDelay = _animQueue.Sum(data => data.delay);
        double factor = totalDelay <= MaxAnimationTime ? 1 : MaxAnimationTime / (double) totalDelay;
        foreach (var (view, viewX, viewY, viewCellData, delay) in _animQueue)
        {
            if (RedrawOnScreenSizeChange()) return;

            var (viewMinScreenX, viewMinScreenY) = _viewCornerCoords[view];
            var (screenX, screenY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(viewX, viewY,
                viewMinScreenX, viewMinScreenY);
            ScreenCellData screenCellData = GetCellWithSwap(screenX, screenY, viewCellData, view);
            Console.SetCursorPosition(screenX, ConvertToConsoleY(screenY));
            SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
            Console.Write(screenCellData.Symbol);
            int actualDelay = (int) (delay * factor);
            if (actualDelay != 0) Thread.Sleep(actualDelay);
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

    public bool RedrawOnScreenSizeChange()
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        if (!(windowWidth == _cellInfos.GetLength(0) && windowHeight == _cellInfos.GetLength(1)))
        {
            RecalcAndRedraw(windowWidth, windowHeight);
            return true;
        }

        return false;
    }

    private void RecalcAndRedraw(int windowWidth, int windowHeight)
    {
        InitPixelFuncArr(windowWidth, windowHeight);
        _viewCornerCoords.Clear();
        _views.Clear();
        foreach (ViewPositioner viewPositioner in _viewPositioners)
        {
            viewPositioner.SetScreenSize(windowWidth, windowHeight);
            foreach (var (view, minX, minY, maxX, maxY) in viewPositioner.ViewPositions)
            {
                _views.AddLast(view);
                _viewCornerCoords.Add(view, (minX, minY));
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        _cellInfos[x, y].AddFirst((view, x - minX, y - minY));
                    }
                }
            }
        }

        OutputAllCells();
    }

    private void OutputAllCells()
    {
        _animQueue.Clear();
        var screenCells = new ScreenCellData[_cellInfos.GetLength(0), _cellInfos.GetLength(1)];
        screenCells.Initialize();
        foreach (View view in _views)
        {
            ViewCellData[,] viewCells = view.GetAllCells();
            var (minViewX, minViewY) = _viewCornerCoords[view];
            for (int viewY = 0; viewY < Math.Min(viewCells.GetLength(1), screenCells.GetLength(1) - minViewY); viewY++)
            {
                for (int viewX = 0;
                     viewX < Math.Min(viewCells.GetLength(0), screenCells.GetLength(0) - minViewX);
                     viewX++)
                {
                    int screenX = viewX + minViewX, screenY = viewY + minViewY;
                    if (viewCells[viewX, viewY].BackgroundColor is not null)
                    {
                        screenCells[screenX, screenY].BgColor = viewCells[viewX, viewY].BackgroundColor.Value;
                        screenCells[screenX, screenY].Symbol =  ' ';
                        screenCells[screenX, screenY].FgColor = DefaultColor;
                    }
                    if (viewCells[viewX, viewY].SymbolData is not null)
                    {
                        (char symbol, ConsoleColor textColor) = viewCells[viewX, viewY].SymbolData.Value;
                        screenCells[screenX, screenY].Symbol = symbol;
                        screenCells[screenX, screenY].FgColor = textColor;
                    }
                }
            }
        }
        ScreenCellData firstCell = screenCells[0, ConvertToConsoleY(0)];
        DrawArea(RowIter(), (firstCell.FgColor, firstCell.BgColor), 0, 0, screenCells.GetLength(0));

        IEnumerable<IEnumerable<ScreenCellData>> RowIter()
        {
            for (int y = screenCells.GetLength(1) - 1; y >= 0; y--)
            {
                yield return CellIter(y);
            }

            IEnumerable<ScreenCellData> CellIter(int y)
            {
                for (int x = 0; x < screenCells.GetLength(0); x++)
                {
                    yield return screenCells[x, y];
                }
            }
        }
    }

    private void InitPixelFuncArr(int windowWidth, int windowHeight)
    {
        _cellInfos = new LinkedList<(View view, int viewX, int viewY)>[windowWidth, windowHeight];
        for (int x = 0; x < _cellInfos.GetLength(0); x++)
        {
            for (int y = 0; y < _cellInfos.GetLength(1); y++)
            {
                _cellInfos[x, y] = new LinkedList<(View view, int viewX, int viewY)>();
            }
        }
    }

    private ScreenCellData GetCellWithSwap(int x, int y, ViewCellData cellData, View swapView)
    {
        (char symbol, ConsoleColor color)? symbolData = null;
        ConsoleColor? backgroundColor = null;
        foreach (var (view, viewX, viewY) in _cellInfos[x, y])
        {
            ViewCellData viewCellData = ReferenceEquals(view, swapView) ? cellData : view.GetSymbol(viewX, viewY);
            symbolData ??= viewCellData.SymbolData;
            if ((backgroundColor = viewCellData.BackgroundColor) is not null)
            {
                break;
            }
        }

        symbolData ??= (' ', DefaultColor);
        backgroundColor ??= DefaultColor;
        return new ScreenCellData(symbolData.Value.symbol, symbolData.Value.color, backgroundColor.Value);
    }

    private void SetConsoleColor(ConsoleColor fgColor, ConsoleColor bgColor)
    {
        if (Console.ForegroundColor != fgColor) Console.ForegroundColor = fgColor;
        if (Console.BackgroundColor != bgColor) Console.BackgroundColor = bgColor;
    }

    private int ConvertToConsoleY(int y)
    {
        return _cellInfos.GetLength(1) - 1 - y;
    }

    private void AddOneCellAnim(object sender, OneCellAnimEventArgs e)
    {
        _animQueue.AddLast(((View) sender, e.CellInfo.x, e.CellInfo.y, e.CellInfo.cellData, e.CellInfo.delay));
    }

    private void AddMultiCellAnim(object sender, MultiCellAnimEventArgs e)
    {
        foreach (var cellInfo in e.Cells)
        {
            _animQueue.AddLast(((View) sender, cellInfo.x, cellInfo.y, cellInfo.cellData, 0));
        }

        if (_animQueue.Last is not null)
        {
            _animQueue.Last.Value = ((View) sender, _animQueue.Last.Value.viewX, _animQueue.Last.Value.viewY,
                _animQueue.Last.Value.cellData, e.Delay);
        }
    }

    private void UpdateCell(object sender, OneCellUpdateEventArgs e)
    {
        PlayAnimations();
        var view = (View) sender;
        var (viewMinScreenX, viewMinScreenY) = _viewCornerCoords[view];
        var (screenX, screenY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(e.CellInfo.x, e.CellInfo.y,
            viewMinScreenX, viewMinScreenY);
        ScreenCellData screenCellData = GetCellWithSwap(screenX, screenY, e.CellInfo.cellData, view);
        Console.SetCursorPosition(screenX, ConvertToConsoleY(screenY));
        SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
        Console.Write(screenCellData.Symbol);
    }

    private void UpdateCells(object sender, MultiCellUpdateEventArgs e)
    {
        PlayAnimations();
        var view = (View) sender;
        var (viewMinScreenX, viewMinScreenY) = _viewCornerCoords[view];
        foreach (var (x, y, cellData) in e.Cells)
        {
            var (screenX, screenY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(x, y,
                viewMinScreenX, viewMinScreenY);
            ScreenCellData screenCellData = GetCellWithSwap(screenX, screenY, cellData, view);
            Console.SetCursorPosition(screenX, ConvertToConsoleY(screenY));
            SetConsoleColor(screenCellData.FgColor, screenCellData.BgColor);
            Console.Write(screenCellData.Symbol);
        }
    }

    private void NeedRedrawHandler(object sender, EventArgs _)
    {
        PlayAnimations();
        var view = (View) sender;
        (int minX, int minY) = _viewCornerCoords[view];
        ViewCellData[,] viewCells = view.GetAllCells();
        int maxX = minX + viewCells.GetLength(0), maxY = minY + viewCells.GetLength(1) - 1;
        ScreenCellData screenCellData = GetCellWithSwap(minX, maxY, viewCells[0, maxY - minY], view);
        DrawArea(RowIter(), (screenCellData.FgColor, screenCellData.BgColor), ConvertToConsoleY(maxY), minX, maxX);

        IEnumerable<IEnumerable<ScreenCellData>> RowIter()
        {
            for (int y = maxY; y >= minY; y--)
            {
                yield return CellIter(y);
            }

            IEnumerable<ScreenCellData> CellIter(int y)
            {
                for (int x = minX; x < maxX; x++)
                {
                    yield return GetCellWithSwap(x, y, viewCells[x - minX, y - minY], view);
                }
            }
        }
    }

    private void DrawArea(IEnumerable<IEnumerable<ScreenCellData>> data,
        (ConsoleColor FgColor, ConsoleColor BgColor) firstColor, int y, int minX, int maxX)
    {
        if (minX == 0)
        {
            if (maxX == _cellInfos.GetLength(0))
                DrawWholeLines();
            else
                DrawLeftLines();
        }
        else
        {
            DrawLines();
        }

        void DrawWholeLines()
        {
            Console.SetCursorPosition(0, y);
            StringBuilder currentSequence = new();
            (ConsoleColor curFgColor, ConsoleColor curBgColor) = firstColor;
            foreach (IEnumerable<ScreenCellData> row in data)
            {
                foreach (ScreenCellData screenCellData in row)
                {
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
            }

            SetConsoleColor(curFgColor, curBgColor);
            Console.Write(currentSequence);
        }

        void DrawLeftLines()
        {
            Console.SetCursorPosition(0, y);
            StringBuilder currentSequence = new();
            (ConsoleColor curFgColor, ConsoleColor curBgColor) = firstColor;
            foreach (IEnumerable<ScreenCellData> row in data)
            {
                foreach (ScreenCellData screenCellData in row)
                {
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

                currentSequence.Append('\n');
            }

            currentSequence.Length--;
            SetConsoleColor(curFgColor, curBgColor);
            Console.Write(currentSequence);
        }

        void DrawLines()
        {
            StringBuilder currentSequence = new();
            (ConsoleColor curFgColor, ConsoleColor curBgColor) = firstColor;
            foreach (IEnumerable<ScreenCellData> row in data)
            {
                Console.SetCursorPosition(minX, y++);
                foreach (ScreenCellData screenCellData in row)
                {
                    if (screenCellData.FgColor != curFgColor && screenCellData.Symbol != ' '
                        || screenCellData.BgColor != curBgColor)
                    {
                        WriteSequenceOut();
                        curFgColor = screenCellData.FgColor;
                        curBgColor = screenCellData.BgColor;
                    }

                    currentSequence.Append(screenCellData.Symbol);
                }

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
}