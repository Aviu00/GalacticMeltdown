using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class Button : PressableListLine
{
    private const ConsoleColor SelectedBgColor = Colors.MenuLine.Button.Selected;

    protected string TextLeft { get; }
    protected string TextRight { get; }
    protected string RenderedText;
    
    private bool _selected;
    
    private readonly Action _action;

    public (string left, string right) Text => (TextLeft, TextRight);

    protected ConsoleColor BgColor => _selected ? SelectedBgColor : DefaultColor;

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
        base.SetWidth(width);
        RenderedText = RenderText();
    }

    protected virtual string RenderText()
    {
        return UtilityFunctions.RenderText(Width, TextLeft, TextRight);
    }
}