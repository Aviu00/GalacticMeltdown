using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class EndGameMessage : TextWindow
{
    public EndGameMessage(string message)
    {
        LineView.SetLines(new List<ListLine>
        {
            new TextLine(message),
            new Button("OK", "", Close)
        });
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }

    public override void Open()
    {
        UserInterface.SetViewPositioner(this, new ExactViewPositioner(LineView, (0.3, 0.25, 0.6, 0.4)));
        UserInterface.SetController(this, Controller);
        UserInterface.TakeControl(this);
    }
}