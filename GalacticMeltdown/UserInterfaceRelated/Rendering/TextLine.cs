using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class TextLine : ListLine
{
    private readonly string _text;
    private string _renderedText;

    public TextLine(string text)
    {
        _text = text;
    }

    public override void SetWidth(int width)
    {
        base.SetWidth(width);
        _renderedText = UtilityFunctions.RenderText(Width, _text, "");
    }

    public override ViewCellData this[int x] => new((_renderedText[x], TextColor), DefaultColor);
}