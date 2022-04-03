using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public abstract class TextWindow
{
    protected LineView LineView = new();
    protected Controller Controller;
    protected (double minX, double minY, double maxX, double maxY)? Position;

    public virtual void Open()
    {
        UserInterface.SetViewPositioner(this, new ExactViewPositioner(LineView, Position));
        UserInterface.SetController(this, Controller);
        UserInterface.TakeControl(this);
    }

    public virtual void Close()
    {
        UserInterface.Forget(this);
    }
}