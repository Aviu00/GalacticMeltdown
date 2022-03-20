namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class MenuButtonInfo
{
    public ButtonTemp ButtonTemp { get; }
    public string RenderedText { get; set; }

    public MenuButtonInfo(ButtonTemp buttonTemp)
    {
        ButtonTemp = buttonTemp;
        RenderedText = "";
    }
}