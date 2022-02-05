using System;

namespace GalacticMeltdown.Rendering;

public static class DrawFunctions
{
    public delegate SymbolData GetSymbolAt(int x, int y);
}

public struct SymbolData
{
    public char Symbol;
    public ConsoleColor FgColor;
    public ConsoleColor BgColor;

    public SymbolData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
    {
        Symbol = symbol;
        BgColor = bgColor;
        FgColor = fgColor;
    }
}