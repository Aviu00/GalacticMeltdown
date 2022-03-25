using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class Button : PressableListLine
{
    private const ConsoleColor SelectedBgColor = DataHolder.Colors.BackgroundColorSelected;
    private const ConsoleColor UnselectedBgColor = DataHolder.Colors.BackgroundColorUnselected;
    protected const ConsoleColor TextColor = DataHolder.Colors.TextColor;

    protected string TextLeft { get; }
    protected string TextRight { get; }
    protected string RenderedText;
    
    private bool _selected;
    
    private readonly Action _action;

    public (string left, string right) Text => (TextLeft, TextRight);

    protected ConsoleColor BgColor => _selected ? SelectedBgColor : UnselectedBgColor;

    public Button(string textLeft, string textRight, Action action)
    {
        TextLeft = textLeft;
        TextRight = textRight;
        _action = action;
    }

    public override ViewCellData this[int x] => new((RenderedText[x], TextColor), BgColor);

    public override void Select() => _selected = true;
    
    public override void Deselect() => _selected = false;

    public override void Press() => _action?.Invoke();

    public override void SetWidth(int width)
    {
        RenderedText = UtilityFunctions.RenderText(width, TextLeft, TextRight);
        base.SetWidth(width);
    }
}