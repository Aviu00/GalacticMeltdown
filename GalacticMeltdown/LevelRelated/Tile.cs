using System;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.LevelRelated;

public class Tile : IDrawable
{
    private TileTypeData _typeData;

    public string Name => _typeData.Name;

    public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    public ConsoleColor? BgColor => null;

    public bool IsTransparent => _typeData.IsTransparent;
    public bool IsWalkable => _typeData.IsWalkable;

    public bool ConnectToWalls => _typeData.IsConnection;

    public string Id => _typeData.Id;

    public int MoveCost => _typeData.MoveCost;

    public readonly bool IsDoor;

    public readonly Action InteractWithDoor;

    public Tile(TileTypeData typeData)
    {
        _typeData = typeData;
        var splitId = Id.Split('-');
        if (splitId[^1] is not ("open" or "closed")) return;
        
        IsDoor = true;
        bool isClosed = splitId[^1] == "closed";
        string baseId = splitId.Aggregate("", (current, s) =>
        {
            if (s is "open" or "closed") return current;
            return current + s + "-";
        });
        TileTypeData closedData = isClosed ? typeData : DataHolder.TileTypes[baseId + "closed"];
        TileTypeData openData = !isClosed ? typeData : DataHolder.TileTypes[baseId + "open"];
        InteractWithDoor = () => _typeData = _typeData == closedData ? openData : closedData;
    }
}