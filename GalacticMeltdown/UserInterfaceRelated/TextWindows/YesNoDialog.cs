using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class YesNoDialog : TextWindow
{
    public YesNoDialog(Action<bool> sendInfo, string message)
    {
        LineView = new LineView();
        LineView.SetLines(new List<ListLine>
        {
            new TextLine(message),
            new Button("Yes", "", () => { UserInterface.Forget(this); sendInfo(true); }),
            new Button("No", "", () => { UserInterface.Forget(this); sendInfo(false); }),
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }
}