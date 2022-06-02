using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Cursor;

public class Cursor : IControllable
{
    private Controller Controller { get; }

    private (int minX, int minY, int maxX, int maxY)? _levelBounds;

    private LevelView _levelView;

    private int _initialX;
    private int _initialY;

    public HashSet<(int x, int y)> LinePoints { get; private set; }

    public (int minX, int minY, int maxX, int maxY)? LevelBounds
    {
        get => _levelBounds;
        set
        {
            _levelBounds = value;
            MoveInbounds();
        }
    }

    public ConsoleColor Color => DataHolder.Colors.CursorColor;
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public Action<int, int> Action { private get; set; }
    
    public event EventHandler<MoveEventArgs> Moved;

    public Cursor(int initialX, int initialY, LevelView levelView)
    {
        _initialX = initialX;
        _initialY = initialY;
        X = _initialX;
        Y = _initialY;
        _levelView = levelView;
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(KeyBindings.Cursor,
            new Dictionary<CursorControl, Action>
            {
                {CursorControl.MoveUp, () => TryMove(0, 1)},
                {CursorControl.MoveDown, () => TryMove(0, -1)},
                {CursorControl.MoveRight, () => TryMove(1, 0)},
                {CursorControl.MoveLeft, () => TryMove(-1, 0)},
                {CursorControl.MoveNe, () => TryMove(1, 1)},
                {CursorControl.MoveSe, () => TryMove(1, -1)},
                {CursorControl.MoveSw, () => TryMove(-1, -1)},
                {CursorControl.MoveNw, () => TryMove(-1, 1)},
                {CursorControl.Interact, Interact},
                {CursorControl.Back, Close},
                {CursorControl.ToggleLine, _levelView.ToggleCursorLine},
                {CursorControl.ToggleFocus, _levelView.ToggleCursorFocus}
            }));
    }
    
    public bool TryMove(int deltaX, int deltaY)
    {
        int newX = X + deltaX, newY = Y + deltaY;
        if (!IsPositionInbounds(newX, newY)) return false;
        MoveTo(newX, newY);
        return true;
    }

    private void Interact()
    {
        Action?.Invoke(X, Y);
    }
    
    public void MoveInbounds()
    {
        if (IsPositionInbounds(X, Y)) return;
        var (minX, minY, maxX, maxY) = GetBounds();
        MoveTo(Math.Min(Math.Max(X, minX), maxX), Math.Min(Math.Max(Y, minY), maxY));
    }

    public void ToggleLine()
    {
        LinePoints = LinePoints is null
            ? Algorithms.BresenhamGetPointsOnLine(_initialX, _initialY, X, Y).ToHashSet()
            : null;
    }

    private void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        if (LinePoints is not null)
            LinePoints = Algorithms.BresenhamGetPointsOnLine(_initialX, _initialY, X, Y).ToHashSet();
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY));
    }

    private bool IsPositionInbounds(int x, int y)
    {
        var (minX, minY, maxX, maxY) = GetBounds();
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    private (int minX, int minY, int maxX, int maxY) GetBounds()
    {
        if (LevelBounds is null) return _levelView.ViewBounds;

        var (minXView, minYView, maxXView, maxYView) = _levelView.ViewBounds;
        var (minXWorld, minYWorld, maxXWorld, maxYWorld) = LevelBounds.Value;
        return (Math.Max(minXView, minXWorld), Math.Max(minYView, minYWorld), Math.Min(maxXView, maxXWorld),
            Math.Min(maxYView, maxYWorld));
    }

    public void Start()
    {
        UserInterface.SetController(this, Controller);
        UserInterface.TakeControl(this);
    }

    public void Close()
    {
        UserInterface.Forget(this);
        _levelView.RemoveCursor();
    }
}