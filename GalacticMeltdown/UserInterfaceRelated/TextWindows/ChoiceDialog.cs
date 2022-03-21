using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class ChoiceDialog : TextWindow
{
    private Action<string> _send;
    
    public ChoiceDialog(List<string> choices, Action<string> send)
    {
        _send = send;
        LineView = new LineView();
        LineView.SetLines(choices.Select(choice => new Button(choice, "", () => SendChoice(choice)))
            .Cast<ListLine>()
            .ToList());
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent},
            }));
    }

    private void SendChoice(string choice)
    {
        UserInterface.Forget(this);
        _send(choice);
    }
}