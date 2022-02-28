using System;

namespace GalacticMeltdown.Rendering;

public delegate ScreenCellData GetSymbolAt(int x, int y);

public struct ScreenCellData
{
    public char Symbol;
    public ConsoleColor FgColor;
    public ConsoleColor BgColor;

    public ScreenCellData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
    {
        Symbol = symbol;
        BgColor = bgColor;
        FgColor = fgColor;
    }
}