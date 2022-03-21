using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class Button : PressableListLine
{
    private const ConsoleColor SelectedBgColor = DataHolder.Colors.BackgroundColorSelected;
    private const ConsoleColor UnselectedBgColor = DataHolder.Colors.BackgroundColorUnselected;
    private const ConsoleColor TextColor = DataHolder.Colors.TextColor;
    
    private string TextLeft { get; }
    private string TextRight { get; }
    private string _renderedText;
    
    private bool _selected;
    
    private readonly Action _action;

    public (string left, string right) Text => (TextLeft, TextRight);

    public Button(string textLeft, string textRight, Action action)
    {
        TextLeft = textLeft;
        TextRight = textRight;
        _action = action;
    }

    public override ViewCellData this[int x] =>
        new((_renderedText[x], TextColor), _selected ? SelectedBgColor : UnselectedBgColor);

    public override void Select() => _selected = true;
    
    public override void Deselect() => _selected = false;

    public override void Press() => _action?.Invoke();

    public override void SetWidth(int width)
    {
        _renderedText = RenderText(width);
        base.SetWidth(width);
    }

    protected string RenderText(int width)
    {
        const string ellipsis = "...";
        const string separator = "  ";
        const string noSpaceForRightText = $"{separator}{ellipsis}";
        int maxLeftStringLength = width - noSpaceForRightText.Length;
        string text;
        
        if (width < ellipsis.Length)
        {
            return new string(' ', width);
        }
        
        if (TextRight.Length == 0)
        {
            text = TextLeft.Length > width
                ? TextLeft.Substring(0, width - ellipsis.Length) + ellipsis
                : TextLeft.PadRight(width);
        }
        else if (TextLeft.Length == 0)
        {
            text = TextRight.Length > width
                ? ellipsis + TextRight.Substring(TextRight.Length - (width - ellipsis.Length))
                : TextRight.PadLeft(width);
        }
        else if (TextLeft.Length >= maxLeftStringLength)
        {
            if (maxLeftStringLength < 0) text = new string(' ', width);
            else text = TextLeft.Substring(0, maxLeftStringLength) + noSpaceForRightText;
        }
        else
        {
            text = TextLeft;
            text += separator;
            int spaceLeft = width - text.Length;
            text += TextRight.Length > spaceLeft
                ? TextRight.Substring(0, spaceLeft - ellipsis.Length) + ellipsis
                : TextRight.PadLeft(spaceLeft);
        }

        return text;
    }
}