using System;

namespace GalacticMeltdown.Rendering;

public delegate ScreenPixelData GetSymbolAt(int x, int y);

public struct ScreenPixelData
{
    public char Symbol;
    public ConsoleColor FgColor;
    public ConsoleColor BgColor;

    public ScreenPixelData(char symbol, ConsoleColor fgColor, ConsoleColor bgColor)
    {
        Symbol = symbol;
        BgColor = bgColor;
        FgColor = fgColor;
    }
}