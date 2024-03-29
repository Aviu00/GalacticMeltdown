using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.ActorActions;
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

public partial class LevelView : View, IFullRedraw, IOneCellAnim, IMultiCellUpdate
{
    [JsonProperty] private readonly Level _level;

    [JsonProperty] private IFocusable _focusedOn;

    private IFocusable FocusObject
    {
        get => _focusOnCursor && _cursor is not null ? _cursor : _focusedOn;
        set
        {
            if (value is not null) _focusedOn = value;
        }
    }

    private bool _showCoordinates;

    private ObservableCollection<ISightedObject> _sightedObjects;
    private HashSet<(int x, int y)> _visiblePoints;
    [JsonProperty] private SeenTilesArray _seenCells;

    [JsonIgnore]
    public bool ShowCoordinates
    {
        get => _showCoordinates;
        set
        {
            _showCoordinates = value;
            NeedRedraw?.Invoke(this, EventArgs.Empty);
        }
    }

    private string CoordinateString => $"X:{FocusObject.X} Y:{FocusObject.Y}";

    public (int minX, int minY, int maxX, int maxY) ViewBounds =>
        (FocusObject.X - Width / 2, FocusObject.Y - Height / 2,
            FocusObject.X + (Width - 1) / 2, FocusObject.Y + (Height - 1) / 2);

    public event EventHandler NeedRedraw;
    public event EventHandler<OneCellAnimEventArgs> OneCellAnim;
    public event EventHandler<MultiCellUpdateEventArgs> MultiCellUpdate;


    [JsonConstructor]
    private LevelView()
    {
    }

    public LevelView(Level level)
    {
        _level = level;
        var (width, height) = _level.Size;
        _seenCells = new SeenTilesArray(width, height);
        FocusObject = _level.Player;
        Init();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        FocusObject = _level.Player;
        Init();
    }

    private void Init()
    {
        _level.NpcDied += DeathHandler;
        _level.ActorDidSomething += ActorDidSomethingHandler;
        _level.TileChanged += TileChangedHandler;
        _sightedObjects = _level.SightedObjects;
        _sightedObjects.CollectionChanged += SightedObjectUpdateHandler;
        foreach (var sightedObject in _sightedObjects)
        {
            sightedObject.VisiblePointsChanged += UpdateVisiblePoints;
        }

        UpdateVisiblePoints();
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        ConsoleColor? backgroundColor = null;

        if (ShowCoordinates && y == Height - 1 && x < CoordinateString.Length)
            return CoordStringCell(CoordinateString[x]);

        int centerScreenX = Width / 2, centerScreenY = Height / 2;
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerScreenX, centerScreenY);
        coords = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, FocusObject.X, FocusObject.Y);
        var (levelX, levelY) = coords;

        if (_cursor?.LinePoints is not null && _cursor.LinePoints.Contains(coords))
            backgroundColor = GetCursorLineColor(levelX, levelY);

        if (_cursor is not null && _cursor.X == levelX && _cursor.Y == levelY) backgroundColor = _cursor.Color;

        if (x == centerScreenX && y == centerScreenY)
        {
            // Draw object in focus on top of everything else
            if (FocusObject is IDrawable drawable)
                return new ViewCellData(drawable.SymbolData, backgroundColor ?? drawable.BgColor);
        }

        ViewCellData cellData = GetLevelCell(levelX, levelY);
        return cellData with {BackgroundColor = backgroundColor ?? cellData.BackgroundColor};
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        int minX = FocusObject.X - Width / 2, minY = FocusObject.Y - Height / 2;
        for (int viewY = 0; viewY < Height; viewY++)
        {
            for (int viewX = 0; viewX < Width; viewX++)
            {
                int levelX = minX + viewX, levelY = minY + viewY;
                cells[viewX, viewY] = GetLevelCell(levelX, levelY);
            }
        }

        if (FocusObject is IDrawable drawable)
            cells[Width / 2, Height / 2] = new ViewCellData(drawable.SymbolData, drawable.BgColor);

        if (_cursor is not null) // contract: cursor is always inside view
        {
            int viewX, viewY;

            if (_cursor.LinePoints is not null)
            {
                foreach ((int x, int y) in _cursor.LinePoints.Where(point => IsPointInsideView(point.x, point.y)))
                {
                    (viewX, viewY) = ToViewCoords(x, y);
                    cells[viewX, viewY] = new ViewCellData(cells[viewX, viewY].SymbolData, GetCursorLineColor(x, y));
                }
            }

            (viewX, viewY) = ToViewCoords(_cursor.X, _cursor.Y);
            cells[viewX, viewY] = new ViewCellData(cells[viewX, viewY].SymbolData, _cursor.Color);
        }

        if (ShowCoordinates)
        {
            string coordinateString = CoordinateString;
            for (int i = 0; i < Math.Min(coordinateString.Length, Width); i++)
            {
                cells[i, Height - 1] = CoordStringCell(coordinateString[i]);
            }
        }

        return cells;
    }

    public void SetFocus(IFocusable focusObj)
    {
        if (ReferenceEquals(focusObj, FocusObject) || focusObj is null) return;
        FocusObject.Moved -= FocusObjectMoved;

        FocusObject = focusObj;
        FocusObject.Moved += FocusObjectMoved;

        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    public List<string> GetDescription(int x, int y)
    {
        IHasDescription descriptionObject =
            _level.GetNonTileObject(x, y) as IHasDescription ?? _level.GetTile(x, y);
        if (descriptionObject is null) return null;
        return _visiblePoints.Contains((x, y)) ? descriptionObject.GetDescription() : new List<string> {"Not visible"};
    }

    private ViewCellData GetLevelCell(int levelX, int levelY)
    {
        if (_visiblePoints.Contains((levelX, levelY)))
        {
            IDrawable drawableObj = _level.GetDrawable(levelX, levelY);
            return new ViewCellData(drawableObj?.SymbolData, drawableObj?.BgColor);
        }

        if (_seenCells.Inbounds(levelX, levelY) && _seenCells[levelX, levelY] is not null)
            return new ViewCellData((_seenCells[levelX, levelY].Value, Colors.OutOfView), null);
        return new ViewCellData(null, null);
    }

    private void SightedObjectUpdateHandler(object _, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (ISightedObject sightedObject in e.NewItems)
            {
                sightedObject.VisiblePointsChanged += UpdateVisiblePoints;
            }

        if (e.OldItems is not null)
            foreach (ISightedObject sightedObject in e.OldItems)
            {
                sightedObject.VisiblePointsChanged -= UpdateVisiblePoints;
            }

        UpdateVisiblePoints();
    }

    private void UpdateVisiblePoints(object sender = null, EventArgs _ = null)
    {
        _visiblePoints = _sightedObjects
            .SelectMany((obj, _) => GetPointsVisibleAround(obj.X, obj.Y, obj.GetViewRange(), obj.Xray))
            .ToHashSet();
        foreach ((int x, int y) in _visiblePoints.Where(cell => _seenCells.Inbounds(cell.x, cell.y)))
        {
            Tile tile = _level.GetTile(x, y);
            if (tile is not null) _seenCells[x, y] = tile.SaveSymbol;
        }

        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void DeathHandler(object sender, EventArgs _)
    {
        if (sender is not IHasCoords hasCoords) return;

        (int x, int y) = (hasCoords.X, hasCoords.Y);
        if (!CanPlayerSeePoint((x, y))) return;
        (int viewX, int viewY) = ToViewCoords(x, y);
        ViewCellData cellData = GetSymbol(viewX, viewY);
        OneCellAnim?.Invoke(this, new OneCellAnimEventArgs((viewX, viewY, cellData, 0)));
    }

    private void FocusObjectMoved(object sender, EventArgs _)
    {
        // Redraw happens on visible tile calculation already
        if (FocusObject is ISightedObject focusObject && _sightedObjects.Contains(focusObject)) return;
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void TileChangedHandler(object sender, TileChangeEventArgs e)
    {
        if (_visiblePoints.Contains(e.Coords)) UpdateVisiblePoints();
    }

    private void ActorDidSomethingHandler(object actor, ActorActionEventArgs actionInfo)
    {
        switch (actionInfo.Action)
        {
            case ActorAction.Shoot:
                PaintCells(Colors.FlashAnim.Tracer, 10);
                break;
            case ActorAction.ApplyEffect:
                PaintCells(Colors.FlashAnim.Effect, 70);
                break;
            case ActorAction.MeleeAttackHit:
                PaintCells(Colors.FlashAnim.MeleeHit, 70);
                break;
            case ActorAction.MeleeAttackMissed:
                PaintCells(Colors.FlashAnim.MeleeMiss, 70);
                break;
            case ActorAction.Move:
                foreach ((int x, int y) in actionInfo.AffectedCells.Where(CanPlayerSeePoint))
                {
                    var (viewX, viewY) = ToViewCoords(x, y);
                    ViewCellData cellData = GetSymbol(viewX, viewY);
                    OneCellAnim?.Invoke(this, new OneCellAnimEventArgs((viewX, viewY, cellData, 20)));
                }

                break;
            case ActorAction.InteractWithDoor:
            case ActorAction.StopTurn:
            default:
                break;
        }

        void PaintCells(ConsoleColor color, int delay)
        {
            foreach ((int x, int y) in actionInfo.AffectedCells.Where(CanPlayerSeePoint))
            {
                (int viewX, int viewY) = ToViewCoords(x, y);
                ViewCellData cellData = GetSymbol(viewX, viewY);
                OneCellAnim?.Invoke(this,
                    new OneCellAnimEventArgs((viewX, viewY, cellData with {BackgroundColor = color}, delay)));
                OneCellAnim?.Invoke(this, new OneCellAnimEventArgs((viewX, viewY, cellData, 0)));
            }
        }
    }

    private bool CanPlayerSeePoint((int x, int y) pos)
    {
        return _visiblePoints.Contains(pos) && IsPointInsideView(pos.x, pos.y);
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

    private ViewCellData CoordStringCell(char sym) =>
        new(sym == ' ' ? null : (sym, Colors.Debug.Sym), Colors.Debug.Main);
}