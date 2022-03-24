using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Views;

public class SeenTilesArray
{
    private const int Offset = 1;
    [JsonProperty] private char?[,] _array;

    public SeenTilesArray(int mapWidth, int mapHeight)
    {
        _array = new char?[mapWidth, mapHeight];
    }

    public char? this[int x, int y]
    {
        get => _array[x + Offset, y + Offset];
        set => _array[x + Offset, y + Offset] = value;
    }

    public bool Inbounds(int x, int y)
    {
        return x + Offset >= 0 && x + Offset < _array.GetLength(0)
            && y + Offset >= 0 && y + Offset < _array.GetLength(1);
    }
}

public partial class LevelView : View
{
    [JsonProperty] private readonly Level _level;

    private IFocusable _focusObject;

    private ObservableCollection<ISightedObject> _sightedObjects;
    private HashSet<(int, int)> _visiblePoints;
    [JsonProperty] private SeenTilesArray _seenCells;

    private (int minX, int minY, int maxX, int maxY) ViewBounds => 
        (_focusObject.X - Width / 2, _focusObject.Y - Height / 2,
            _focusObject.X + (Width - 1) / 2, _focusObject.Y + (Height - 1) / 2);

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;


    [JsonConstructor]
    private LevelView()
    {
    }
    public LevelView(Level level, IFocusable initialFocusObj)
    {
        _level = level;
        var (width, height) = _level.Size;
        _seenCells = new SeenTilesArray(width, height);
        _focusObject = initialFocusObj;
        Init();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _focusObject = _level.Player;
        Init();
    }

    private void Init()
    {
        _level.NpcDied += DeathHandler;
        _level.ActorDidSomething += ActorDidSomethingHandler;
        _sightedObjects = _level.SightedObjects;
        _sightedObjects.CollectionChanged += SightedObjectUpdateHandler;
        foreach (var sightedObject in _sightedObjects)
        {
            sightedObject.VisiblePointsChanged += UpdateVisiblePoints;
        }
        _focusObject.InFocus = true;
        
        UpdateVisiblePoints();
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, _focusObject.X, _focusObject.Y);
        var (levelX, levelY) = coords;
        
        ConsoleColor? backgroundColor = null;
        if (DrawCursorLine && _cursorLinePoints.Contains(coords)) 
            backgroundColor = GetCursorLineColor(levelX, levelY);
        
        if (_cursor is not null && _cursor.X == levelX && _cursor.Y == levelY) backgroundColor = _cursor.Color;
        
        if (x == centerScreenX && y == centerScreenY)
        {
            // Draw object in focus on top of everything else
            if (_focusObject is IDrawable drawable)
                return new ViewCellData(drawable.SymbolData, backgroundColor ?? drawable.BgColor);
        }
        
        if (_visiblePoints.Contains(coords))
        {
            IDrawable drawableObj = _level.GetDrawable(levelX, levelY);
            return drawableObj is null
                ? new ViewCellData(null, backgroundColor)
                : new ViewCellData(drawableObj.SymbolData, backgroundColor ?? drawableObj.BgColor);
        }

        if (_seenCells.Inbounds(levelX, levelY) && _seenCells[levelX, levelY] is not null)
            return new ViewCellData((_seenCells[levelX, levelY].Value, DataHolder.Colors.OutOfVisionTileColor),
                backgroundColor);
        
        return new ViewCellData(null, backgroundColor);
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, _focusObject) || focusObj is null) return;
        _focusObject.InFocus = false;
        _focusObject.Moved -= FocusObjectMoved;

        _focusObject = focusObj;
        _focusObject.InFocus = true;
        _focusObject.Moved += FocusObjectMoved;
        
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void SightedObjectUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (var sightedObject in e.NewItems)
            {
                ((ISightedObject) sightedObject).VisiblePointsChanged += UpdateVisiblePoints;
            }

        if (e.OldItems is not null)
            foreach (var sightedObject in e.OldItems)
            {
                ((ISightedObject) sightedObject).VisiblePointsChanged -= UpdateVisiblePoints;
            }

        UpdateVisiblePoints();
    }

    private void UpdateVisiblePoints(object sender = null, EventArgs _ = null)
    {
        _visiblePoints = _sightedObjects
            .SelectMany((obj, _) => GetPointsVisibleAround(obj.X, obj.Y, obj.GetViewRange(), obj.Xray))
            .ToHashSet();
        foreach (var (x, y) in _visiblePoints)
        {
            if (!_seenCells.Inbounds(x, y)) continue;
            var tile = _level.GetTile(x, y);
            if (tile is not null) _seenCells[x, y] = tile.SymbolData.symbol;
        }

        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void DeathHandler(object sender, EventArgs _)
    {
        if (sender is not Actor actor) return;

        if (!IsPointInsideView(actor.X, actor.Y)) return;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void FocusObjectMoved(object sender, MoveEventArgs _)
    {
        // Redraw happens on visible tile calculation already
        if (_focusObject is ISightedObject focusObject && _sightedObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void ActorDidSomethingHandler(object actor, ActorActionEventArgs actionInfo)
    {
        switch (actionInfo.Action)
        {
            case ActorAction.Shoot:
                PaintCells(ConsoleColor.Magenta, 10);
                break;
            case ActorAction.ApplyEffect:
                PaintCells(ConsoleColor.Yellow, 100);
                break;
            case ActorAction.MeleeAttackHit:
                PaintCells(ConsoleColor.Red, 100);
                break;
            case ActorAction.MeleeAttackMissed:
                PaintCells(ConsoleColor.DarkGray, 100);
                break;
            case ActorAction.InteractWithDoor:
                foreach ((int x, int y) in actionInfo.AffectedCells)
                {
                    if (CanPlayerSeePoint(x, y)) UpdateVisiblePoints();
                }
                UpdateVisiblePoints();
                break;
            case ActorAction.Move:
                foreach ((int x, int y) in actionInfo.AffectedCells)
                {
                    if (!CanPlayerSeePoint(x, y)) continue;
                    IDrawable drawable = _level.GetDrawable(x, y);
                    var (viewX, viewY) = ToViewCoords(x, y);
                    CellChanged?.Invoke(this,
                        new CellChangedEventArgs((viewX, viewY, new ViewCellData(drawable?.SymbolData, drawable?.BgColor), 0)));
                }
                break;
            case ActorAction.StopTurn:
            default:
                break;
        }

        void PaintCells(ConsoleColor color, int delay)
        {
            foreach ((int x, int y) in actionInfo.AffectedCells)
            {
                if (!CanPlayerSeePoint(x, y)) continue;
                IDrawable drawable = _level.GetDrawable(x, y);
                var (viewX, viewY) = ToViewCoords(x, y);
                CellChanged?.Invoke(this,
                    new CellChangedEventArgs((viewX, viewY, new ViewCellData(drawable?.SymbolData, color), delay)));
                CellChanged?.Invoke(this,
                    new CellChangedEventArgs((viewX, viewY, new ViewCellData(drawable?.SymbolData, drawable?.BgColor),
                        0)));
            }
        }
    }

    private bool CanPlayerSeePoint(int x, int y)
    {
        return _visiblePoints.Contains((x, y)) && IsPointInsideView(x, y);
    }

    private bool IsPointInsideView(int x, int y)
    {
        var (minX, minY, maxX, maxY) = ViewBounds;
        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    private (int screenX, int screenY) ToViewCoords(int xLevel, int yLevel)
    {
        var (minX, minY, _, _) = ViewBounds;
        return UtilityFunctions.ConvertAbsoluteToRelativeCoords(xLevel, yLevel, minX, minY);
    }
}