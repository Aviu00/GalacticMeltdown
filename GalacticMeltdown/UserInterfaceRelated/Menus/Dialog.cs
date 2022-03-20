using System;
using System.Collections.Generic;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public abstract class Dialog : Menu
{
    protected Action<List<object>> Sender;

    public Dialog(Action<List<object>> sender)
    {
        Sender = sender;
    }

    protected virtual void SendInfo()
    {
        UserInterface.Forget(this);
    }
}