namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class MenuButtonInfo
{
    public Button Button { get; }
    public string RenderedText { get; set; }

    public MenuButtonInfo(Button button)
    {
        Button = button;
        RenderedText = "";
    }
}