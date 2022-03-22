using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public abstract class TextWindow
{
    protected LineView LineView = new();
    protected Controller Controller;
    
    public virtual void Open()
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