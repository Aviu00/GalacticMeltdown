using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class PauseMenu : Menu
{
    public PauseMenu(PlaySession session)
    {
        LineView = new LineView();
        LineView.SetLines(new List<ListLine>
        {
            new Button("Back", "", Close),
            new Button("To main menu", "", session.Stop)
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>()
        {
            {SelectionControl.Back, Close},
            {SelectionControl.Down, LineView.SelectNext},
            {SelectionControl.Up, LineView.SelectPrev},
            {SelectionControl.Select, LineView.PressCurrent}
        }));
    }
}