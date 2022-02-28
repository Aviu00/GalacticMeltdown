using System;

namespace GalacticMeltdown.Rendering;

public readonly record struct ViewCellData(
    (char Symbol, ConsoleColor TextColor)? SymbolData, ConsoleColor? BackgroundColor);
