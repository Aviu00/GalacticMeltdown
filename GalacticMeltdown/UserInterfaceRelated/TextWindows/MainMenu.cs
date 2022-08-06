using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class MainMenu : TextWindow
{
    public MainMenu()
    {
        LineView.SetLines(new List<ListLine>
        {
            new Button("Select level", "", OpenLevelMenu),
            new Button("Help", "", OpenHelp),
            new Button("Quit", "", Game.Quit)
        });
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(KeyBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }

    private void OpenLevelMenu()
    {
        LevelMenu levelMenu = new();
        UserInterface.AddChild(this, levelMenu);
        levelMenu.Open();
    }

    private void OpenHelp()
    {
        InfoWindow infoWindow = new(Text.InfoLines);
        UserInterface.AddChild(this, infoWindow);
        infoWindow.Open();
    }
}