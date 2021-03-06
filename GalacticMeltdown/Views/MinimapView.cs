using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

public class MinimapView : View
{
    private const char UndiscoveredSym = '◻';
    
    private Chunk[,] _chunks;
    private Player _player;

    public override event EventHandler NeedRedraw;

    public MinimapView(Chunk[,] chunks, Player player)
    {
        _chunks = chunks;
        _player = player;
        _player.Moved += PlayerMoveHandler;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerViewX = Width / 2, centerViewY = Height / 2;
        var (xPlayer, yPlayer) = Level.GetChunkCoords(_player.X, _player.Y);
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerViewX, centerViewY);
        var (xAbs, yAbs) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, xPlayer, yPlayer);

        ConsoleColor? bgColor = x == centerViewX && y == centerViewY ? Colors.Minimap.CenterChunk : null;
        if (!Inbounds(xAbs, yAbs)) return new ViewCellData(null, bgColor);
        if (_chunks[xAbs, yAbs].IsFinalRoom)
            bgColor ??= Colors.Minimap.FinalRoom;

        return new ViewCellData(
            _chunks[xAbs, yAbs].WasVisitedByPlayer
                ? (_chunks[xAbs, yAbs].Symbol, Colors.Minimap.Visited)
                : (UndiscoveredSym, Colors.Minimap.Undiscovered),
            bgColor);
    }

    public override ViewCellData[,] GetAllCells()
    {
        ViewCellData[,] cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        var (xPlayer, yPlayer) = Level.GetChunkCoords(_player.X, _player.Y);
        int minX = xPlayer - Width / 2, minY = yPlayer - Height / 2;
        for (int viewY = 0; viewY < Height; viewY++)
        {
            for (int viewX = 0; viewX < Width; viewX++)
            {
                int chunkX = minX + viewX, chunkY = minY + viewY;
                if (!Inbounds(chunkX, chunkY)) continue;
                cells[viewX, viewY] = new ViewCellData(_chunks[chunkX, chunkY].WasVisitedByPlayer
                        ? (_chunks[chunkX, chunkY].Symbol, Colors.Minimap.Visited)
                        : (UndiscoveredSym, Colors.Minimap.Undiscovered),
                    _chunks[chunkX, chunkY].IsFinalRoom ? Colors.Minimap.FinalRoom : null);
            }
        }

        cells[Width / 2, Height / 2] =
            new ViewCellData(cells[Width / 2, Height / 2].SymbolData, Colors.Minimap.CenterChunk);

        return cells;
    }

    private void PlayerMoveHandler(object sender, MoveEventArgs e)
    {
        var player = (Player) sender;
        if (Level.GetChunkCoords(e.OldX, e.OldY) != Level.GetChunkCoords(player.X, player.Y))
            NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private bool Inbounds(int x, int y)
    {
        return x >= 0 && x < _chunks.GetLength(0) && y >= 0 && y < _chunks.GetLength(1);
    }
}