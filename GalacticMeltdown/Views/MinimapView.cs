using System;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

public class MinimapView : View
{
    private Chunk[,] _chunks;
    private Func<(int, int)> _getPlayerChunk;

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangedEventArgs> CellChanged;
    public override event EventHandler<CellsChangedEventArgs> CellsChanged;

    public MinimapView(Chunk[,] chunks, Func<(int, int)> getPlayerChunk)
    {
        _chunks = chunks;
        _getPlayerChunk = getPlayerChunk;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerViewX = Width / 2, centerViewY = Height / 2;
        var (xPlayer, yPlayer) = _getPlayerChunk();
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerViewX, centerViewY);
        var (xAbs, yAbs) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, xPlayer, yPlayer);
        if (!Inbounds(xAbs, yAbs)) return new ViewCellData(null, null);
        return new ViewCellData((_chunks[xAbs, yAbs].Symbol, ConsoleColor.DarkYellow),
            x == centerViewX && y == centerViewY ? ConsoleColor.Green : null);
    }

    private bool Inbounds(int x, int y)
    {
        return x >= 0 && x < _chunks.GetLength(0) && y >= 0 && y < _chunks.GetLength(1);
    }
}