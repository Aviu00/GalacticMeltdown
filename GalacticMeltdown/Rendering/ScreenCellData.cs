using System;

namespace GalacticMeltdown.Rendering;

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