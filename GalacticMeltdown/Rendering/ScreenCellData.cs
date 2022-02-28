using System;

namespace GalacticMeltdown.Rendering;

public readonly record struct ScreenCellData(
    (char Symbol, ConsoleColor TextColor)? SymbolData, ConsoleColor? BackgroundColor);
