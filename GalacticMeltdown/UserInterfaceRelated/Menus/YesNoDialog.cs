using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class YesNoDialog : Dialog
{
    public YesNoDialog(Action<bool> sender, string message)
    {
        LineView = new LineView();
        LineView.SetLines(new List<ListLine>
        {
            new TextLine(message),
            new Button("Yes", "", () => { sender(true); UserInterface.Forget(this); }),
            new Button("No", "", () => { sender(false); UserInterface.Forget(this); }),
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Back, Close},
            {SelectionControl.Down, LineView.SelectNext},
            {SelectionControl.Up, LineView.SelectPrev}
        }));
    }
}