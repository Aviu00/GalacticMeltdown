namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public abstract class Dialog : Menu
{
    protected virtual void SendInfo()
    {
        UserInterface.Forget(this);
    }
}