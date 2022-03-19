using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public abstract class Menu
{
    protected LineView LineView;
    protected Controller Controller;
    
    public void Open()
    {
        UserInterface.SetView(this, LineView);
        UserInterface.SetController(this, Controller);
        UserInterface.TakeControl(this);
    }
    
    public void Close()
    {
        UserInterface.Forget(this);
    }
}