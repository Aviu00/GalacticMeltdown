using System;
using System.Collections.Generic;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public abstract class Dialog : Menu
{
    protected List<ValueType> Info;
    protected Action<List<ValueType>> Sender;

    public Dialog(Action<List<ValueType>> sender)
    {
        Sender = sender;
    }

    protected virtual void SendInfo()
    {
        UserInterface.Forget(this);
        Sender(Info);
    }
}