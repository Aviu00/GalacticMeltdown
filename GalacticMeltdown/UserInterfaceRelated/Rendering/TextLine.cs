using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class TextLine : ListLine
{
    private const ConsoleColor BgColor = DataHolder.Colors.BackgroundColorUnselected;
    private const ConsoleColor TextColor = DataHolder.Colors.TextColor;

    private readonly string _text;
    private string _renderedText;

    public TextLine(string text)
    {
        _text = text;
    }

    public override void SetWidth(int width)
    {
        const string ellipsis = "...";

        if (width < ellipsis.Length)
        {
            _renderedText = new string(' ', width);
            base.SetWidth(width);
            return;
        }

        _renderedText = _text.Length > width
            ? _text.Substring(0, width - ellipsis.Length) + ellipsis
            : _text.PadRight(width);
        base.SetWidth(width);
    }

    public override ViewCellData this[int x] => new((_renderedText[x], TextColor), BgColor);
}