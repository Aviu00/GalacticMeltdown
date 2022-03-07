using System;

namespace GalacticMeltdown.Views;

public readonly record struct ViewCellData((char Symbol, ConsoleColor TextColor)? SymbolData,
    ConsoleColor? BackgroundColor);