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
        LineView.SetPos((0.3, 0.6, 0.6, 0.9));
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }
}