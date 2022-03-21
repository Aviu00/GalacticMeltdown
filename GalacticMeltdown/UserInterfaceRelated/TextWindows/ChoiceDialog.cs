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

public class ChoiceDialog<T> : TextWindow
{
    private Action<T> _send;
    
    public ChoiceDialog(IEnumerable<(string text, T choice)> options, Action<T> send)
    {
        _send = send;
        LineView = new LineView();
        LineView.SetLines(options.Select(info => new Button(info.text, "", () => SendChoice(info.choice)))
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

    private void SendChoice(T choice)
    {
        Close();
        _send(choice);
    }
}