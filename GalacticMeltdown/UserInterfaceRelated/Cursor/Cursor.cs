using System;
using System.Collections.Generic;
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
    public Controller Controller { get; }
    
    private Func<(int, int)> _getStartCoords;
    
    private Func<(int, int, int, int)> _getViewBounds;
    private (int minX, int minY, int maxX, int maxY)? _levelBounds;

    private LevelView _levelView;

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
    
    public bool InFocus { get; set; }
    
    public Action<int, int, int, int> Action { private get; set; }
    
    public event EventHandler<MoveEventArgs> Moved;

    public Cursor(int x, int y, Func<(int, int)> getStartCoords, Func<(int, int, int, int)> getViewBounds,
        LevelView levelView)
    {
        X = x;
        Y = y;
        _levelView = levelView;
        _getStartCoords = getStartCoords;
        _getViewBounds = getViewBounds;
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Cursor,
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

    public void Interact()
    {
        var (x0, y0) = _getStartCoords();
        Action?.Invoke(x0, y0, X, Y);
    }
    
    public void MoveInbounds()
    {
        if (IsPositionInbounds(X, Y)) return;
        var (minX, minY, maxX, maxY) = GetBounds();
        MoveTo(Math.Min(Math.Max(X, minX), maxX), Math.Min(Math.Max(Y, minY), maxY));
    }

    private void MoveTo(int x, int y)
    {
        int oldX = X, oldY = Y;
        X = x;
        Y = y;
        Moved?.Invoke(this, new MoveEventArgs(oldX, oldY, X, Y));
    }

    private bool IsPositionInbounds(int x, int y)
    {
        var (minX, minY, maxX, maxY) = GetBounds();
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    private (int minX, int minY, int maxX, int maxY) GetBounds()
    {
        if (LevelBounds is null) return _getViewBounds();

        var (minXView, minYView, maxXView, maxYView) = _getViewBounds();
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