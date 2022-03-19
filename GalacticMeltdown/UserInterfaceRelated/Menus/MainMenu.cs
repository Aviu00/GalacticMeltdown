using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class MainMenu : Menu
{
    public MainMenu()
    {
        LineView = new LineView();
        LineView.SetLines(new List<ListLine>()
        {
            new Button("Select level", "", OpenLevelMenu),
            new Button("Quit", "", Game.Quit)
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }

    private void OpenLevelMenu()
    {
        
    }
}