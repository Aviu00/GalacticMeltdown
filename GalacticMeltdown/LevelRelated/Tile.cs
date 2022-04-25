using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

[JsonObject(IsReference = false)]
public class Tile : IDrawable, IHasDescription
{
    private TileTypeData _typeData;

    [JsonIgnore] public string Name => _typeData.Name;

    [JsonIgnore] public (char symbol, ConsoleColor color) SymbolData => (_typeData.Symbol, _typeData.Color);
    [JsonIgnore] public ConsoleColor? BgColor => null;

    [JsonIgnore] public bool IsTransparent => _typeData.IsTransparent;
    [JsonIgnore] public bool IsWalkable => _typeData.IsWalkable;

    [JsonIgnore] public bool ConnectToWalls => _typeData.IsConnection;

    public string Id => _typeData.Id;

    [JsonIgnore] public int MoveCost => _typeData.MoveCost;

    [JsonIgnore] public bool IsDoor => InteractWithDoor is not null;

    [JsonIgnore] public readonly Action InteractWithDoor;

    public Tile(TileTypeData typeData)
    {
        _typeData = typeData;
        var splitId = Id.Split('-');
        if (splitId[^1] is not ("open" or "closed")) return;
        
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

    [JsonConstructor]
    private Tile(string id) : this(DataHolder.TileTypes[id])
    {
    }

    public List<string> GetDescription()
    {
        return new()
        {
            Name,
            "",
            IsWalkable ? $"Move Cost: {MoveCost}" : "Impassible",
            IsTransparent ? "Transparent" : "Not Transparent"
        };
    }
}