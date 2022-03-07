using System;

namespace GalacticMeltdown.Frontend.Rendering;

public struct ScreenCellData
{
    public readonly char Symbol;
    public readonly ConsoleColor FgColor;
    public readonly ConsoleColor BgColor;

    public ScreenCellData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
    {
        Symbol = symbol;
        BgColor = bgColor;
        FgColor = fgColor;
    }
}