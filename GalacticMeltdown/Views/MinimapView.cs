using System;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Views;

public class MinimapView : View, IFullRedraw
{
    private const char UndiscoveredSym = '◻';
    
    private Chunk[,] _chunks;
    private Player _player;

    public event EventHandler NeedRedraw;

    public MinimapView(Chunk[,] chunks, Player player)
    {
        _chunks = chunks;
        _player = player;
        _player.Moved += PlayerMoveHandler;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        int centerX = Width / 2, centerY = Height / 2;
        var (xPlayer, yPlayer) = Level.GetChunkCoords(_player.X, _player.Y);
        var coords = UtilityFunctions.ConvertAbsoluteToRelativeCoords(x, y, centerX, centerY);
        var (chunkX, chunkY) = UtilityFunctions.ConvertRelativeToAbsoluteCoords(coords.x, coords.y, xPlayer, yPlayer);

        ConsoleColor? bgColor = x == centerX && y == centerY ? Colors.Minimap.CenterChunk : null;
        if (!Inbounds(chunkX, chunkY)) return new ViewCellData(null, bgColor);

        ViewCellData cellData = GetChunkCell(chunkX, chunkY);
        return cellData with {BackgroundColor = bgColor ?? cellData.BackgroundColor};
    }

    public override ViewCellData[,] GetAllCells()
    {
        var cells = new ViewCellData[Width, Height];
        cells.Initialize();
        if (Width == 0 || Height == 0) return cells;
        (int xPlayer, int yPlayer) = Level.GetChunkCoords(_player.X, _player.Y);
        int minX = xPlayer - Width / 2, minY = yPlayer - Height / 2;
        for (var viewY = 0; viewY < Height; viewY++)
        {
            for (var viewX = 0; viewX < Width; viewX++)
            {
                int chunkX = minX + viewX, chunkY = minY + viewY;
                if (!Inbounds(chunkX, chunkY)) continue;
                cells[viewX, viewY] = GetChunkCell(chunkX, chunkY);
            }
        }

        cells[Width / 2, Height / 2] = cells[Width / 2, Height / 2] with {BackgroundColor = Colors.Minimap.CenterChunk};

        return cells;
    }

    private ViewCellData GetChunkCell(int chunkX, int chunkY) => new(_chunks[chunkX, chunkY].WasVisitedByPlayer
            ? (_chunks[chunkX, chunkY].Symbol, Colors.Minimap.Visited)
            : (UndiscoveredSym, Colors.Minimap.Undiscovered),
        _chunks[chunkX, chunkY].IsFinalRoom ? Colors.Minimap.FinalRoom : null);

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