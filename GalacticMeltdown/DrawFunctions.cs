using System;

namespace GalacticMeltdown;

public static class DrawFunctions
{
    public delegate SymbolData DrawFunc(int x, int y);
    public static SymbolData ScreenCordsMapDrawFunc(int x, int y)
    {
        (int x, int y) glCords = GameManager.ConsoleManager.ConvertScreenToGlobalCords(x, y);
        return MapDrawFunc(glCords.x, glCords.y);
    }

    public static SymbolData MapDrawFunc(int x, int y)
    {
        GameObject drawableObj;
        if (GameManager.Player.VisibleObjects.ContainsKey((x,y)))//if currently visible by player
        {
            drawableObj = GameManager.Player.VisibleObjects[(x,y)];
            return new SymbolData(drawableObj.Symbol, drawableObj.FGColor, drawableObj.BGColor);
        }

        drawableObj = GameManager.Map.GetTile(x, y);
        if (drawableObj != null && ((Tile) drawableObj).WasSeenByPlayer)//if not visible by player
        {
            return new SymbolData(drawableObj.Symbol, Utility.OutOfVisionTileColor, ConsoleColor.Black);
        }

        return new SymbolData(' ', Console.ForegroundColor, ConsoleColor.Black);
    }
    
    public struct SymbolData
    {
        public char Symbol;
        public ConsoleColor FGColor;
        public ConsoleColor BGColor;

        public SymbolData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
        {
            Symbol = symbol;
            BGColor = bgColor;
            FGColor = fgColor;
        }
    }
}